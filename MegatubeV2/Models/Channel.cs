using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public partial class Channel
    {
        public override string ToString() => this.Name;

        public List<AccreditationsPerMonth> CreditHistory;

    }
}