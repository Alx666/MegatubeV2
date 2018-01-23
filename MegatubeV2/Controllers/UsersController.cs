﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MegatubeV2;
using System.Data.Entity.Core.Objects;

namespace MegatubeV2.Controllers
{
    public class UsersController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();
        

        // GET: Users
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Index()
        {
            try
            {
                int netid = Session.GetUser().NetworkId;
                var users = db.Users.Where(x => x.NetworkId == netid);
                return View(users.OrderBy(x => x.LastName).ToList());
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }

        // GET: Users/Details/5
        [CustomAuthorize(RoleType.Standard, RoleType.Manager)]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                User current = Session.GetUser();

                if (current.IsStandard && current.Id != id.Value)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

                User user = db.Users.Find(id);

                if (user == null)
                {
                    return HttpNotFound();
                }

                ViewBag.Notes = db.ViewNotes.Where(x => x.Subject == user.Id).ToList();

                if (user.Accreditations.Count() > 0)
                    user.TotalGrossEarning = (from a in db.Accreditations where a.UserId == user.Id select a).Sum(x => x.GrossAmmount);
                else
                    user.TotalGrossEarning = 0;

                if (user.Payments.Count() > 0)
                    user.TotalGrossPaid = (from p in user.Payments select p).Sum(x => x.Gross);
                else
                    user.TotalGrossPaid = 0;

                if (user.Accreditations.Where(x => x.PaymentId == null).Count() > 0)
                    user.TotalGrossToPay = user.Accreditations.Where(x => x.PaymentId == null).Sum(x => x.GrossAmmount);
                else
                    user.TotalGrossToPay = 0;

                if (user.PaymentMethod.HasValue)
                    user.TotalNetToPay = PaymentMethodFactory.GetMethodFromDBCode(user.PaymentMethod.Value).ComputeNet(user.TotalGrossToPay);
                else
                    user.TotalNetToPay = 0;


                if (user.Accreditations.Count() > 0)
                {
                    var accr = (from p in db.Accreditations where p.UserId == user.Id group p by p.DateFrom into g select new { Date = g.Key, Amount = g.Sum(x => x.GrossAmmount) }).OrderByDescending(x => x.Date).Take(12).ToList();
                    user.CreditHistory = accr.Select(x => new AccreditationsPerMonth(x.Date, x.Amount)).ToList();
                    user.CreditHistory.Reverse();
                }

                if (user.Accreditations.Any())
                    user.FirstAccredationDate = user.Accreditations.Min(x => x.DateFrom);
                else
                    user.FirstAccredationDate = null;

                if (user.Payments.Any())
                    user.LastPaymentDate = db.Payments.Where(x => x.UserId == user.Id).Max(x => x.Date);
                else
                    user.LastPaymentDate = null;

                return View(user);
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
   
        }

        // GET: Users/Create
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Create()
        {
            try
            {
                var userSelection = db.Users.Select(x => new { Id = x.Id, Name = x.LastName + " " + x.Name }).OrderBy(x => x.Name);
                ViewBag.RoleId = new SelectList(db.Roles.Where(x => x.Id > 0), "Id", "Name", 2);
                ViewBag.FiscalAdministratorId = new SelectList(userSelection, "Id", "Name");
                return View();
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Create([Bind(Include = "Name,LastName,Mobile,EMail,Password,Skype,BirthDate,BirthPlace,CompanyName,CompanyKind,IBAN,PIVAorVAT,FullAddress,PostalCode,PaymentMethod,BICSWIFT,FiscalAdministratorId,RoleId")] User user)
        {
            try
            {
                //if (Session.GetUser().RoleId > )
                //    throw new Exception("Requested priviledges are greater than loggred user");

                //Look for duplicated email
                if(!string.IsNullOrEmpty(user.EMail))
                    user.EMail = user.EMail.ToLower().Trim();

                User withDuplicatedEmail = db.Users.Where(x => x.EMail == user.EMail).FirstOrDefault();
                if (withDuplicatedEmail != null)
                    throw new Exception("A user with the same email already exist");

                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(user.Password))
                        user.Password = user.Password.ToMD5();

                    user.RegistrationDate = DateTime.Now;
                    user.NetworkId = Session.GetUser().NetworkId;
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                var userSelection = db.Users.Select(x => new { Id = x.Id, Name = x.LastName + " " + x.Name }).OrderBy(x => x.Name);

                ViewBag.RoleId = new SelectList(db.Roles.Where(x => x.Id > 0), "Id", "Name", user.RoleId);
                ViewBag.FiscalAdministratorId = new SelectList(userSelection, "Id", "Name", user.FiscalAdministratorId);
                return View(user);
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }

        }

        // GET: Users/Edit/5
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                User user = db.Users.Find(id);
                user.Password = null;

                if (user == null)
                {
                    return HttpNotFound();
                }

                var userSelection = db.Users.Select(x => new { Id = x.Id, Name = x.LastName + " " + x.Name }).OrderBy(x => x.Name);

                ViewBag.RoleId = new SelectList(db.Roles.Where(x => x.Id > 0), "Id", "Name", user.RoleId);
                ViewBag.FiscalAdministratorId = new SelectList(userSelection, "Id", "Name", user.FiscalAdministratorId);
                return View(user);
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }

        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Edit([Bind(Include = "Id,Name,LastName,Mobile,EMail,Password,Skype,BirthDate,BirthPlace,CompanyName,CompanyKind,IBAN,PIVAorVAT,FullAddress,PostalCode,PaymentMethod,BICSWIFT,FiscalAdministratorId,RoleId")] User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(user.Password))
                    {
                        user.Password = user.Password.ToMD5();
                    }

                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                var userSelection = db.Users.Select(x => new { Id = x.Id, Name = x.LastName + " " + x.Name }).OrderBy(x => x.Name);

                ViewBag.RoleId = new SelectList(db.Roles.Where(x => x.Id > 0), "Id", "Name", user.RoleId);
                ViewBag.FiscalAdministratorId = new SelectList(userSelection, "Id", "Name", user.FiscalAdministratorId);
                return View(user);
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }

        }

        // GET: Users/Delete/5
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                User user = db.Users.Find(id);

                if (user == null)
                {
                    return HttpNotFound();
                }

                db.Users.Remove(user);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
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
