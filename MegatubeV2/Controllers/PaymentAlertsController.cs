using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace MegatubeV2.Controllers
{

    public class PaymentAlertsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        // GET: PaymentAlerts

        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, Order = 2)]
        public ActionResult Index()
        {
            try
            {
                int netId = Session.GetUser().NetworkId;

                var paymentAlerts = db.PaymentAlerts.Where(x => x.NetworkId == netId).ToList();
                return View(paymentAlerts);
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
