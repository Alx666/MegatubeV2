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
    [SessionTimeout]
    public class EventLogsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        [CustomAuthorize(RoleType.Developer)]
        public ActionResult Index()
        {
            var eventLogs = db.EventLogs.Include(e => e.User);
            return View(eventLogs.ToList());
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
