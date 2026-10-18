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

                var list = doc.Descendants()
                              .Where(x => string.Equals(x.Name.LocalName, ProjXML, StringComparison.OrdinalIgnoreCase))
                              .Select(p =>
                              {
                                  var lastSyncValue = GetAttributeValue(p, "lastSync");
                                  var lastSizeValue = GetAttributeValue(p, "lastSize");
                                  var autoSyncValue = GetAttributeValue(p, "autoSync", "Autosync");

                                  var project = new Project(GetAttributeValue(p, "name") ?? "Unknown")
                                  {
                                      LastSyncTime = DateTime.TryParse(
                                          lastSyncValue,
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.None,
                                          out var date) ? date : null,
                                      LastSyncSize = long.TryParse(
                                          lastSizeValue,
                                          NumberStyles.Integer,
                                          CultureInfo.InvariantCulture,
                                          out var size) ? size : 0,
                                      AutoSync = bool.TryParse(autoSyncValue, out var auto) && auto,
                                      ExceptionDirs = ElementsNamed(p, "ExceptionDir")
                                          .Select(x => GetAttributeValue(x, "path") ?? string.Empty)
                                          .Where(x => x.Length > 0)
                                          .ToList()
                                  };

                                  project.AllProjectDirs = ElementsNamed(p, "Directory")
                                      .Select(x =>
                                      {
                                          var netBios = GetAttributeValue(x, "NetBios") ?? Environment.MachineName;
                                          var savedPath = GetAttributeValue(x, "path") ?? string.Empty;

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

        /// <summary>
        /// Читает атрибут без учёта регистра: старые конфиги используют Autosync/directory.
        /// </summary>
        private static string? GetAttributeValue(XElement element, params string[] names)
        {
            foreach (var name in names)
            {
                var attribute = element.Attributes().FirstOrDefault(a =>
                    string.Equals(a.Name.LocalName, name, StringComparison.OrdinalIgnoreCase));

                if (attribute is not null) return attribute.Value;
            }

            return null;
        }

        private static IEnumerable<XElement> ElementsNamed(XElement parent, string name) =>
            parent.Elements().Where(x => string.Equals(x.Name.LocalName, name, StringComparison.OrdinalIgnoreCase));

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
