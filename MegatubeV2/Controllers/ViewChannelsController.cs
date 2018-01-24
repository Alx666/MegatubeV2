﻿using System;
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
    public class ViewChannelsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        // GET: ViewChannels
        [CustomAuthorize(RoleType.Manager)]
        public ActionResult Index(bool referenced = true, bool active = true)
        {
            ViewBag.Referenced = referenced;
            ViewBag.Active = active;
            try
            {
                int networkId = Session.GetUser().NetworkId;
                IEnumerable<ViewChannel> channels = db.ViewChannels.Where(x => x.IsActive == active && x.NetworkId == networkId);

                if (referenced)
                    channels = channels.Where(x => x.OwnerId != null);
                else
                    channels = channels.Where(x => x.OwnerId == null);

                return View(channels.OrderBy(x => x.Name));
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
