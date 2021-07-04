using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PurchaseForMe.Actors.Project;
using PurchaseForMe.Core.Project;

namespace PurchaseForMe.Areas.Project.Pages
{
    [Authorize]
    public class NewProjectModel : PageModel
    {
        public class CreateResult
        {
            public bool IsSuccessful { get; set; }
            public string Message { get; set; }
        }

        private readonly IActorRef _projectManager;

        public NewProjectModel(ProjectManagerFactory projectManager)
        {
            _projectManager = projectManager();
        }
        [BindProperty]
        public string ProjectName { get; set; }
        [BindProperty]
        public string ProjectDescription { get; set; }
        [BindProperty]
        public CreateResult Result { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CreateProjectMessage message = new CreateProjectMessage(ProjectName, ProjectDescription);
            CreateProjectResultMessage result = await _projectManager.Ask<CreateProjectResultMessage>(message);
            Result = new CreateResult();
            Result.IsSuccessful = result.IsSuccessful;
            Result.Message = result.Message;
            if (result.IsSuccessful)
            {
                return RedirectToPage("Projects");
            }
            else
            {
                return Page();
            }
        }
    }
}