using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PurchaseForMe.Core.User;

namespace PurchaseForMe.Data
{
    public class ApplicationDbContext : IdentityDbContext<PurchaseForMeUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
