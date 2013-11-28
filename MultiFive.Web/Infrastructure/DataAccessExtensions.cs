using System.Security.Principal;
using Microsoft.AspNet.Identity;
using MultiFive.Domain;
using MultiFive.Web.DataAccess;

namespace MultiFive.Web.Infrastructure
{
    public static class DataAccessExtensions
    {
        public static Player FindPlayer(this IRepository repository, IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                string userId = user.Identity.GetUserId();
                return repository.FindPlayer(userId);
            }

            return null;
        }
    }
}