using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PurchaseForMe.Actors.User;
using PurchaseForMe.Configuration;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.Project.OpenedProjectStore;
using PurchaseForMe.Core.User;
using PurchaseForMe.Core.User.Message;

namespace PurchaseForMe.Actors.Project
{
    public delegate IActorRef ProjectManagerFactory();
    public class ProjectManager : ReceiveActor, IWithUnboundedStash
    {
        private readonly IOptions<ProjectSettings> _options;
        private readonly IActorRef _userManager;
        private IActorRef _openedProjectStore;
        private IActorRef _callingActor;
        //We want to stash any messages that may have come in when the manager is not in the normal state.
        public IStash Stash { get; set; }
        public ProjectManager(IOptions<ProjectSettings> options, UserManagerActorFactory userManagerActorFactory)
        {
            _options = options;
            _userManager = userManagerActorFactory();
            _openedProjectStore = Context.ActorOf(Props.Create(() => new OpenedProjectsStore()), "openedProjectsStore");
            setupProjectManager();
            Become(NormalState);
        }

        protected void NormalState()
        {
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
                GetUserResponseMessage user = null;
                if (string.IsNullOrEmpty(message.UserId))
                {
                    user = await _userManager.Ask<GetUserResponseMessage>(new GetLoggedInUserMessage());
                }
                else
                {
                    user = await _userManager.Ask<GetUserResponseMessage>(new GetUserByIdMessage(message.UserId));
                }
                _openedProjectStore.Tell(new GetOpenedProjectMessage(message.UserId, message.ProjectGuid), Self);
                _callingActor = Sender;
                Become(OpeningProject);
            });
            ReceiveAsync<SaveProjectMessage>(async message =>
            {
                string userId = "";
                if (string.IsNullOrEmpty(message.UserId))
                {
                    userId = (await _userManager.Ask<GetUserResponseMessage>(new GetLoggedInUserMessage())).User.Id;
                }
                else
                {
                    userId = message.UserId;
                }
                _openedProjectStore.Tell(new GetOpenedProjectMessage(userId, message.ProjectGuid));
                Become(SavingProject);
            });
            Receive<CloseAllUserProjectsMessage>(message =>
            {
                _openedProjectStore.Tell(message, Self);
            });
        }
        protected void OpeningProject()
        {
            Receive<GetProjectResponseMessage>(message =>
            {
                _callingActor.Tell(new GetProjectResponseMessage(message.Project));
                Stash.UnstashAll();
                Become(NormalState);
            });
            ReceiveAsync<ProjectNotOpenedMessage>(async message =>
            {
                var projects = await getAllProjects();
                var project = projects
                    .FirstOrDefault(s => s.ProjectGuid.Equals(message.ProjectGuid));

                Sender.Tell(new OpenProjectMessage(message.UserId, project));
                _callingActor.Tell(new GetProjectResponseMessage(project));
                Stash.UnstashAll();
                Become(NormalState);
            });
            ReceiveAny(message => Stash.Stash());
        }

        protected void SavingProject()
        {
            Receive<ProjectNotOpenedMessage>(message =>
            {
                Stash.UnstashAll();
                Become(NormalState);
            });
            ReceiveAsync<GetProjectResponseMessage>(async message =>
            {
                await saveNewProjectAsync(message.Project, true);
                Stash.UnstashAll();
                Become(NormalState);
            });
            ReceiveAny(message => Stash.Stash());
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