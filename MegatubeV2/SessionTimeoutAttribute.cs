using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MegatubeV2
{
    public class SessionTimeoutAttribute : AuthorizeAttribute
    {
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    //HttpContext ctx = HttpContext.Current;
        //    //if (HttpContext.Current.Session["User"] == null)
        //    //{
        //    //    filterContext.Result = new RedirectResult("~/Account/Index");
        //    //    return;
        //    //}

        //    //base.OnActionExecuting(filterContext);
        //}

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["User"] == null)
            {
                filterContext.Result = new RedirectResult("~/Account/Index");
                return;
            }
        }
    }
}