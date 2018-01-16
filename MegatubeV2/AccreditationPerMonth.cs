using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
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