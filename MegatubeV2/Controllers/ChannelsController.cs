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
    public class ChannelsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        // GET: Channels/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Channel channel = db.Channels.Find(id);
            if (channel == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "Name", channel.OwnerId);
            ViewBag.RecruiterId = new SelectList(db.Users, "Id", "Name", channel.RecruiterId);
            return View(channel);
        }

        // POST: Channels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,RegistrationDate,OwnerId,RecruiterId,PercentOwner,PercentRecruiter,PercentMegatube")] Channel channel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(channel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "Name", channel.OwnerId);
            ViewBag.RecruiterId = new SelectList(db.Users, "Id", "Name", channel.RecruiterId);
            return View(channel);
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