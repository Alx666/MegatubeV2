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
    public class NotesController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Create(string text, int userId)
        {
            Note n = new Note();
            n.AuthorId = Session.GetUser().Id;
            n.UserId = userId;
            n.Text = text;
            n.Date = DateTime.Now;
            db.Notes.Add(n);
            db.SaveChanges();
        }

        // GET: Notes/Edit/5
        public void Edit(int? id, string text)
        {
            Note note = db.Notes.Find(id);
            note.Text = text;
            note.Date = DateTime.Now;
            note.AuthorId = Session.GetUser().Id;
            db.SaveChanges();
        }

        public void Delete(int? id)
        {
            Note note = db.Notes.Find(id);

            db.Notes.Remove(note);
            db.SaveChanges();
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
