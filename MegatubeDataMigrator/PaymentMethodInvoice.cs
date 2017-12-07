using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class PaymentMethodInvoice : IPaymentMethod
{
    public decimal ComputeNet(decimal dGross)
    {
        return dGross + (dGross * 0.22m);
    }

    public override string ToString()
    {
        return "Fattura";
    }

}