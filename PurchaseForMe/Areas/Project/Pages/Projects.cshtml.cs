using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PurchaseForMe.Areas.Project.Pages
{
    [Authorize]
    public class ProjectsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
