﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MegatubeV2;
using MegatubeV2.Models;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace MegatubeV2.Controllers
{
    
    public class PaymentsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        // GET: Payments
        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, Order = 2)]
        public ActionResult Index(int? Months, int? Years)
        {
            try
            {
                int networkId = Session.GetUser().NetworkId;

                IEnumerable<Payment> result;
                result = db.Payments.Where(x => x.NetworkId == networkId).Include(p => p.User).Include(x => x.Accreditations).OrderBy(x => x.Date);

                ViewBag.SelectedYear = 0;
                ViewBag.SelectedMonth = 0;

                if (Months.HasValue)
                {
                    result = result.Where(x => x.Date.Month == Months.Value);
                    ViewBag.SelectedMonth = Months;
                }

                if (Years.HasValue)
                {
                    result = result.Where(x => x.Date.Year == Years.Value);
                    ViewBag.SelectedYear = Years;
                }

                ViewBag.Months = new SelectList(Enum.GetValues(typeof(Month)));




                if (db.Payments.Any())
                {
                    int oldYear = db.Payments.Min(x => x.Date.Year);
                    int current = DateTime.Now.Year;
                    var years = Enumerable.Range(oldYear, current - oldYear + 1);
                    ViewBag.Years = new SelectList(years, years.Last());
                }
                else
                {
                    if (Years.HasValue)
                        ViewBag.Years = new SelectList(Enumerable.Range(2018, 1), Years.Value);
                    else
                        ViewBag.Years = new SelectList(Enumerable.Range(2018, 1));
                }


                return View(result.ToList());
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }

        // GET: Payments/Details/5
        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, Order = 2)]
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
        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, Order = 2)]
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
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, Order = 2)]
        public ActionResult Create(int userId, int receiptCounter)
        {
            try
            {
                User toPay          = db.Users.Find(userId);
                Payment p = toPay.CreatePayment(db, Session.GetUser().NetworkId, receiptCounter, out PaymentAlert toRemove);

                db.Payments.Add(p);
                db.PaymentAlerts.Remove(toRemove);

                EventLog.Log(db, Session.GetUser(), EventLogType.PaymentCreated, $"{Session.GetUser().Id} {Session.GetUser().LastName} {Session.GetUser().Name} Payment Created for {toPay.Id} {toPay.EMail}");
                db.SaveChanges();

                return RedirectToAction("index", "PaymentAlerts");
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }

        [HttpPost]
        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, Order = 2)]
        public ActionResult GenerateSepa(int[] ids)
        {
            try
            {
                Network current          = db.Networks.Find(Session.GetUser().NetworkId);
                PaymentAlert[] toPay     = db.PaymentAlerts.Where(x => ids.Contains(x.User.Id)).ToArray();
                XmlSerializer serializer = new XmlSerializer(typeof(Sepa));
                Sepa document            = new Sepa("GROWUP", "Grow Up Network SRL", "IT", DateTime.Now, "SIAB8VPN", "IT45G0306901798100000005467", toPay, "Pagamento Traffico Growup");

                byte[] data;

                using (MemoryStream ms = new MemoryStream()) //write on disk and send to avoid invalid characters?
                {
                    using (XmlTextWriter x = new XmlTextWriter(ms, System.Text.Encoding.UTF8))
                    {
                        serializer.Serialize(ms, document);
                        data = new byte[ms.GetBuffer().Length];
                        Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, ms.GetBuffer().Length);

                        for (int i = 0; i < toPay.Length; i++)
                        {
                            Payment p = toPay[i].User.CreatePayment(db, Session.GetUser().NetworkId, null, out PaymentAlert toRemove);
                            db.Payments.Add(p);
                            db.PaymentAlerts.Remove(toRemove);
                            db.SaveChanges();
                            EventLog.Log(db, Session.GetUser(), EventLogType.PaymentCreated, $"{Session.GetUser().Id} {Session.GetUser().LastName} {Session.GetUser().Name} Payment Created via SEPA for {p.User.EMail}");
                        }

                        string filename = $"sepa_{current.Name}_{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}.xml";
                        return File(data, "application/xml", filename);
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }



        // GET: Payments/Delete/5
        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, Order = 2)]
        public ActionResult Revert(int? id)
        {
            try
            {
                Payment p           = db.Payments.Find(id);
                PaymentAlert alert  = new PaymentAlert(DateTime.Now, p.UserId, p.Gross, p.NetworkId);

                p.Accreditations.Clear();

                db.Payments.Remove(p);
                db.PaymentAlerts.Add(alert);
                EventLog.Log(db, Session.GetUser(), EventLogType.PaymentReverted, $"{Session.GetUser().Id} {Session.GetUser().LastName} {Session.GetUser().Name} Payment Reverted for {p.User.EMail}");
                db.SaveChanges();

                return RedirectToAction("index", "Payments");
            }
            catch(Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }

        // GET: Payments/Delete/5
        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, RoleType.Standard, Order = 2)]
        public ActionResult DownloadReceipt(int? id)
        {
            try
            {
                Payment p = db.Payments.Find(id);
                Receipt r = p.Receipt;

                if (p.UserId != Session.GetUser().Id && !Session.GetUser().IsManager)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

                return File(r.Data, "application/pdf", $"{p.User.Name}{p.User.LastName}_{p.ReceiptCount}{DateTime.Now.Year}.pdf");                                
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
