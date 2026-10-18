using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace SyncFlash.Services
{
    public interface IConfigService
    {
        BindingList<Project> GetProjects();
        bool SaveProjects(BindingList<Project> projects);
        string FilePath { get; }
    }

    public class ConfigService : IConfigService
    {
        private readonly string _filePath;
        private const string RootXMLProject = "Projects";
        private const string ProjXML = "Project";
        private const string BackupSuffix = ".bak";

        public ConfigService(string filePath)
        {
            _filePath = Path.IsPathRooted(filePath)
                ? filePath
                : Path.Combine(AppContext.BaseDirectory, filePath);
        }

        public string FilePath => _filePath;

        public BindingList<Project> GetProjects()
        {
            if (!File.Exists(_filePath)) return new BindingList<Project>();

            try
            {
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                };

                using var stream = File.OpenRead(_filePath);
                using var reader = XmlReader.Create(stream, settings);
                var doc = XDocument.Load(reader);

                var list = doc.Descendants(ProjXML)
                              .Select(p =>
                              {
                                  var project = new Project(p.Attribute("name")?.Value ?? "Unknown")
                                  {
                                      LastSyncTime = DateTime.TryParse(
                                          p.Attribute("lastSync")?.Value,
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.None,
                                          out var date) ? date : null,
                                      LastSyncSize = long.TryParse(
                                          p.Attribute("lastSize")?.Value,
                                          NumberStyles.Integer,
                                          CultureInfo.InvariantCulture,
                                          out var size) ? size : 0,
                                      AutoSync = bool.TryParse(p.Attribute("autoSync")?.Value, out var auto) && auto,
                                      ExceptionDirs = p.Elements("ExceptionDir")
                                          .Select(x => x.Attribute("path")?.Value ?? string.Empty)
                                          .Where(x => x.Length > 0)
                                          .ToList()
                                  };

                                  project.AllProjectDirs = p.Elements("Directory")
                                      .Select(x =>
                                      {
                                          var netBios = x.Attribute("NetBios")?.Value ?? Environment.MachineName;
                                          var savedPath = x.Attribute("path")?.Value ?? string.Empty;

                                          var finalPath = netBios == CONSTS.FlashDrive
                                              ? CONSTS.CombineWithDriveLetter(savedPath)
                                              : savedPath;

                                          return new Projdir(finalPath, project, netBios);
                                      })
                                      .ToList();

                                  return project;
                              })
                              // Не трогаем диск при загрузке: сортировка по доступности была источником зависаний.
                              .OrderByDescending(x => x.AllProjectDirs.Count)
                              .ThenByDescending(p => p.LastSyncTime)
                              .ToList();

                return new BindingList<Project>(list);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is XmlException || ex is InvalidOperationException)
            {
                TryBackupCorruptFile();
                return new BindingList<Project>();
            }
        }

        private void TryBackupCorruptFile()
        {
            try
            {
                if (!File.Exists(_filePath)) return;
                var backupPath = $"{_filePath}.corrupt-{DateTime.Now:yyyyMMdd-HHmmss}";
                File.Move(_filePath, backupPath);
            }
            catch
            {
                // Не мешаем приложению запуститься с пустым списком проектов.
            }
        }

        public bool SaveProjects(BindingList<Project> projects)
        {
            if (projects is null) return false;

            var tempPath = _filePath + ".tmp";

            try
            {
                var doc = new XDocument(new XElement(RootXMLProject, projects.Select(ToXElement)));

                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                doc.Save(tempPath);

                if (File.Exists(_filePath))
                {
                    File.Replace(tempPath, _filePath, _filePath + BackupSuffix, ignoreMetadataErrors: true);
                }
                else
                {
                    File.Move(tempPath, _filePath);
                }

                return true;
            }
            catch
            {
                try
                {
                    if (File.Exists(tempPath)) File.Delete(tempPath);
                }
                catch
                {
                    // Игнорируем ошибку очистки временного файла.
                }

                return false;
            }
        }

        private static XElement ToXElement(Project project) => new(ProjXML,
            new XAttribute("name", project.Name),
            new XAttribute("lastSync", project.LastSyncTime?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? string.Empty),
            new XAttribute("lastSize", project.LastSyncSize),
            new XAttribute("autoSync", project.AutoSync),
            project.ExceptionDirs.Select(ex => new XElement("ExceptionDir", new XAttribute("path", ex))),
            project.AllProjectDirs.Select(dir => new XElement("Directory",
                new XAttribute("NetBios", dir.PC_Name),
                // Для флешки _dir уже хранится как относительный путь с ведущим "\\".
                new XAttribute("path", dir.PC_Name == CONSTS.FlashDrive ? dir._dir : dir.Dir))));
    }
}
