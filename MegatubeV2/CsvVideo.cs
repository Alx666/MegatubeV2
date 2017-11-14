using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public class CsvVideo : IReferenceable
    {
        public string   VideoId             { get; set; }
        public string   ChannelID           { get; set; }
        public string   Uploader            { get; set; }
        public decimal  PartnerRevenue      { get; set; }        
        public string   ContentType         { get; set; }
        public string   AssetChannelId      { get; set; }       
        public string   ChannelDisplayName  { get; set; }

        public CsvVideo()
        {

        }

        public string GetOwnerReference() => string.IsNullOrEmpty(AssetChannelId) ? AssetChannelId : ChannelID;
    }
}