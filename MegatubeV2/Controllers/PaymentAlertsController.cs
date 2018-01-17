using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MegatubeV2;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace MegatubeV2.Controllers
{
    public class PaymentAlertsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        // GET: PaymentAlerts
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Index()
        {
            int netId = Session.GetUser().NetworkId;

            var paymentAlerts = db.PaymentAlerts.Where(x => x.NetworkId == netId).ToList();
            return View(paymentAlerts);
        }

        [HttpPost]
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult GenerateSepa(int[] ids)
        {
            PaymentAlert[] toPay = db.PaymentAlerts.Where(x => ids.Contains(x.User.Id)).ToArray();

            XmlSerializer serializer    = new XmlSerializer(typeof(Sepa));
            Sepa document = new Sepa("GROWUP", "Grow Up Network SRL", "IT", DateTime.Now, "SIAB8VPN", "IT45G0306901798100000005467", toPay, "Pagamento Traffico Growup");

            byte[] data;

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, document);
                data = new byte[ms.GetBuffer().Length];
                Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, ms.GetBuffer().Length);


                for (int i = 0; i < toPay.Length; i++)
                {
                    Payment p = toPay[i].User.CreatePayment(db, Session.GetUser().NetworkId, out PaymentAlert toRemove);
                    db.Payments.Add(p);
                    db.PaymentAlerts.Remove(toRemove);
                    db.SaveChanges();
                }

                return File(data, "application/xml", "sepa.xml");
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
