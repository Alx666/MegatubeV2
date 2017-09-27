using System;
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
            return View();
        }

        //YouTube_id0iPcHVVMh4ASTXBUYkOA_Ecommerce_paid_features_M_20170801_20170831_v1-0
        //YouTube_id0iPcHVVMh4ASTXBUYkOA_M_20170801_20170831_videoclaim_rawdata_v1-0
        public ActionResult UploadChannels(HttpPostedFileBase file, float dollarToEuro)
        {
            //Handle 3 possible scenarios
            //- Update channel informations (activity, displayname and new channel creation)
            //- Create Accreditations for channel traffic
            //- Create Accreditations for Superchat traffic
            IOperation operation = OperationSelector.Select(file, dollarToEuro, db);



            operation.Execute();

     
            return null;
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