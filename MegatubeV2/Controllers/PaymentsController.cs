using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MegatubeV2;
using MegatubeV2.Models;

namespace MegatubeV2.Controllers
{
    public class PaymentsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        // GET: Payments
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Index(int? month, int? year)
        {
            int networkId = Session.GetUser().NetworkId;

            var payments = db.Payments.Where(x => x.NetworkId == networkId).Include(p => p.User).Include(x => x.Accreditations).OrderBy(x => x.Date);

            if (month.HasValue)
                payments.Where(x => x.Date.Month == month.Value);

            if(year.HasValue)
                payments.Where(x => x.Date.Month == month.Value);


            ViewBag.Months = new SelectList(Enum.GetValues(typeof(Month)));

            if (db.Payments.Any())
            {
                int oldYear = db.Payments.Min(x => x.Date.Year);
                int current = DateTime.Now.Year;
                var years = Enumerable.Range(oldYear, current - oldYear);
                ViewBag.Years = new SelectList(years, years.Last());
            }
            else
            {
                ViewBag.Years = new SelectList(Enumerable.Range(2018, 1));
            }
                

            return View(payments.ToList());
        }

        // GET: Payments/Details/5
        [CustomAuthorize(RoleType.Manager)]
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
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Create(int? userId)
        {
            try
            {
                User toPay          = db.Users.Find(userId);
                User admin          = db.Users.Find(toPay.FiscalAdministratorId) ?? toPay;
                int year            = DateTime.Now.Year;

                Payment mostRecent  = (from x in db.Payments
                                       where x.Date.Year == year && x.UserId == admin.Id
                                       orderby x.ReceiptCount descending
                                       select x).FirstOrDefault();

                int count           = mostRecent != null ? mostRecent.ReceiptCount + 1 : 1;

                List<Accreditation> accreditations = (from a in db.Accreditations where a.UserId == toPay.Id && !a.PaymentId.HasValue select a).ToList();

                PaymentData p       = new PaymentData();
                p.User              = toPay;
                p.Administrator     = admin;
                p.Accreditations    = accreditations;
                p.Gross             = p.Accreditations.Sum(x => x.GrossAmmount);
                p.Net               = PaymentMethodFactory.GetMethodFromDBCode(admin.PaymentMethod.Value).ComputeNet(p.Gross);
                p.From              = accreditations.Min(x => x.DateFrom);
                p.To                = accreditations.Max(x => x.DateTo);
                p.PaymentMode       = PaymentMethodFactory.GetMethodFromDBCode(admin.PaymentMethod.Value).ToString();
                p.ReceiptCount      = count;                

                return View(p);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Create(int userId, int receiptCounter)
        {
            try
            {
                User toPay          = db.Users.Find(userId);
                Payment p = toPay.CreatePayment(db, Session.GetUser().NetworkId, out PaymentAlert toRemove);

                db.Payments.Add(p);
                db.PaymentAlerts.Remove(toRemove);
                db.SaveChanges();

                return RedirectToAction("index", "PaymentAlerts");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }


        // GET: Payments/Delete/5
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Revert(int? id)
        {
            try
            {
                Payment p           = db.Payments.Find(id);

                PaymentAlert alert  = new PaymentAlert();
                alert.Gross         = p.Accreditations.Sum(x => x.GrossAmmount);
                alert.CreationDate  = p.DateFrom;
                alert.UpdateDate    = DateTime.Now;
                alert.UserId        = p.UserId;

                if (p.User.PaymentMethod != null)
                    alert.Net       = PaymentMethodFactory.GetMethodFromDBCode(p.User.PaymentMethod.Value).ComputeNet(alert.Gross);
                else
                    alert.Net       = alert.Gross;

                p.Accreditations.Clear();

                db.Payments.Remove(p);
                db.PaymentAlerts.Add(alert);
                db.SaveChanges();

                return RedirectToAction("index", "Payments");
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        // GET: Payments/Delete/5
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult DownloadReceipt(int? id)
        {
            try
            {
                Payment p = db.Payments.Find(id);
                Receipt r = p.Receipt;
                
                return File(r.Data, "application/pdf", $"{p.User.Name}{p.User.LastName}_{p.ReceiptCount}{DateTime.Now.Year}.pdf");                                
            }
            catch (Exception ex)
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
