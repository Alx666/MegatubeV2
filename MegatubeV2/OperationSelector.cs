using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MegatubeV2
{
    internal static class OperationSelector
    {
        private static Regex dateParseRegex;

        static OperationSelector()
        {
            dateParseRegex = new Regex(@"\d{8}");
        }

        public static IOperation Select(HttpPostedFileBase file, float dollarToEuro, MegatubeV2Entities db)
        {
            MatchCollection matches = dateParseRegex.Matches(file.FileName);
            DateTime fileStartDate  = DateTime.ParseExact(matches[0].Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            DateTime fileEndDate    = DateTime.ParseExact(matches[1].Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            DataFile record         = db.DataFiles.Where(x => x.Name == file.FileName).FirstOrDefault();
            bool isSuperChat        = file.FileName.Contains("Ecommerce_paid_features_M");

            if (isSuperChat && record == null)
            {
                throw new NotImplementedException();
            }
            else if (record == null && !isSuperChat)
            {
                return new OperationUpdateChannels(file, dollarToEuro, db, fileStartDate, fileEndDate);
            }
            else if (record != null && !isSuperChat)
            {
                throw new NotImplementedException("Data Upload Not Implemented");
            }
            else
            {
                throw new Exception("Me la vedo poi");
            }

        }
    }
}