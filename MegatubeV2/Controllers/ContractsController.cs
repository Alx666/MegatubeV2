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

        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, RoleType.Standard, Order = 2)]
        public ActionResult Index(int? userId)
        {
            try
            {
                if(userId == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                User contractOwner = db.Users.Find(userId);
                
                if((Session.GetUser().IsStandard && Session.GetUser().Id != userId) || (Session.GetUser().IsManager && Session.GetUser().NetworkId != contractOwner.NetworkId))
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

                int networkId = Session.GetUser().NetworkId;

                var contracts = db.Contracts.Include(c => c.User).Where(x => x.User.NetworkId == networkId && x.UserId == userId);

                return View(contracts.ToList());
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }


        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, Order = 2)]
        public ActionResult Upload(HttpPostedFileBase file, int userId, DateTime? expireDate)
        {
            try
            {
                byte[] data = new byte[file.InputStream.Length];

                using (MemoryStream ms = new MemoryStream(data))
                {
                    file.InputStream.CopyTo(ms);                    

                    BinaryData binaryRecord = new BinaryData();
                    binaryRecord.Data = data;

                    Contract contract = new Contract();
                    contract.UserId = userId;
                    contract.UploadDate = DateTime.Now;
                    contract.FilenName = file.FileName;
                    contract.BinaryData = binaryRecord;
                    contract.ExpireDate = expireDate;

                    db.Contracts.Add(contract);
                    db.SaveChanges();

                    return Redirect(Request.UrlReferrer.ToString());
                }

            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }


        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, RoleType.Standard, Order = 2)]
        public ActionResult Download(int? contractId)
        {
            try
            {
                if (contractId == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                Contract contract = db.Contracts.Find(contractId);

                if((Session.GetUser().IsStandard && contract.UserId != this.Session.GetUser().Id) || (Session.GetUser().IsManager && contract.User.NetworkId != Session.GetUser().NetworkId))
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

                BinaryData data = db.BinaryDatas.Find(contract.DataId);

                return File(data.Data, "application/octet-stream", contract.FilenName);
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }
        }


        [SessionTimeout(Order = 1)]
        [CustomAuthorize(RoleType.Manager, Order = 2)]
        public ActionResult Delete(int? contractId)
        {
            try
            {
                Contract c = db.Contracts.Find(contractId);

                if(c == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                if(!Session.GetUser().IsManager || c.User.NetworkId != Session.GetUser().NetworkId)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

                db.Contracts.Remove(c);
                db.BinaryDatas.Remove(c.BinaryData);
                db.SaveChanges();

                return Redirect(Request.UrlReferrer.ToString());
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
