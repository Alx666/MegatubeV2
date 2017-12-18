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
    public class ViewChannelsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        // GET: ViewChannels
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Index(bool referenced = true, bool active = true)
        {
            IEnumerable<ViewChannel> channels = db.ViewChannels.Where(x => x.IsActive == active);

            if (referenced)
                channels = channels.Where(x => x.OwnerId != null);
            else
                channels = channels.Where(x => x.OwnerId == null);
                      
            return View(channels.OrderBy(x => x.Name));
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
