using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public class BalanceData
    {
        public BalanceData()
        {

        }

        public int OwnerId          { get; set; }
        public BalanceType Type     { get; set; }



        public enum BalanceType
        {
            User    = 0,
            Channel = 1,
        }


    }
}