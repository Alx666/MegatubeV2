using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PaymentMethodCessioneDirittiOver35
/// </summary>
public class PaymentMethodCessioneDirittiOver35 : IPaymentMethod
{
    public decimal ComputeNet(decimal dGross)
    {
        return dGross - (dGross * 0.75m) * 0.2m;
    }


    public override string ToString()
    {
        return "Cessione Diritti Over 35 (20% su 75% del lordo)";
    }


    
}