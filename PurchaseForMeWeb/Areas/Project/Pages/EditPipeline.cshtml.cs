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

namespace PurchaseForMeWeb.Areas.Project.Pages
{
    [Authorize]
    public class EditPipelineModel : PageModel
    {
        [BindProperties]
        public class EditPipelineFormModel
        {
            public string PipelineName { get; set; }
            public string PipelineCodeName { get; set; }
        }

        [BindProperty(SupportsGet = true)]
        public string ProjectGuid { get; set; }
        [BindProperty(SupportsGet = true)]
        public string PipelineGuid { get; set; }
        [BindProperty]
        public EditPipelineFormModel Form { get; set; }

        private readonly IActorRef _projectManager;
        private BlocklyPipelineNode _pipelineObject;

        public EditPipelineModel(ProjectManagerFactory projectManager)
        {
            _projectManager = projectManager();
        }
        public async Task<IActionResult> OnGetAsync()
        {
            ProjectInstance project = (await _projectManager.Ask<GetProjectResponseMessage>(
                new GetProjectMessage(Guid.Parse(ProjectGuid), User.FindFirstValue(ClaimTypes.NameIdentifier)))).Project;

            Guid taskGuid = Guid.Parse(PipelineGuid);
            _pipelineObject = (BlocklyPipelineNode)project.ProjectItems.First(t => t.NodeGuid.Equals(taskGuid));
            Form = new EditPipelineFormModel();
            Form.PipelineName = _pipelineObject.NodeName;
            Form.PipelineCodeName = _pipelineObject.CodeName;
            return Page();
        }
    }
}