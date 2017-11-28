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
        public ActionResult Index()
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
        public ActionResult Create(int? userId)
        {
            try
            {
                User toPay         = db.Users.Find(userId);
                User admin         = db.Users.Find(toPay.FiscalAdministratorId) ?? toPay;
                int year           = DateTime.Now.Year;
                Payment mostRecent = (from x in db.Payments
                                      where x.Date.Year == year && x.UserId == admin.Id
                                      orderby x.ReceiptCount descending
                                      select x).FirstOrDefault();
                int count          = mostRecent != null ? mostRecent.ReceiptCount + 1 : 1;

                List<Accreditation> accreditations = (from a in db.Accreditations where a.UserId == toPay.Id && !a.PaymentId.HasValue select a).ToList();

                PaymentData p      = new PaymentData();
                p.User             = toPay;
                p.Administrator    = admin;
                p.Accreditations   = accreditations;
                p.Gross            = p.Accreditations.Sum(x => x.GrossAmmount);
                p.Net              = PaymentMethodFactory.GetMethodFromDBCode(admin.PaymentMethod.Value).ComputeNet(p.Gross);
                p.From             = accreditations.Min(x => x.DateFrom);
                p.To               = accreditations.Max(x => x.DateTo);
                p.PaymentMode      = PaymentMethodFactory.GetMethodFromDBCode(admin.PaymentMethod.Value).ToString();
                p.ReceiptCount     = count;

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
        public ActionResult Create(int userId, int receiptCounter)
        {
            try
            {
                User toPay = db.Users.Find(userId);
                User admin = toPay.Administrator ?? toPay;

                List<Accreditation> accreditations = (from a in db.Accreditations where a.UserId == toPay.Id && !a.PaymentId.HasValue select a).ToList();
                Payment p       = new Payment();
                p.Id            = db.Payments.Max(x => x.Id) + 1;
                p.DateFrom      = accreditations.Min(x => x.DateFrom);
                p.DateTo        = accreditations.Max(x => x.DateTo);
                p.Amount        = 0;                            
                p.UserId        = toPay.Id;
                p.PaymentType   = (byte)admin.PaymentMethod;

                accreditations.ForEach(a => a.PaymentId = p.Id);

                //Receipt receipt = new Receipt(admin, accreditations);

                return RedirectToAction("index", "PaymentAlerts");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }


        // GET: Payments/Delete/5
        public ActionResult Revert(int? id)
        {
            throw new NotImplementedException();

            Payment p = db.Payments.Find(id);
            p.Accreditations.Clear();
            db.Payments.Remove(p);
            return RedirectToAction("index", "Payments");
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
