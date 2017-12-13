using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public partial class Payment
    {
        public Receipt Receipt => new Receipt(this, true);
    }
}