using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Text;

namespace MegatubeV2
{
    internal class OperationUpdateCredits : IOperation
    {
        private HttpPostedFileBase file;
        private MegatubeV2Entities db;
        private DateTime fileStartDate;
        private DateTime fileEndDate;
        private decimal dollarToEuro;


        public OperationUpdateCredits(HttpPostedFileBase file, float dollarToEuro, MegatubeV2Entities db, DateTime startDate, DateTime endDate)
        {
            this.file = file;
            this.db = db;
            this.fileStartDate = startDate;
            this.fileEndDate = endDate;
            this.dollarToEuro = (decimal)dollarToEuro;
        }

        public string ReturnActionName => "Index";
        public string ReturnControllerName => "ViewChannels";
        public object ReturnRouteValues => new { referenced = false, active = true };

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
                if (missingOwner.Count > 0)
                {
                    StringBuilder sb = new StringBuilder("Unassociated channels detected:");
                    missingOwner.ForEach(c => sb.AppendLine(c.Id));
                    throw new ApplicationException(sb.ToString());
                }

                //Let's start with the parsing
                SmartParser<CsvVideo> parser = new SmartParser<CsvVideo>(sr, "Video ID,Content Type,Policy,Video Title,Video Duration (sec),Username,Uploader,Channel Display Name,Channel ID,Claim Type,Claim Origin,Multiple Claims?,Category,Asset ID,Asset Labels,Asset Channel ID,Custom ID,Owned Views,Owned Views : Watch Page,Owned Views : Embedded Player,Owned Views : Channel Page,Owned Views : Live,Owned Views : On Demand,Owned Views : Ad-Enabled,Owned Views : Ad-Requested,YouTube Revenue Split : AdSense Served YouTube Sold,YouTube Revenue Split : DoubleClick Served YouTube Sold,YouTube Revenue Split : DoubleClick Served Partner Sold,YouTube Revenue Split : Partner Served Partner Sold,YouTube Revenue Split,Partner Revenue : AdSense Served YouTube Sold,Partner Revenue : DoubleClick Served YouTube Sold,Partner Revenue : DoubleClick Served Partner Sold,Partner Revenue : Partner Served Partner Sold,Partner Revenue,Estimated RPM");

                parser.Map<string>("Video ID",          (r, v) => r.VideoId = v);
                parser.Map<string>("Channel ID",        (r, v) => r.ChannelID = v);
                parser.Map<string>("Asset Channel ID",  (r, v) => r.AssetChannelId = v);
                parser.Map<decimal>("Partner Revenue",  (r, v) => r.PartnerRevenue = v);
                parser.Map<string>("Content Type",      (r, v) => r.ContentType = v);

                //- Get All records from csv
                //- Get reference to the owner
                //- check intersect with current DB data
                //- Allocate Accreditations for each Owner, Recruiter and Megatube
                //- 1 statement => Godlike :D
                var accreditations = (from v in parser.ReadAllLines()
                                      group v by v.GetOwnerReference() into g
                                      join c in allChannels
                                      on g.Key equals c.Id
                                      select Accreditation.Create(g, c, dollarToEuro, fileStartDate, fileEndDate, Accreditation.AccreditationMainType.Traffic)).SelectMany(x => x).GroupBy(x => x.UserId).ToList();
                

                
            }

        }
    }
}