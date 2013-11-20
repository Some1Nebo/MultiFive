using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using MultiFive.Domain;

namespace MultiFive.Web.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public IDbSet<Game> Games { get; set; }
    }
}