using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PurchaseForMeWeb.Areas.Project.Pages
{
    [Authorize]
    public class ProjectsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
