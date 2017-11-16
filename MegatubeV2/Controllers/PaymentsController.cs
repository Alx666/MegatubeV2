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
    public class PaymentsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();
        
        // GET: Payments
        public ActionResult Index(DateTime? date = null)
        {
            //var result = from p in db.Payments ord


            var payments = db.Payments.Include(p => p.User);
            return View(payments.ToList());
        }

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // GET: Payments/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Amount,Date,PaymentType,DateFrom,DateTo,Receipt")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Payments.Add(payment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "Name", payment.UserId);
            return View(payment);
        }


        // GET: Payments/Delete/5
        public ActionResult Revert(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Payment payment = db.Payments.Find(id);
            //if (payment == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(payment);
            throw new NotImplementedException();
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Payment payment = db.Payments.Find(id);
            //db.Payments.Remove(payment);
            //db.SaveChanges();
            //return RedirectToAction("Index");
            throw new NotImplementedException();
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
