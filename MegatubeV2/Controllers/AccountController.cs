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
        [AllowAnonymous]
        public ActionResult Index()
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        //POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string username, string password)
        {
            try
            {
                return RedirectToAction("Index", "Users");
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }                                    
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