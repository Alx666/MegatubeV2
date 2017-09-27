using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MegatubeV2.Controllers
{
    internal static class OperationSelector
    {
        public static IOperation Select(HttpPostedFileBase file, MegatubeV2Entities db)
        {
            throw new NotImplementedException();
        }
    }

    internal interface IOperation
    {
        void Execute();
    }

    internal class OperationUpdateChannels : IOperation
    {
        private HttpPostedFileBase file;
        private MegatubeV2Entities db;

        public OperationUpdateChannels(HttpPostedFileBase file, MegatubeV2Entities db)
        {
            this.file   = file;
            this.db     = db;
        }

        public void Execute()
        {
        }
    }



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
            IOperation op = OperationSelector.Select(file, db);

            op.Execute();

            //clear active status for all channels
            db.Channels.ToList().ForEach(c => c.IsActive = false);

            List<string[]> records = new List<string[]>();
            //1) check if the file exist in the database, if yes error
            using (StreamReader reader = new StreamReader(file.InputStream))
            {
                while (!reader.EndOfStream)
                {
                    records.Add(reader.ReadLine().Split(new char[] { ',' }));
                }
            }


                //2) exctract all the channels from the file
                //2.1) deactivate all the channels
                //2.2) enable and update all the others
                //2.3) add the new ones
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