using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PurchaseForMe.Actors.Project;
using PurchaseForMe.Core.Project;

namespace PurchaseForMe.Areas.Project.Pages
{
    [Authorize]
    public class ProjectModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string ProjectGuid { get; set; }
        public ProjectInstance Project { get; private set; }
        private readonly IActorRef _projectManager;

        public ProjectModel(ProjectManagerFactory projectManager)
        {
            _projectManager = projectManager();
        }
        public async Task<IActionResult> OnGetAsync()
        {
            Guid guid = Guid.Parse(ProjectGuid);
            GetProjectResponseMessage response =
                await _projectManager.Ask<GetProjectResponseMessage>(new GetProjectMessage(guid, User.FindFirstValue(ClaimTypes.NameIdentifier)));
            if (response.Project == null)
            {
                return NotFound();
            }
            Project = response.Project;
            return Page();
        }
    }
}