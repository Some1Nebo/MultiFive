using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using MultiFive.Domain;
using MultiFive.Web.Models;
using MultiFive.Web.Models.Messaging;

namespace MultiFive.Web.DataAccess
{    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("MultiFiveDb")
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<GameSnapshot> GameSnapshots { get; set; }
    }
}