using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SyncFlash.Services
{
    public interface IConfigService
    {
        BindingList<Project> GetProjects();
        bool SaveProjects(BindingList<Project> projects);
    }

    public class ConfigService : IConfigService
    {
        private readonly string _filePath;
        private const string RootXMLProject = "Projects";
        private const string ProjXML = "Project";

        public ConfigService(string filePath)
        {
            _filePath = filePath;
        }

        public BindingList<Project> GetProjects()
        {
            if (!File.Exists(_filePath)) return new BindingList<Project>();

            var doc = XDocument.Load(_filePath);
            var list = doc.Descendants(ProjXML)
                          .Select(p =>
                          {
                              var project = new Project(p.Attribute("name")?.Value ?? "Unknown")
                              {
                                  LastSyncTime = DateTime.TryParse(p.Attribute("lastSync")?.Value, out var date) ? date : (DateTime?)null,
                                  LastSyncSize = long.TryParse(p.Attribute("lastSize")?.Value, out var size) ? size : 0,
                                  AutoSync = bool.TryParse(p.Attribute("autoSync")?.Value, out var auto) && auto,
                                  ExceptionDirs = p.Elements("ExceptionDir").Select(x => x.Value).ToList(),
                              };

                              project.AllProjectDirs = p.Elements("Directory")
                                  .Select(x =>
                                  {
                                      string netBios = x.Attribute("NetBios")?.Value ?? Environment.MachineName;
                                      string savedPath = x.Attribute("path")?.Value ?? "";

                              // Если папка была на флешке, добавляем текущую букву диска
                              string finalPath = (netBios == CONSTS.FlashDrive)
                                          ? Path.Combine(CONSTS.GetDriveLetter(), savedPath)
                                          : savedPath;

                                      return new Projdir(finalPath, project, netBios);
                                  })
                                  .ToList();

                              return project;
                          })
                          .OrderByDescending(p => p.LastSyncTime)
                          .ToList();

            return new BindingList<Project>(list);
        }

        public bool SaveProjects(BindingList<Project> projects)
        {
            if (projects == null || projects.Count == 0) return false;

            var doc = new XDocument(new XElement(RootXMLProject,
                projects.Select(p => new XElement(ProjXML,
                    new XAttribute("name", p.Name),
                    new XAttribute("lastSync", p.LastSyncTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),
                    new XAttribute("lastSize", p.LastSyncSize),
                    new XAttribute("autoSync", p.AutoSync),
                    p.ExceptionDirs.Select(ex => new XElement("ExceptionDir", ex)),
                    p.AllProjectDirs.Select(dir =>
                    {
                // Если папка на флешке, сохраняем путь без буквы диска
                string pathToSave = (dir.PC_Name == CONSTS.FlashDrive)
                            ? Form1.GetRelationPath(dir.Dir, CONSTS.GetDriveLetter()) // Оставляем относительный путь
                            : dir.Dir; // Полный путь для папок на ПК

                return new XElement("Directory",
                            new XAttribute("NetBios", dir.PC_Name),
                            new XAttribute("path", pathToSave)
                        );
                    })
                ))
            ));

            doc.Save(_filePath);
            return true;
        }
    }
}
