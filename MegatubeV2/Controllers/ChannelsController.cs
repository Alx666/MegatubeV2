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
    public class ChannelsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        // GET: Channels/Edit/5
        public ActionResult Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Channel channel = db.Channels.Find(id);
                channel.PercentMegatube *= 100d;
                channel.PercentOwner *= 100d;
                channel.PercentRecruiter *= 100d;

                channel.CreditHistory = db.Accreditations.Where(x => x.ChannelId == id).ToList().Select(x => new AccreditationsPerMonth(x.DateFrom, x.GrossAmmount)).OrderByDescending(x => x.Date).ToList();


                if (channel == null)
                {
                    return HttpNotFound();
                }

                var users = db.Users.Select(t => new { Id = t.Id, Name = t.LastName + " " + t.Name });

                ViewBag.OwnerId = new SelectList(users, "Id", "Name", channel.OwnerId);
                ViewBag.RecruiterId = new SelectList(users, "Id", "Name", channel.RecruiterId);
                return View(channel);
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }

        }

        // POST: Channels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,RegistrationDate,OwnerId,RecruiterId,PercentOwner,PercentRecruiter,PercentMegatube")] Channel channel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Channel c = db.Channels.Find(channel.Id);

                    c.PercentMegatube = channel.PercentMegatube / 100d;
                    c.PercentOwner = channel.PercentOwner / 100d;
                    c.PercentRecruiter = channel.PercentRecruiter / 100d;
                    c.RecruiterId = channel.RecruiterId;
                    c.OwnerId = channel.OwnerId;

                    //db.Entry(channel).State      = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "ViewChannels");
                }

                var users = db.Users.Select(t => new { Id = t.Id, Name = t.LastName + " " + t.Name });

                ViewBag.OwnerId = new SelectList(users, "Id", "Name", channel.OwnerId);
                ViewBag.RecruiterId = new SelectList(users, "Id", "Name", channel.RecruiterId);

                return View(channel);
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
