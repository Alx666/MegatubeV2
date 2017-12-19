using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MegatubeV2;

namespace MegatubeV2.Controllers
{
    public class UsersController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        // GET: Users
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Administrator);
            return View(users.OrderBy(x => x.LastName).ToList());
        }

        // GET: Users/Details/5
        [CustomAuthorize(RoleType.Standard, RoleType.Manager)]
        public ActionResult Details(int? id)
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
            {
                user.TotalGrossEarning   = (from a in db.Accreditations where a.UserId == user.Id select a).Sum(x => x.GrossAmmount);
                user.TotalGrossPaid      = (from p in db.Payments where p.UserId == user.Id select p).Sum(x => x.Gross);
                user.TotalGrossToPay     = user.TotalGrossEarning - user.TotalGrossPaid;

                if (user.PaymentMethod.HasValue)
                    user.TotalNetToPay   = PaymentMethodFactory.GetMethodFromDBCode(user.PaymentMethod.Value).ComputeNet(user.TotalGrossToPay);
                else
                    user.TotalNetToPay   = 0;
            }
            else
            {
                user.TotalGrossEarning   = 0;
                user.TotalGrossPaid      = 0;
                user.TotalGrossToPay     = 0;
                user.TotalNetToPay       = 0;

            }

            if (user.Payments.Count() > 0)
            {
                var accr = (from p in db.Accreditations where p.UserId == user.Id group p by p.DateFrom into g select new { Date = g.Key, Amount = g.Sum(x => x.GrossAmmount) }).OrderByDescending(x => x.Date).Take(12).ToList();

                user.CreditHistory = accr.Select(x => new User.AccreditationsPerMonth(x.Date, x.Amount)).ToList();
                user.CreditHistory.Reverse();
            }

            return View(user);
        }

        // GET: Users/Create
        [CustomAuthorize(RoleType.Developer)]
        public ActionResult Create()
        {
            ViewBag.FiscalAdministratorId = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(RoleType.Developer)]
        public ActionResult Create([Bind(Include = "Id,Name,LastName,Mobile,EMail,Password,Skype,BirthDate,BirthPlace,CompanyName,CompanyKind,IBAN,PIVAorVAT,FullAddress,PostalCode,PaymentMethod,BICSWIFT,RegistrationDate,FiscalAdministratorId")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FiscalAdministratorId = new SelectList(db.Users, "Id", "Name", user.FiscalAdministratorId);
            return View(user);
        }

        // GET: Users/Edit/5
        [CustomAuthorize(RoleType.Developer)]
        public ActionResult Edit(int? id)
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
            ViewBag.FiscalAdministratorId = new SelectList(db.Users, "Id", "Name", user.FiscalAdministratorId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(RoleType.Developer)]
        public ActionResult Edit([Bind(Include = "Id,Name,LastName,Mobile,EMail,Password,Skype,BirthDate,BirthPlace,CompanyName,CompanyKind,IBAN,PIVAorVAT,FullAddress,PostalCode,PaymentMethod,BICSWIFT,RegistrationDate,FiscalAdministratorId")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FiscalAdministratorId = new SelectList(db.Users, "Id", "Name", user.FiscalAdministratorId);
            return View(user);
        }

        // GET: Users/Delete/5
        [CustomAuthorize(RoleType.Developer)]
        public ActionResult Delete(int? id)
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
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(RoleType.Developer)]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
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
