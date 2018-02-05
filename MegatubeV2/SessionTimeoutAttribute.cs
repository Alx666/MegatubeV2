using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MegatubeV2
{
    public class SessionTimeoutAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {                                    
            if (HttpContext.Current.Session["User"] == null)
            {
                //if session expired try to create another from cookie... -.-''
                try
                {
                    Cookie c = new Cookie(HttpContext.Current.Request.Cookies[Cookie.SysCookieName].Value);

                    using (MegatubeV2Entities db = new MegatubeV2Entities())
                    {
                        string username = c.Email;
                        string password = c.Password;

                        User logged = db.Users.Where(x => x.EMail == username && x.Password == password).FirstOrDefault();

                        if (logged != null)
                        {
                            HttpContext.Current.Session["User"] = logged;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
                catch (Exception)
                {
                    filterContext.Result = new RedirectResult("~/Account/Index");
                }             
            }
        }
    }
}