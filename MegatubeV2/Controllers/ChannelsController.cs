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
    
    public class ChannelsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        
        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, Order = 2)]
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

                if (channel.Accreditations.Any())
                {
                    var accr = (from a in db.Accreditations where a.ChannelId == channel.Id group a by a.DateFrom into g select new { Date = g.Key, Amount = g.Sum(x => x.GrossAmmount) }).OrderByDescending(x => x.Date).Take(12).ToList();
                    channel.CreditHistory = accr.Select(x => new AccreditationsPerMonth(x.Date, x.Amount)).ToList();
                    channel.CreditHistory.Reverse();
                }

                if (channel == null)
                {
                    return HttpNotFound();
                }

                var users = db.Users.ToList().Select(t => new { Id = t.Id, Name = t.ToString() });

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

        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, RoleType.Standard, Order = 2)]
        public ActionResult Details(string id)
        {            
            Channel channel = db.Channels.Find(id);
            User    current = Session.GetUser();
            channel.PercentMegatube     *= 100;
            channel.PercentOwner        *= 100;
            channel.PercentRecruiter    *= 100;

            if (channel.Accreditations.Any())
            {
                var accr = (from a in db.Accreditations where a.ChannelId == channel.Id group a by a.DateFrom into g select new { Date = g.Key, Amount = g.Sum(x => x.GrossAmmount) }).OrderByDescending(x => x.Date).Take(12).ToList();
                channel.CreditHistory = accr.Select(x => new AccreditationsPerMonth(x.Date, x.Amount)).ToList();
                channel.CreditHistory.Reverse();
            }

            if (current.Id == channel.OwnerId || current.Id == channel.RecruiterId || current.IsManager)
            {
                var users = db.Users.ToList().Select(t => new { Id = t.Id, Name = t.ToString() });
                ViewBag.OwnerId = new SelectList(users, "Id", "Name", channel.OwnerId);
                ViewBag.RecruiterId = new SelectList(users, "Id", "Name", channel.RecruiterId);
                return View(db.Channels.Find(id));
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadGateway);
            }
        }

        // POST: Channels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, Order = 2)]
        public ActionResult Edit([Bind(Include = "Id,Name,RegistrationDate,OwnerId,RecruiterId,PercentOwner,PercentRecruiter,PercentMegatube")] Channel channel)
        {
            try
            {
                User current = Session.GetUser();

                if (ModelState.IsValid)
                {
                    Channel c = db.Channels.Find(channel.Id);

                    c.PercentMegatube  = channel.PercentMegatube    / 100d;
                    c.PercentOwner     = channel.PercentOwner       / 100d;
                    c.PercentRecruiter = channel.PercentRecruiter   / 100d;
                    c.RecruiterId      = channel.RecruiterId;
                    c.OwnerId          = channel.OwnerId;

                    EventLog.Log(db, db.Users.Find(current.Id), EventLogType.UserChanged, $"{current.LastName} {current.Name} Changed Channel Record {c.Id} {c.Name}");
                    db.SaveChanges();
                    return RedirectToAction("Index", "ViewChannels");
                }

                var users = db.Users.ToList().Select(t => new { Id = t.Id, Name = t.ToString() });

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

        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, Order = 2)]
        [HttpGet]
        public ActionResult Balance(int? Years)
        {
            if (!Years.HasValue)
                Years = DateTime.Now.Year;

            int netId       = Session.GetUser().NetworkId;
            int minYear     = db.Accreditations.Min(x => x.DateFrom.Year);
            int currentYear = DateTime.Now.Year;
            ViewBag.Years   = new SelectList(Enumerable.Range(minYear, currentYear - minYear + 1), Years);

            int networkId = Session.GetUser().NetworkId;

            return View(db.ViewChannelBalances.Where(x => x.NetworkId == networkId && x.Year == Years.Value));
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
