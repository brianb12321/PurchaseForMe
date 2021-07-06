using System.Collections.Concurrent;
using System.Linq;
using Akka.Actor;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.Project.OpenedProjectStore;

namespace PurchaseForMe.Project
{
    public class OpenedProjectsStore : ReceiveActor
    {
        private readonly ConcurrentDictionary<string, UserProjects> _openedProjects;

        public OpenedProjectsStore()
        {
            _openedProjects = new ConcurrentDictionary<string, UserProjects>();
            Receive<CloseAllUserProjectsMessage>(message =>
            {
                //Close projects even if unsaved. The user should have saved their work.
                if (_openedProjects.ContainsKey(message.UserId))
                {
                    _openedProjects[message.UserId].OpenedProjects.Clear();
                }
            });
            Receive<OpenProjectMessage>(message =>
            {
                if (!_openedProjects.ContainsKey(message.UserId))
                {
                    UserProjects projects = new UserProjects(message.UserId);
                    projects.OpenedProjects.Add(message.Project);
                    _openedProjects.TryAdd(message.UserId, projects);
                }
                else
                {
                    _openedProjects[message.UserId].OpenedProjects.Add(message.Project);
                }
            });
            Receive<GetOpenedProjectMessage>(message =>
            {
                if (!_openedProjects.ContainsKey(message.UserId) || _openedProjects[message.UserId].OpenedProjects.All(p => p.ProjectGuid != message.ProjectGuid))
                {
                    Sender.Tell(new ProjectNotOpenedMessage(message.ProjectGuid, message.UserId), Self);
                }
                else
                {
                    Sender.Tell(new GetProjectResponseMessage(_openedProjects[message.UserId].OpenedProjects.First(p => p.ProjectGuid == message.ProjectGuid)), Self);
                }
            });
        }
    }
}