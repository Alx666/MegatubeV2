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
    public class ViewChannelsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        // GET: ViewChannels
        public ActionResult Index(bool referenced = true, bool active = false)
        {
            var res = (from c in db.ViewChannels
                      where ((referenced && c.OwnerId != null) || (!referenced && c.OwnerId == null)) && c.IsActive == active                      
                      select c).OrderBy(c => c.Name);
                      
            return View(res);
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
