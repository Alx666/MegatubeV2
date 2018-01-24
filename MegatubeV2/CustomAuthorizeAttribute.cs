using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MegatubeV2
{
    public class CustomAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private readonly RoleType[] allowedroles;
        public CustomAuthorizeAttribute(params RoleType[] roles)
        {
            this.allowedroles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //if (httpContext.Session.GetUser() == null)
            //    httpContext.Response.RedirectToRoute(new
            //    {
            //        controller = "Account",
            //        action = "Index"
            //    });
                        
                    

            try
            {
                if (httpContext.Session.GetUser().RoleId == 0)
                    return true;

                return allowedroles.Contains((RoleType)httpContext.Session.GetUser().RoleId);
                
            }
            catch (Exception)
            {
                return false;
            }
        }
        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}