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

        public static void SetUser(this HttpSessionStateBase session, User u)
        {
            session["User"] = u;                        
        }

        public static User GetUser(this HttpSessionStateBase b)
        {
            return b["User"] as User;
        }

        public static void UpdatePaymentAlerts(this MegatubeV2Entities db, int netId)
        {
            db.PaymentAlerts.RemoveRange(db.PaymentAlerts.Where(x => x.NetworkId == netId));

            var credits = (from a in db.Accreditations
                           where a.PaymentId == null && a.NetworkId == netId && a.UserId != null
                           group a by a.User into g
                           select new { User = g.Key, Gross = g.Sum(x => x.GrossAmmount) }).ToList();

            foreach (var item in credits)
            {
                if (item.User.PaymentMethod.HasValue)
                {
                    IPaymentMethod  method  = PaymentMethodFactory.GetMethodFromDBCode(item.User.PaymentMethod.Value);
                    decimal         net     = method.ComputeNet(item.Gross);

                    if (net > 100m)
                    {
                        db.PaymentAlerts.Add(new PaymentAlert(DateTime.Now, item.User.Id, item.Gross, netId));
                    }
                }
                else
                {
                    if (item.Gross > 130m)
                    {
                        db.PaymentAlerts.Add(new PaymentAlert(DateTime.Now, item.User.Id, item.Gross, netId));
                    }
                }                
            }
        }
    }
}