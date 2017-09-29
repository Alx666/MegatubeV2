using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MegatubeV2
{
    public class OperationSuperChat : IOperation
    {
        private HttpPostedFileBase  file;
        private MegatubeV2Entities  db;
        private float               dollarToEuro;
        private DateTime            fileStartDate;
        private DateTime            fileEndDate;

        public OperationSuperChat(HttpPostedFileBase file, float dollarToEuro, MegatubeV2Entities db, DateTime startDate, DateTime endDate)
        {
            this.file           = file;
            this.db             = db;
            this.dollarToEuro   = dollarToEuro;
            this.fileStartDate  = startDate;
            this.fileEndDate    = endDate;
        }

        public string ReturnActionName => throw new NotImplementedException();

        public string ReturnControllerName => throw new NotImplementedException();

        public object ReturnRouteValues => throw new NotImplementedException();

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}