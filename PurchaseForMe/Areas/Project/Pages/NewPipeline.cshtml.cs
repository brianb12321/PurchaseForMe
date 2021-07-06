using System;
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
    public class NewPipelineModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string ProjectGuid { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string PipelineName { get; set; }
        [BindProperty]
        public string PipelineCodeName { get; set; }

        private readonly IActorRef _projectManager;

        public NewPipelineModel(ProjectManagerFactory projectManager)
        {
            _projectManager = projectManager();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                GetProjectMessage message = new GetProjectMessage(Guid.Parse(ProjectGuid), userId);
                GetProjectResponseMessage response = await _projectManager.Ask<GetProjectResponseMessage>(message);

                response.Project.ProjectItems.Add(new BlocklyPipelineNode() { NodeName = PipelineName, CodeName = PipelineCodeName});
                _projectManager.Tell(new SaveProjectMessage(response.Project.ProjectGuid, userId));
                return RedirectToPage("Project", new { ProjectGuid = ProjectGuid });
            }
            else
            {
                return Page();
            }
        }
        public void OnGet()
        {
        }
    }
}
