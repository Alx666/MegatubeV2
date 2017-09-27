using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    internal class OperationUpdateChannels : IOperation
    {
        private HttpPostedFileBase file;
        private MegatubeV2Entities db;
        private DateTime fileStartDate;
        private DateTime fileEndDate;

        public OperationUpdateChannels(HttpPostedFileBase file, float dollarToEuro, MegatubeV2Entities db, DateTime startDate, DateTime endDate)
        {
            this.file           = file;
            this.db             = db;
            this.fileStartDate  = startDate;
            this.fileEndDate    = endDate;
        }

        public void Execute()
        {
        }
    }
}