using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    [MetadataType(typeof(IUserMetaData))]
    public partial class User
    {
        public decimal TotalGrossEarning                    { get; set; }
        public decimal TotalGrossPaid                       { get; set; }
        public decimal TotalGrossToPay                      { get; set; }
        public decimal TotalNetToPay                        { get; set; }
        public List<AccreditationsPerMonth> CreditHistory   { get; set; }

        [DisplayName("First Accreditation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? FirstAccredationDate               { get; set; }

        [DisplayName("Last Payment Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? LastPaymentDate                    { get; set; }



        public bool IsDeveloper 
        {
            get 
            {
                return this.RoleId == (int)RoleType.Developer;
            }
        }

        public bool IsManager 
        {
            get 
            {
                return this.RoleId == (int)RoleType.Manager || this.RoleId == (int)RoleType.Developer;
            }
        }

        public bool IsStandard 
        {
            get 
            {
                return this.RoleId == (int)RoleType.Standard;
            }
        }

        public Payment CreatePayment(MegatubeV2Entities db, int networkId, int? receiptCount, out PaymentAlert toRemove)
        {
            User toPay                           = this;
            User admin                           = toPay.Administrator ?? toPay;

            if (receiptCount == null)
            {
                int year = DateTime.Now.Year;

                Payment mostRecent = (from x in db.Payments
                                      where x.Date.Year == year && x.UserId == admin.Id
                                      orderby x.ReceiptCount descending
                                      select x).FirstOrDefault();

                receiptCount = mostRecent != null ? mostRecent.ReceiptCount + 1 : 1;
            }

            List<Accreditation> accreditations   = (from a in db.Accreditations where a.UserId == toPay.Id && !a.PaymentId.HasValue select a).ToList();

            Payment p                            = new Payment();
            p.DateFrom                           = accreditations.Min(x => x.DateFrom);
            p.DateTo                             = accreditations.Max(x => x.DateTo);
            p.Gross                              = accreditations.Sum(x => x.GrossAmmount);
            p.Net                                = PaymentMethodFactory.GetMethodFromDBCode(admin.PaymentMethod.Value).ComputeNet(p.Gross);
            p.UserId                             = toPay.Id;
            p.PaymentType                        = (byte)admin.PaymentMethod;
            p.Date                               = DateTime.Now;
            p.NetworkId                          = networkId;
            p.ReceiptCount                       = receiptCount.Value;

            if (toPay.Administrator != null)
                p.AdministratorId                = toPay.Administrator.Id;

            accreditations.ForEach(a             => a.PaymentId = p.Id);

            db.Payments.Add(p);
            toRemove                             = (from a in db.PaymentAlerts
                                                    where a.UserId == toPay.Id
                                                    select a).SingleOrDefault();
            return p;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.CompanyName))
                return this.CompanyName;
            else
                return $"{this.LastName ?? ""} {this.Name ?? ""} - {this.PostalCode ?? ""} ";
        } 

    }

    public interface IUserMetaData
    {
        [Required]
        string Name { get; }

        [Required]
        string LastName { get; }
        
        [EmailAddress]
        string EMail { get; }

        [DisplayName("Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        DateTime? BirthDate { get; }

        [DisplayName("Registration Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        DateTime RegistrationDate { get; }
    }
}