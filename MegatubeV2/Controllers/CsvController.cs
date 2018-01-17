﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MegatubeV2;

namespace MegatubeV2.Controllers
{
    public class CsvController : Controller
    {
        private MegatubeV2Entities db = new MegatubeV2Entities();

        public ActionResult Index()
        {
            int netId = Session.GetUser().NetworkId;
            return View(db.DataFiles.Where(x => x.NetworkId == netId).OrderByDescending(x => x.UploadDate));
        }

        public ActionResult UploadChannels(HttpPostedFileBase file, float dollarToEuro = 0f)
        {
            try
            {
                IOperation operation = OperationSelector.Select(file, dollarToEuro, db, Session.GetUser().NetworkId);

                operation.Execute();

                return RedirectToAction(operation.ReturnActionName, operation.ReturnControllerName, operation.ReturnRouteValues);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        public ActionResult UpdatePaymentAlerts()
        {
            db.UpdatePaymentAlerts(Session.GetUser().NetworkId);
            return RedirectToAction("Index", "PaymentAlerts");
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