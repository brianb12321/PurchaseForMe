using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PurchaseForMe.Configuration;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.User;

namespace PurchaseForMe.Actors.Project
{
    public delegate IActorRef ProjectManagerFactory();
    public class ProjectManager : ReceiveActor
    {
        private readonly IOptions<ProjectSettings> _options;
        private readonly UserManager<PurchaseForMeUser> _userManager;
        private readonly ConcurrentDictionary<string, UserProjects> _openedProjects;
        private readonly ClaimsPrincipal _currentUser;
        public ProjectManager(IOptions<ProjectSettings> options, UserManager<PurchaseForMeUser> userManager, ClaimsPrincipal currentUser)
        {
            _options = options;
            _userManager = userManager;
            _openedProjects = new ConcurrentDictionary<string, UserProjects>();
            _currentUser = currentUser;
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
                var projects = await getAllProjects();

                var projectSummaries = projects.
                    Select(p => new ProjectSummary(p.ProjectGuid, p.Name))
                    .ToList();

                Sender.Tell(new ProjectEnumerationMessage(projectSummaries));
            });
            ReceiveAsync<GetProjectMessage>(async message =>
            {
                var user = await _userManager.GetUserAsync(_currentUser);

                //Get project from cache
                if (_openedProjects.ContainsKey(user.Id) && _openedProjects[user.Id].OpenedProjects.Any(p => p.ProjectGuid == message.ProjectGuid))
                {
                    Sender.Tell(new GetProjectResponseMessage(_openedProjects[user.Id].OpenedProjects.FirstOrDefault(p => p.ProjectGuid == message.ProjectGuid)));
                    return;
                }
                //If project is not in cache
                //Need better way to do this.
                var projects = await getAllProjects();
                var project = projects
                    .FirstOrDefault(s => s.ProjectGuid.Equals(message.ProjectGuid));

                if (!_openedProjects.ContainsKey(user.Id))
                {
                    UserProjects openedProjects = new UserProjects(user);
                    openedProjects.OpenedProjects.Add(project);
                    _openedProjects.AddOrUpdate(user.Id, openedProjects, (id, p) => openedProjects);
                }
                else
                {
                    _openedProjects[user.Id].OpenedProjects.Add(project);
                }
                Sender.Tell(new GetProjectResponseMessage(project));
            });
            ReceiveAsync<SaveProjectMessage>(async message =>
            {
                var user = await _userManager.GetUserAsync(_currentUser);
                if (!_openedProjects.ContainsKey(user.Id) || _openedProjects[user.Id].OpenedProjects.Any(p => p.ProjectGuid != message.ProjectGuid))
                {
                    throw new ArgumentException($"Project {message.ProjectGuid} must be opened by a user.");
                }

                await saveNewProjectAsync(
                    _openedProjects[user.Id].OpenedProjects.FirstOrDefault(p => p.ProjectGuid == message.ProjectGuid),
                    true);
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
        private async Task<IEnumerable<ProjectInstance>> getAllProjects()
        {
            List<ProjectInstance> projects = new List<ProjectInstance>();
            foreach (string projectDir in Directory.GetDirectories(_options.Value.ProjectBasePath, "*", SearchOption.TopDirectoryOnly))
            {
                if (File.Exists(Path.Combine(projectDir, "project.json")))
                {
                    ProjectInstance project =
                        JsonConvert.DeserializeObject<ProjectInstance>(
                            await File.ReadAllTextAsync(Path.Combine(projectDir, "project.json")), new JsonSerializerSettings()
                            {
                                TypeNameHandling = TypeNameHandling.Objects
                            });

                    projects.Add(project);
                }
            }

            return projects;
        }
        private async Task saveNewProjectAsync(ProjectInstance project, bool overwrite = false)
        {
            string projectGuid = project.ProjectGuid.ToString();
            if (Directory.Exists(Path.Combine(_options.Value.ProjectBasePath, projectGuid)) && !overwrite)
                throw new ArgumentException("Project guid already exists on the filesystem.");

            string projectJson = JObject.FromObject(project, JsonSerializer.Create(new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            })).ToString();
            string newProjectPath = Path.Combine(_options.Value.ProjectBasePath, projectGuid);
            Directory.CreateDirectory(newProjectPath);
            await File.WriteAllTextAsync(Path.Combine(newProjectPath, "project.json"), projectJson);
        }
    }
}