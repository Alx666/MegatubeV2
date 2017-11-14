using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MegatubeV2
{
    public class OperationUpdatePaidFeatures : IOperation
    {
        private HttpPostedFileBase file;
        private MegatubeV2Entities db;
        private DateTime fileStartDate;
        private DateTime fileEndDate;
        private decimal dollarToEuro;


        public OperationUpdatePaidFeatures(HttpPostedFileBase file, float dollarToEuro, MegatubeV2Entities db, DateTime startDate, DateTime endDate)
        {
            this.file           = file;
            this.db             = db;
            this.fileStartDate  = startDate;
            this.fileEndDate    = endDate;
            this.dollarToEuro   = (decimal)dollarToEuro;
        }

        public string ReturnActionName      => "Index";
        public string ReturnControllerName  => "ViewChannels";
        public object ReturnRouteValues     => new { referenced = false, active = true };

        public void Execute()
        {
            if (dollarToEuro == 0m)
                throw new ApplicationException("Invalid Dollar to Euro Coefficient");

            //Extract them from csv
            using (StreamReader sr = new StreamReader(file.InputStream))
            {
                //Get All Channels From Db
                List<Channel> allChannels = db.Channels.ToList();
                List<Channel> missingOwner = allChannels.Where(c => c.OwnerId == null).ToList();

                //If there are unassociated channels rise error
                //if (missingOwner.Count > 0)
                //{
                //    StringBuilder sb = new StringBuilder("Unassociated channels detected:");
                //    missingOwner.ForEach(c => sb.AppendLine(c.Id));
                //    throw new ApplicationException(sb.ToString());
                //}

                SmartParser<CsvPaidFeatures> parser = new SmartParser<CsvPaidFeatures>(sr, "Date,Purchase Type,Refund/Chargeback,Country,Channel Name,Channel ID,Retail Price (USD),Total Tax (USD),Partner Earnings Fraction,Earnings (USD)");
                parser.Map<DateTime>("Date",                        (r, v) => r.Date = v);
                parser.Map<string>("Purchase Type",                 (r, v) => r.PurchaseType = v);
                parser.Map<bool>("Refund/Chargeback",               (r, v) => r.RefundChargeback = v);
                parser.Map<string>("Channel Name",                  (r, v) => r.ChannelName = v);
                parser.Map<string>("Channel ID",                    (r, v) => r.ChannelId = v);
                parser.Map<decimal>("Retail Price (USD)",           (r, v) => r.RetailPriceUSD = v);
                parser.Map<decimal>("Total Tax (USD)",              (r, v) => r.TotalTaxUSD = v);
                parser.Map<decimal>("Partner Earnings Fraction",    (r, v) => r.PartnerEarningsFraction = v);
                parser.Map<decimal>("Earnings (USD)",               (r, v) => r.EarningsUSD = v);

                
                var accreditations = (from v in parser.ReadAllLines()
                                      group v by v.GetOwnerReference() into g
                                      join c in allChannels
                                      on g.Key equals c.Id
                                      select Accreditation.Create(g, c, dollarToEuro, fileStartDate, fileEndDate, Accreditation.AccreditationMainType.Traffic)).SelectMany(x => x).GroupBy(x => x.UserId).ToList();


                DataFile updateRecord               = new DataFile();
                updateRecord.Name                   = file.FileName;
                updateRecord.ProcessingType         = (byte)ProcessingType.PaidFeatures;
                updateRecord.TrafficIncomeTotal     = accreditations.SelectMany(x => x).Sum(x => x.GrossAmmount);
                updateRecord.TrafficIncomeNetwork   = accreditations.SelectMany(x => x).Where(x => ((Accreditation.AccreditationSubType)x.SubType) == Accreditation.AccreditationSubType.NetworkPerformance).Sum(x => x.GrossAmmount);
                updateRecord.UploadDate             = DateTime.Now;                
                updateRecord.DollarToEuroConv       = (double)dollarToEuro;

                db.DataFiles.Add(updateRecord);

                db.SaveChanges();

            }
        }
    }
}