using franklins13.net.Models;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace franklins13.net.Common
{
    public class AuthorizeEntryPermission : AuthorizeAttribute
    {
        public string Permission { get; set; }

        protected ApplicationDbContext db = new ApplicationDbContext();

        public AuthorizeEntryPermission(){
        }

        public AuthorizeEntryPermission(string Permission)
        {
            this.Permission = Permission;
        }


        protected override bool AuthorizeCore(HttpContextBase context)
        {

            var isAuthorized = base.AuthorizeCore(context);
            if (!isAuthorized)
            {
                return false;
            }

            if (context.User.IsInRole("Admin"))
            {
                return true;
            }

            var EntryId = context.Request.RequestContext.RouteData.Values["Id"];
            var UserID = context.User.Identity.GetUserId();
            var FullPermission = Permission + EntryId;

            var query = from e in db.AccountPermissions
                        where (e.UserID == UserID &&
                        e.Permission == FullPermission)
                        select e;

            AccountPermission accountPermission = query.FirstOrDefault();

            if (accountPermission != null)
            {
                return true;
            }

            return false;
        }



        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (AuthorizeCore(filterContext.HttpContext))
            {
                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null);
            }
            else
            {
                var values = new { controller = "Error", action = "Index" };
                var routeDictionary = new RouteValueDictionary(values);
                var result = new RedirectToRouteResult(routeDictionary);

                filterContext.Result = result;
            }
        }


        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var values = new { controller = "Error", action = "Index" };
            var routeDictionary = new RouteValueDictionary(values);
            var result = new RedirectToRouteResult(routeDictionary);

            filterContext.Result = result;
        }

    }
}