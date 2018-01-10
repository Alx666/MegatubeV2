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
    
    public partial class Channel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Channel()
        {
            this.Accreditations = new HashSet<Accreditation>();
        }
    
        public string Id { get; set; }
        public string Name { get; set; }
        public System.DateTime RegistrationDate { get; set; }
        public Nullable<int> OwnerId { get; set; }
        public Nullable<int> RecruiterId { get; set; }
        public double PercentOwner { get; set; }
        public double PercentRecruiter { get; set; }
        public double PercentMegatube { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime LatestActivity { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Accreditation> Accreditations { get; set; }
        public virtual User Owner { get; set; }
        public virtual User Recruiter { get; set; }
    }
}
