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
    
    public class EventLogsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        
        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Developer, Order = 2)]
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
