//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MegatubeDataMigrator.ModelOld
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.Accreditations = new HashSet<Accreditation>();
            this.CustomOwners = new HashSet<CustomOwner>();
            this.Payments = new HashSet<Payment>();
            this.UserMontlyEarnings = new HashSet<UserMontlyEarning>();
            this.Contracts = new HashSet<Contract>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone_Number { get; set; }
        public string Email { get; set; }
        public string BirthPlace { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Password { get; set; }
        public string Skype { get; set; }
        public bool Enabled { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public string CompanyName { get; set; }
        public string CompanyKind { get; set; }
        public string Iban { get; set; }
        public string PIVAORVAT { get; set; }
        public string Full_Address { get; set; }
        public string PostalCode { get; set; }
        public string Notes { get; set; }
        public short PaymentMethod { get; set; }
        public string BICSWIFT { get; set; }
        public string TutorBirthPlace { get; set; }
        public Nullable<System.DateTime> TutorBirthDate { get; set; }
        public string TutorFullAddress { get; set; }
        public string TutorCompanyName { get; set; }
        public string TutorCompanyKind { get; set; }
        public string TutorIban { get; set; }
        public string TutorPIVAORVAT { get; set; }
        public string TutorPostalCode { get; set; }
        public string TutorBICSWIFT { get; set; }
        public string TutorName { get; set; }
        public string TutorSurname { get; set; }
        public int NetworkId { get; set; }
        public int ReceiptCounter { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Accreditation> Accreditations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomOwner> CustomOwners { get; set; }
        public virtual Manager Manager { get; set; }
        public virtual Network Network { get; set; }
        public virtual Partner Partner { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual Recruiter Recruiter { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserMontlyEarning> UserMontlyEarnings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
