using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public static class MegatubeExtensions
    {
        public static string FormatName(this string s)
        {
            try
            {
                return s.Trim().Substring(0, Math.Min(50,s.Length));
            }
            catch (Exception)
            {
                return null;
            }            
        }


        public static void UpdatePaymentAlerts(this MegatubeV2Entities db)
        {
            db.PaymentAlerts.RemoveRange(db.PaymentAlerts);

            var credits = (from a in db.Accreditations
                           where a.PaymentId == null
                           group a by a.UserId into g
                           select new { UserId = g.Key, Gross = g.Sum(x => x.GrossAmmount) }).Where(x => x.Gross > 100).ToList();

            foreach (var item in credits)
            {
                PaymentAlert p = new PaymentAlert();
                p.CreationDate = DateTime.Now;
                p.UpdateDate = DateTime.Now;
                p.UserId = item.UserId;
                p.Gross = item.Gross;
                p.Net = p.Gross;

                db.PaymentAlerts.Add(p);
            }
        }
    }
}