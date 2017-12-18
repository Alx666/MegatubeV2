using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        public static string ToMD5(this string s)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(s));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

        public static void SetUser(this HttpSessionStateBase b, User u)
        {
            b["User"] = u;
        }

        public static User GetUser(this HttpSessionStateBase b)
        {
            return b["User"] as User;
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
                PaymentAlert p  = new PaymentAlert();
                p.CreationDate  = DateTime.Now;
                p.UpdateDate    = DateTime.Now;
                p.UserId        = item.UserId;
                p.Gross         = item.Gross;
                p.Net           = p.Gross;

                db.PaymentAlerts.Add(p);
                db.SaveChanges();
            }
        }
    }
}