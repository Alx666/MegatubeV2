//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MegatubeV2
{
    using System;
    using System.Collections.Generic;
    
    public partial class PaymentAlert
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Gross { get; set; }
        public decimal Net { get; set; }
        public System.DateTime CreationDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
    
        public virtual User User { get; set; }
    }
}