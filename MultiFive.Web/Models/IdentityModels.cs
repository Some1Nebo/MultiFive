using Microsoft.AspNet.Identity.EntityFramework;

namespace MultiFive.Web.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

    }
}