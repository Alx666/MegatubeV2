using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MegatubeV2.Controllers
{
    public class AccountController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        // GET: /Account/Login
        public ActionResult Index()
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        //POST: /Account/Login
        [HttpPost]
        public ActionResult Login(string username, string password, string network)
        {
            try
            {
                Network net = db.Networks.Where(x => x.Name == network).Single();

                string pass = password.ToMD5();
                User user = (from u in db.Users where u.EMail == username && u.Password == pass select u).Single();

                if (user.IsDeveloper)
                {
                    user.NetworkId = net.Id; //Developers can interact with all networks
                }
                else if(user.NetworkId != net.Id)
                {
                    throw new InvalidOperationException();
                }

                Session.SetUser(user);

                string data = new Cookie(user.EMail, user.Password, user.NetworkId).ToString();
                HttpCookie cookie = new HttpCookie(Cookie.SysCookieName, data);
                cookie.Expires = new DateTime(9999, 12, 1);
                Response.SetCookie(cookie);

                EventLog.Log(db, user, EventLogType.LoginSuccess, "Login Success", true);

                if (user.IsDeveloper || user.IsManager)
                {
                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    return RedirectToAction("Details", "Users", new { id = user.Id });
                }

            }
            catch(Exception)
            {
                EventLog.Log(db, null, EventLogType.LoginFailed, $"Login Failed: \"{Request.UserHostAddress}\" on (\"{username}\",\"{password}\", \"{network}\",)", true);
                return RedirectToAction("Index", "Account");
            }     
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            Session.SetUser(null);
            return RedirectToAction("Index");
        }

        


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}