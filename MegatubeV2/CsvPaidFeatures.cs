using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public class CsvPaidFeatures : IReferenceable
    {
        public DateTime Date { get; set; }
        public string PurchaseType { get; set; }
        public bool RefundChargeback { get; set; }
        public string Country { get; set; }
        public string ChannelName { get; set; }
        public string ChannelId { get; set; }
        public decimal RetailPriceUSD { get; set; }
        public decimal TotalTaxUSD { get; set; }
        public decimal PartnerEarningsFraction { get; set; }
        public decimal EarningsUSD { get; set; }

        public string GetOwnerReference() => ChannelId.Substring(2, ChannelId.Length - 2);
    }
}