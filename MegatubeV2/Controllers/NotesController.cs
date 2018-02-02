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


        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, Order = 2)]
        [HttpPost]
        public ActionResult Dispatch(string text, int userId, int? noteId)
        {
            try
            {
                if (!noteId.HasValue)
                {
                    EventLog.Log(db, Session.GetUser(), EventLogType.NoteEdited, $"{Session.GetUser().Id} {Session.GetUser().LastName} {Session.GetUser().Name} Note Created");
                    this.Create(text, userId);
                }
                else if (noteId.HasValue)
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        EventLog.Log(db, Session.GetUser(), EventLogType.NoteEdited, $"{Session.GetUser().Id} {Session.GetUser().LastName} {Session.GetUser().Name} Note Edited");
                        this.Edit(noteId.Value, text);
                    }
                    else
                    {
                        EventLog.Log(db, Session.GetUser(), EventLogType.NoteDeleted, $"{Session.GetUser().Id} {Session.GetUser().LastName} {Session.GetUser().Name} Note Deleted");
                        this.Delete(noteId);
                    }
                }


                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }


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
