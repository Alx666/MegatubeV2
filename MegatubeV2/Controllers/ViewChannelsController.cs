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
        public ActionResult Index()
        {
            return View(db.ViewChannels.ToList());
        }

        // GET: ViewChannels/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewChannel viewChannel = db.ViewChannels.Find(id);
            if (viewChannel == null)
            {
                return HttpNotFound();
            }
            return View(viewChannel);
        }

        // GET: ViewChannels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ViewChannels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,RegistrationDate,PercentMegatube,PercentOwner,PercentRecruiter,OwnerId,OwnerName,RecruiterId,RecruiterName")] ViewChannel viewChannel)
        {
            if (ModelState.IsValid)
            {
                db.ViewChannels.Add(viewChannel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(viewChannel);
        }

        // GET: ViewChannels/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewChannel viewChannel = db.ViewChannels.Find(id);
            if (viewChannel == null)
            {
                return HttpNotFound();
            }
            return View(viewChannel);
        }

        // POST: ViewChannels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,RegistrationDate,PercentMegatube,PercentOwner,PercentRecruiter,OwnerId,OwnerName,RecruiterId,RecruiterName")] ViewChannel viewChannel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(viewChannel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(viewChannel);
        }

        // GET: ViewChannels/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewChannel viewChannel = db.ViewChannels.Find(id);
            if (viewChannel == null)
            {
                return HttpNotFound();
            }
            return View(viewChannel);
        }

        // POST: ViewChannels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ViewChannel viewChannel = db.ViewChannels.Find(id);
            db.ViewChannels.Remove(viewChannel);
            db.SaveChanges();
            return RedirectToAction("Index");
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
