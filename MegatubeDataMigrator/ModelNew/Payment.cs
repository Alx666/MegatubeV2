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
    
    public partial class Payment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Payment()
        {
            this.Accreditations = new HashSet<Accreditation>();
        }
    
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Gross { get; set; }
        public decimal Net { get; set; }
        public System.DateTime Date { get; set; }
        public byte PaymentType { get; set; }
        public System.DateTime DateFrom { get; set; }
        public System.DateTime DateTo { get; set; }
        public int ReceiptCount { get; set; }
        public Nullable<int> AdministratorId { get; set; }
        public int NetworkId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Accreditation> Accreditations { get; set; }
        public virtual Network Network { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
