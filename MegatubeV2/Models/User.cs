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
        public DateTime? FirstAccredationDate               { get; set; }



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

        public Payment CreatePayment(MegatubeV2Entities db, out PaymentAlert toRemove)
        {
            User toPay                           = this;
            User admin                           = toPay.Administrator ?? toPay;

            List<Accreditation> accreditations   = (from a in db.Accreditations where a.UserId == toPay.Id && !a.PaymentId.HasValue select a).ToList();

            Payment p                            = new Payment();
            p.DateFrom                           = accreditations.Min(x => x.DateFrom);
            p.DateTo                             = accreditations.Max(x => x.DateTo);
            p.Gross                              = accreditations.Sum(x => x.GrossAmmount);
            p.Net                                = PaymentMethodFactory.GetMethodFromDBCode(admin.PaymentMethod.Value).ComputeNet(p.Net);
            p.UserId                             = toPay.Id;
            p.PaymentType                        = (byte)admin.PaymentMethod;
            p.Date                               = DateTime.Now;

            if (toPay.Administrator != null)
                p.AdministratorId                = toPay.Administrator.Id;

            accreditations.ForEach(a             => a.PaymentId = p.Id);

            db.Payments.Add(p);
            toRemove                             = (from a in db.PaymentAlerts
                                                    where a.UserId == toPay.Id
                                                    select a).SingleOrDefault();
            return p;
        }


        public struct AccreditationsPerMonth
        {
            public AccreditationsPerMonth(DateTime date, decimal amount)
            {
                Date = date;
                Amount = amount;
            }

            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
        }
    }

    public interface IUserMetaData
    {
        [Required]
        string Name { get; }

        [Required]
        string LastName { get; }

        [Required]
        [EmailAddress]
        string EMail { get; }
    }
}