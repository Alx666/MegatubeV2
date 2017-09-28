using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2.Csv
{
    public class CsvVideo
    {
        public string VideoId { get; set; }
        public string ChannelID { get; set; }
        public string Uploader { get; set; }
        public double TotalEarnings { get; set; }
        public string CustomId { get; set; }
        public string ContentType { get; set; }
        public string AssetChannelId { get; set; }
        public string Title { get; set; }
        public string ChannelDisplayName { get; set; }

        public CsvVideo()
        {

        }
    }
}