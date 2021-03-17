using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;


#region Additional Namespaces
using System.Data.Entity;
using WebApp.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Configuration;
#endregion

namespace WebApp.Security
{
    public class SecurityDbContextInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {

        protected override void Seed(ApplicationDbContext context)
        {
            #region Seed the roles
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var startupRoles = ConfigurationManager.AppSettings["startupRoles"].Split(';');
            foreach (var role in startupRoles)
                roleManager.Create(new IdentityRole { Name = role });
            #endregion

            #region Seed the users
            //webmaster
            string adminUser = ConfigurationManager.AppSettings["adminUserName"];
            string adminRole = ConfigurationManager.AppSettings["adminRole"];
            string adminEmail = ConfigurationManager.AppSettings["adminEmail"];
            string adminPassword = ConfigurationManager.AppSettings["adminPassword"];
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var result = userManager.Create(new ApplicationUser
            {
                UserName = adminUser,
                Email = adminEmail,
                EmployeeId = null,
                CustomerId = null
            }, adminPassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(adminUser).Id, adminRole);

            //customer
            string customerUser = "HansenB";
            string customerRole = ConfigurationManager.AppSettings["customerRole"];
            string customerEmail = "bjorn.hansen@yahoo.no";
            string customerPassword = ConfigurationManager.AppSettings["newUserPassword"];
            //var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            result = userManager.Create(new ApplicationUser
            {
                UserName = customerUser,
                Email = customerEmail,
                EmployeeId = null,
                CustomerId = 4
            }, customerPassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(customerUser).Id, customerRole);

            //employee
            string employeeUser = "jPeacock";
            string employeeRole = ConfigurationManager.AppSettings["employeeRole"];
            string employeeEmail = "PeacockJ@Chinook.ca";
            string employeePassword = ConfigurationManager.AppSettings["newUserPassword"];
            //var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            result = userManager.Create(new ApplicationUser
            {
                UserName = employeeUser,
                Email = employeeEmail,
                EmployeeId = 20,
                CustomerId = null
            }, employeePassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(employeeUser).Id, employeeRole);
            #endregion

            // ... etc. ...

            base.Seed(context);
        }
    }
}