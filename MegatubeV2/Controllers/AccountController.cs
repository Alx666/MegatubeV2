﻿using System;
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
        public ActionResult Login(string username, string password)
        {
            //lucazodaa@gmail.com
            //asdasd
            try
            {
                string pass = password.ToMD5();
                User user = (from u in db.Users where u.EMail == username && u.Password == pass select u).Single();

                Session.SetUser(user);

                if (user.IsDeveloper || user.IsManager)
                {
                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    return RedirectToAction("Details", "Users", new { id = user.Id });
                }

            }
            catch(InvalidOperationException e)
            {
                //Piu di un utente
                return RedirectToAction("Index", "Account");
            }
            catch(ArgumentNullException e)
            {
                
                //utente non esiste oppure username/psw errati
                return RedirectToAction("Index", "Account");
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