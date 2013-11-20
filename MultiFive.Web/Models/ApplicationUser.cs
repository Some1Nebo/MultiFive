using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MultiFive.Domain;

namespace MultiFive.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        private ApplicationUser()
        {
        }

        public ApplicationUser(Player player)
        {
            Player = player;
        }

        [Required]
        public virtual Player Player { get; private set; }
    }
}