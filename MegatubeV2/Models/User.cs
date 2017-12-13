using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public partial class User
    {
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
    }
}