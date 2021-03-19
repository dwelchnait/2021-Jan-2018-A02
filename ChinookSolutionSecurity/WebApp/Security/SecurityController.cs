using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

#region Additional Namespaces
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebApp.Models;
using System.Configuration;
#endregion

namespace WebApp.Security
{
    public class SecurityController
    {
        #region Constructor & Dependencies
        private readonly ApplicationUserManager UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        public SecurityController()
        {
            UserManager = HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
        }

        private void CheckResult(IdentityResult result, string item, string action)
        {
            if (!result.Succeeded)
                throw new Exception($"Failed to " + action + $" " + item +
                    $":<ul> {string.Join(string.Empty, result.Errors.Select(x => $"<li>{x}</li>"))}</ul>");
        }
        #endregion

        #region Get the customer id of security (user record) via the username
        public int? GetCurrentUserCustomerId(string username)
        {
            int? id = null;
            //get the Request object of the current user
            var request = HttpContext.Current.Request;
            if (request.IsAuthenticated) // is the current web app user logged in?
            {
                var manager = request.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
                var appUser = manager.Users.SingleOrDefault(x => x.UserName == username);
                if (appUser != null)
                {
                    id = appUser.CustomerId;
                }
            }
            return id;
        }
        #endregion
    }
}