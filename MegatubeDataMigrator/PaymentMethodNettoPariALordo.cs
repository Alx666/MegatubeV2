using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PaymentMethodNettoPariALordo
/// </summary>
public class PaymentMethodNettoPariALordo : IPaymentMethod
{
    public decimal ComputeNet(decimal dGross)
    {
        return dGross;
    }

    public override string ToString()
    {
        return "Netto Pari a Lordo";
    }



}