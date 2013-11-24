using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MultiFive.Web.DataAccess;
using MultiFive.Web.Models;

namespace MultiFive.Web.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            AddTestUser(context, manager, "test1");
            AddTestUser(context, manager, "test2");
        }

        private void AddTestUser(ApplicationDbContext context, UserManager<ApplicationUser> userManager, string userName)
        {
            if (!context.Users.Any(u => u.UserName == userName))
            {
                var testPlayer = context.Players.Create();
                context.Players.Add(testPlayer);

                var user = new ApplicationUser(testPlayer)
                {
                    UserName = userName,
                };

                userManager.Create(user, "password");
            }
        }
    }
}
