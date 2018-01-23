using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MegatubeV2;
using System.IO;

namespace MegatubeV2.Controllers
{
    public class ContractsController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        // GET: Contracts
        public ActionResult Index()
        {
            try
            {
                int networkId = Session.GetUser().NetworkId;
                var contracts = db.Contracts.Include(c => c.User).Where(x => x.User.NetworkId == networkId);
                return View(contracts.ToList());
            }
            catch(Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }

        // GET: Contracts/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Contract contract = db.Contracts.Find(id);
                if (contract == null)
                {
                    return HttpNotFound();
                }

                return View(contract);
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }

        // GET: Contracts/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.UserId = new SelectList(db.Users.ToList().Select(x => new { Id = x.Id, Name = $"{x.Name} {x.LastName}" }), "Id", "Name");
                return View();

            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }            
        }

        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file, DateTime expireDate, int userId)
        {
            try
            {
                string path = Server.MapPath("~\\ArchivioContratti");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                int fileName = file.FileName.GetHashCode();
                if (System.IO.File.Exists($"{path}\\{fileName}.pdf"))
                {
                    throw new Exception("Contract already exist");
                }
                file.SaveAs($"{path}\\{fileName}.pdf");
                Contract contract = new Contract()
                {
                    ExpireDate = expireDate,
                    FilenName = fileName.ToString(),
                    UserId = userId
                };
                db.Contracts.Add(contract);
                db.SaveChanges();
                return RedirectToAction("Index", "Contracts");
            }
            catch(Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }

        // GET: Contracts/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Contract contract = db.Contracts.Find(id);

                if (contract == null)
                {
                    return HttpNotFound();
                }

                ViewBag.UserId = new SelectList(db.Users, "Id", "Name", contract.UserId);
                return View(contract);
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,FilenName,UploadDate,ExpireDate")] Contract contract)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(contract).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.UserId = new SelectList(db.Users, "Id", "Name", contract.UserId);
                return View(contract);
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }

        // GET: Contracts/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Contract contract = db.Contracts.Find(id);
                if (contract == null)
                {
                    return HttpNotFound();
                }
                return View(contract);
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                string path = Server.MapPath($"~\\ArchivioContratti\\{db.Contracts.First(x => x.Id == id).FilenName}.pdf");
                if (!System.IO.File.Exists(path))
                {
                    throw new FileNotFoundException("File not exist");
                }
                System.IO.File.Delete(path);
                Contract contract = db.Contracts.Find(id);
                db.Contracts.Remove(contract);
                db.SaveChanges();
                return RedirectToAction("Index");
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
