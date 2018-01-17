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

        public static IOperation Select(HttpPostedFileBase file, float dollarToEuro, MegatubeV2Entities db, int networkId)
        {
            MatchCollection matches = dateParseRegex.Matches(file.FileName);
            DateTime fileStartDate  = DateTime.ParseExact(matches[0].Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            DateTime fileEndDate    = DateTime.ParseExact(matches[1].Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            DataFile record         = db.DataFiles.Where(x => x.Name == file.FileName).FirstOrDefault();
            bool isPaidFeatures     = file.FileName.Contains("Ecommerce_paid_features_M");


            if (record != null && (ProcessingType)record.ProcessingType == ProcessingType.TrafficRevenueUpdate)
            {
                throw new Exception("File already Fully Processed");
            }
            else if (record != null && (ProcessingType)record.ProcessingType == ProcessingType.PaidFeatures)
            {
                throw new Exception("File already Fully Processed");
            }
            else if (isPaidFeatures && record == null)
            {
                return new OperationUpdatePaidFeatures(file, dollarToEuro, db, fileStartDate, fileEndDate);
            }
            else if (record == null && !isPaidFeatures)
            {
                return new OperationUpdateChannels(file, db, fileStartDate, fileEndDate, networkId);
            }
            else if (record != null && !isPaidFeatures && ((ProcessingType)record.ProcessingType) == ProcessingType.ChannelListUpdate)
            {
                return new OperationUpdateCredits(file, dollarToEuro, db, fileStartDate, fileEndDate, networkId);
            }
            else
            {
                throw new Exception("Could not deduce the correct processing model for the uploaded file");
            }

        }
    }
}