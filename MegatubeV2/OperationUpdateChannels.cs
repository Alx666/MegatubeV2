using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MegatubeV2.Csv;
using System.IO;
using System.Web.Mvc;

namespace MegatubeV2
{
    internal class OperationUpdateChannels : IOperation
    {
        private HttpPostedFileBase  file;
        private MegatubeV2Entities  db;
        private DateTime            fileStartDate;
        private DateTime            fileEndDate;
        

        public OperationUpdateChannels(HttpPostedFileBase file, float dollarToEuro, MegatubeV2Entities db, DateTime startDate, DateTime endDate)
        {
            this.file           = file;
            this.db             = db;
            this.fileStartDate  = startDate;
            this.fileEndDate    = endDate;
        }

        public string ReturnActionName      => "Index";
        public string ReturnControllerName  => "ViewChannels";
        public object ReturnRouteValues     => new { referenced = false, active = true };

        public void Execute()
        {
            //Get all active channels
            IEnumerable<Channel> activeChannels = db.Channels.Where(c => c.IsActive);

            //Classify them and disable all
            var allChannels = activeChannels.ToDictionary(k => k.Id, k => k);
            allChannels.AsParallel().ForAll(x => x.Value.IsActive = false);

            //Extract them from csv
            using (StreamReader sr = new StreamReader(file.InputStream))
            {
                SmartParser<CsvVideo> parser = new SmartParser<CsvVideo>(sr, "Video ID,Content Type,Policy,Video Title,Video Duration (sec),Username,Uploader,Channel Display Name,Channel ID,Claim Type,Claim Origin,Multiple Claims?,Category,Asset ID,Asset Labels,Asset Channel ID,Custom ID,Owned Views,Owned Views : Watch Page,Owned Views : Embedded Player,Owned Views : Channel Page,Owned Views : Live,Owned Views : On Demand,Owned Views : Ad-Enabled,Owned Views : Ad-Requested,YouTube Revenue Split : AdSense Served YouTube Sold,YouTube Revenue Split : DoubleClick Served YouTube Sold,YouTube Revenue Split : DoubleClick Served Partner Sold,YouTube Revenue Split : Partner Served Partner Sold,YouTube Revenue Split,Partner Revenue : AdSense Served YouTube Sold,Partner Revenue : DoubleClick Served YouTube Sold,Partner Revenue : DoubleClick Served Partner Sold,Partner Revenue : Partner Served Partner Sold,Partner Revenue,Estimated RPM");

                parser.Map<string>("Video ID",              (r, v) => r.VideoId = v);
                parser.Map<string>("Channel ID",            (r, v) => r.ChannelID = v);
                parser.Map<string>("Asset Channel ID",      (r, v) => r.AssetChannelId = v);
                parser.Map<string>("Channel Display Name",  (r, v) => r.ChannelDisplayName = v);
                parser.Map<string>("Content Type",          (r, v) => r.ContentType = v);
                parser.Map<string>("Uploader",              (r, v) => r.Uploader = v);

                //get all channels from CSV
                var csvChannels =  (from c in parser.ReadAllLines()
                                    where c.ContentType == "Partner-provided"
                                    group c by c.ChannelID into g
                                    select g).ToList();

                List<Channel> newChannels = new List<Channel>();

                foreach (var csvChannel in csvChannels)
                {
                    if (allChannels.ContainsKey(csvChannel.Key))
                    {
                        //Channel already present so update it
                        Channel oldOne = allChannels[csvChannel.Key];
                        oldOne.IsActive = true;
                        oldOne.LatestActivity = DateTime.Now;

                        try
                        {
                            oldOne.Name = csvChannel.FirstOrDefault().ChannelDisplayName.FormatName();
                        }
                        catch (Exception)
                        {
                            oldOne.Name = null;
                        }
                    }
                    else
                    {
                        Channel newOne = new Channel();
                        newOne.Id = csvChannel.Key;
                        newOne.IsActive = true;
                        newOne.RegistrationDate = DateTime.Now;
                        newOne.LatestActivity = DateTime.Now;

                        try
                        {
                            newOne.Name = csvChannel.FirstOrDefault().ChannelDisplayName.FormatName();
                        }
                        catch (Exception)
                        {
                            newOne.Name = null;
                        }

                        newChannels.Add(newOne);
                    }
                }

                db.DataFiles.Add(new DataFile(file.FileName, DateTime.Now, 1));
                db.Channels.AddRange(newChannels);
                db.SaveChanges();
            }
            
        }
    }
}