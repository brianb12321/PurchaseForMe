using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PurchaseForMe.Core;
using PurchaseForMe.Core.Project.OpenedProjectStore;
using PurchaseForMe.Core.User;

namespace PurchaseForMe.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<PurchaseForMeUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IActorRef _projectManager;

        public LogoutModel(SignInManager<PurchaseForMeUser> signInManager, ILogger<LogoutModel> logger, ProjectManagerFactory projectManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _projectManager = projectManager();
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            //We are going to abstract this logic away.
            await _signInManager.SignOutAsync();
            _projectManager.Tell(new CloseAllUserProjectsMessage((await _signInManager.UserManager.GetUserAsync(User)).Id));
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
