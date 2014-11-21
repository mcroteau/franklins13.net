using franklins13.net.Models;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace franklins13.net.Common
{
    public class AuthorizationManager
    {

        protected static ApplicationDbContext db = new ApplicationDbContext();
        protected static UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

        
        public static bool IsPermitted(string PermissionString, string UserName)
        {

            var user = UserManager.FindByName(UserName);

            var query = from e in db.AccountPermissions
                        where (e.UserID == user.Id &&
                        e.Permission == PermissionString)
                        select e;

            AccountPermission accountPermission = query.FirstOrDefault();

            if (accountPermission != null)
            {
                return true;
            }

            return false;
        }

    }
}