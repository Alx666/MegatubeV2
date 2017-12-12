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
        public ActionResult Index()
        {
            var paymentAlerts = db.PaymentAlerts.ToList();
            return View(paymentAlerts);
        }

        [HttpPost]
        public ActionResult GenerateSepa(int[] ids)
        {
            PaymentAlert[] toPay = db.PaymentAlerts.Where(x => ids.Contains(x.Id)).ToArray();

            XmlSerializer serializer    = new XmlSerializer(typeof(Sepa));
            Sepa document = new Sepa("INPUT REQUIRED FOR MsgId", "Company Name Here", "NationCode", DateTime.Now, "CUC", "EMITTER IBAN", toPay, "REASON" );

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, document);

                return File(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(ms.GetBuffer())), "text/html");
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
