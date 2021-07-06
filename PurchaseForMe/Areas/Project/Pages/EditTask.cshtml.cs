using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PurchaseForMe.Core;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.Project.Nodes.Blockly;

namespace PurchaseForMe.Areas.Project.Pages
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

        private readonly IActorRef _projectManager;
        private BlocklyTaskNode _taskObject;

        public EditTaskModel(ProjectManagerFactory projectManager)
        {
            _projectManager = projectManager();
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
            return Page();
        }
    }
}