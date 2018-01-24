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
    
    public partial class Accreditation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ChannelId { get; set; }
        public Nullable<int> PaymentId { get; set; }
        public decimal GrossAmmount { get; set; }
        public System.DateTime DateFrom { get; set; }
        public System.DateTime DateTo { get; set; }
        public byte Type { get; set; }
        public byte SubType { get; set; }
        public int NetworkId { get; set; }
    
        public virtual Channel Channel { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual User User { get; set; }
    }
}
