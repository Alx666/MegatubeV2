using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2.Models
{
    public class PaymentData
    {
        public User User { get; internal set; }
        public User Administrator { get; internal set; }
        public List<Accreditation> Accreditations { get; internal set; }
        public object Gross { get; internal set; }

        public DateTime From { get; internal set; }
        public DateTime To { get; internal set; }
    }
}