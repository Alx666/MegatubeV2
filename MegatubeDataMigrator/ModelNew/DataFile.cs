//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MegatubeDataMigrator.ModelNew
{
    using System;
    using System.Collections.Generic;
    
    public partial class DataFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public System.DateTime UploadDate { get; set; }
        public byte ProcessingType { get; set; }
        public Nullable<int> UnactiveChannels { get; set; }
        public Nullable<int> NewChannels { get; set; }
        public Nullable<decimal> TrafficIncomeTotal { get; set; }
        public Nullable<decimal> TrafficIncomeNetwork { get; set; }
        public Nullable<int> TotalChannels { get; set; }
        public Nullable<double> DollarToEuroConv { get; set; }
        public int NetworkId { get; set; }
    
        public virtual Network Network { get; set; }
    }
}
