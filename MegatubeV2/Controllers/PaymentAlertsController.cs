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
    public class PaymentAlertsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        // GET: PaymentAlerts
        public ActionResult Index()
        {
            var paymentAlerts = db.PaymentAlerts.Include(p => p.User);
            return View(paymentAlerts.ToList());
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
