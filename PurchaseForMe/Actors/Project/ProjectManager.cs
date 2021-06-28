using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PurchaseForMe.Configuration;
using PurchaseForMe.Core.Project;

namespace PurchaseForMe.Actors.Project
{
    public delegate IActorRef ProjectManagerFactory();
    public class ProjectManager : ReceiveActor
    {
        private readonly IOptions<ProjectSettings> _options;
        public ProjectManager(IOptions<ProjectSettings> options)
        {
            _options = options;
            ReceiveAsync<CreateProjectMessage>(async message =>
            {
                ProjectInstance project = new ProjectInstance();
                project.Name = message.ProjectName;
                project.Description = message.Description;
                project.ProjectItems.AddRange(message.ProjectItems);
                try
                {
                    await saveNewProjectAsync(project);
                    Sender.Tell(new CreateProjectResultMessage(true, $"Project {project.Name} was successfully created", new ProjectSummary(project.ProjectGuid, project.Description)));
                }
                catch (Exception e)
                {
                    Sender.Tell(new CreateProjectResultMessage(false, $"An error occurred while creating a project: {e.Message}"));
                }
            });
            ReceiveAsync<GetAllProjectsMessage>(async message =>
            {
                Sender.Tell(await getAllProjects());
            });
            setupProjectManager();
        }

        private void setupProjectManager()
        {
            if (!Directory.Exists(_options.Value.ProjectBasePath))
            {
                Directory.CreateDirectory(_options.Value.ProjectBasePath);
            }
        }
        private async Task<ProjectEnumerationMessage> getAllProjects()
        {
            List<ProjectSummary> projects = new List<ProjectSummary>();
            foreach (string projectDir in Directory.GetDirectories(_options.Value.ProjectBasePath, "*", SearchOption.TopDirectoryOnly))
            {
                if (File.Exists(Path.Combine(projectDir, "project.json")))
                {
                    ProjectInstance project =
                        JsonConvert.DeserializeObject<ProjectInstance>(
                            await File.ReadAllTextAsync(Path.Combine(projectDir, "project.json")));

                    projects.Add(new ProjectSummary(project.ProjectGuid, project.Name));
                }
            }
            ProjectEnumerationMessage enumeration = new ProjectEnumerationMessage(projects);
            return enumeration;

        }
        private async Task saveNewProjectAsync(ProjectInstance project)
        {
            string projectGuid = project.ProjectGuid.ToString();
            if (Directory.Exists(Path.Combine(_options.Value.ProjectBasePath, projectGuid)))
                throw new ArgumentException("Project guid already exists on the filesystem.");

            string projectJson = JObject.FromObject(project).ToString();
            string newProjectPath = Path.Combine(_options.Value.ProjectBasePath, projectGuid);
            Directory.CreateDirectory(newProjectPath);
            await File.WriteAllTextAsync(Path.Combine(newProjectPath, "project.json"), projectJson);
        }
    }
}