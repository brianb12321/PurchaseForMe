using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PurchaseForMe.Core;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.Project.Nodes.Blockly;

namespace PurchaseForMeWeb.Areas.Project.Pages
{
    [Authorize]
    public class EditTaskModel : PageModel
    {
        [BindProperties]
        public class EditTaskFormModel
        {
            public string TaskName { get; set; }
        }

        [BindProperty(SupportsGet = true)]
        public string ProjectGuid { get; set; }
        [BindProperty(SupportsGet = true)]
        public string TaskGuid { get; set; }
        [BindProperty]
        public EditTaskFormModel Form { get; set; }
        public string UserId { get; set; }
        public List<SelectListItem> NodeUrls { get; set; }
        private readonly IActorRef _projectManager;
        private readonly Cluster _cluster;
        private BlocklyTaskNode _taskObject;

        public EditTaskModel(ProjectManagerFactory projectManager, ActorSystem system)
        {
            _projectManager = projectManager();
            _cluster = Cluster.Get(system);
        }
        public async Task<IActionResult> OnGetAsync()
        {
            ProjectInstance project = (await _projectManager.Ask<GetProjectResponseMessage>(
                new GetProjectMessage(Guid.Parse(ProjectGuid), User.FindFirstValue(ClaimTypes.NameIdentifier)))).Project;

            Guid taskGuid = Guid.Parse(TaskGuid);
            _taskObject = (BlocklyTaskNode)project.ProjectItems.First(t => t.NodeGuid.Equals(taskGuid));
            Form = new EditTaskFormModel();
            Form.TaskName = _taskObject.NodeName;
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            NodeUrls = new List<SelectListItem>();
            NodeUrls.AddRange(_cluster.State.Members
                .Where(m => m.HasRole("Task"))
                .Select(m =>
            {
                string url = m.Address.ToString();
                return new SelectListItem(url, url);
            }));
            if (NodeUrls.Count > 0)
            {
                NodeUrls[0].Selected = true;
            }
            return Page();
        }
    }
}