using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class PaymentMethodCessioneDirittiUnder35 : IPaymentMethod
{
    public decimal ComputeNet(decimal dGross)
    {
        return dGross - (dGross * 0.6m) * 0.2m; 
    }


    public override string ToString()
    {
        return "Cessione Diritti Under 35 (20% su 60% del lordo)";
    }



}