using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public partial class PaymentAlert
    {
        public PaymentAlert(DateTime creationDate, int userId, decimal gross, int networkId)
        {
            this.CreationDate = creationDate;
            this.UserId = userId;
            this.Gross = gross;
            this.NetworkId = networkId;
        }

        public PaymentAlert()
        {

        }
    }
}