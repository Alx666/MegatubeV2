using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for IPaymentMethod
/// </summary>
public interface IPaymentMethod
{
    decimal ComputeNet(decimal dGross);

    
}