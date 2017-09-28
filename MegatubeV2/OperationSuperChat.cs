using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public class OperationSuperChat : IOperation
    {
        private HttpPostedFileBase file;
        private MegatubeV2Entities db;
        private float dollarToEuro;
        private DateTime fileStartDate;
        private DateTime fileEndDate;

        public OperationSuperChat(HttpPostedFileBase file, float dollarToEuro, MegatubeV2Entities db, DateTime startDate, DateTime endDate)
        {
            this.file = file;
            this.db = db;
            this.dollarToEuro = dollarToEuro;
            this.fileStartDate = startDate;
            this.fileEndDate = endDate;
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}