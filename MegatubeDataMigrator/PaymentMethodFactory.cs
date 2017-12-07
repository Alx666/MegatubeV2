using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public static class PaymentMethodFactory
{
    private static Dictionary<short, IPaymentMethod> m_hPaymentMethods;

    static PaymentMethodFactory()
    {
        m_hPaymentMethods = new Dictionary<short, IPaymentMethod>();

        //Attenzione non inserire il valore 0, che viene riflettuto come "nessuna modalità" sulla GUI

        m_hPaymentMethods.Add(1, new PaymentMethodCessioneDirittiUnder35());
        m_hPaymentMethods.Add(2, new PaymentMethodCessioneDirittiOver35());
        m_hPaymentMethods.Add(3, new PaymentMethodNettoPariALordo());
        m_hPaymentMethods.Add(4, new PaymentMethodInvoice());
    }

    public static IPaymentMethod GetMethodFromDBCode(short bCode)
    {
        try
        {
            return m_hPaymentMethods[bCode];
        }
        catch (Exception)
        {
            throw new ArgumentException("Richiesto Tipo Pagamento Inesistente");
        }
    }

    public static string GetNetFromGross(short bCode, decimal bGross)
    {
        try
        {
            return m_hPaymentMethods[bCode].ComputeNet(bGross).ToString("C2");
        }
        catch (Exception)
        {
            return "Missing Payment Method";
        }
    }

    public static decimal GetNetFromGrossF(short bCode, decimal bGross)
    {
        try
        {
            return m_hPaymentMethods[bCode].ComputeNet(bGross);
        }
        catch (Exception)
        {
            return -1M;
        }
    }

    public static IEnumerable<PaymentMethodInfo> Enumerate()
    {
        List<PaymentMethodInfo> hResult = new List<PaymentMethodInfo>();

        foreach (KeyValuePair<short, IPaymentMethod> hPair in m_hPaymentMethods)
        {
            hResult.Add(new PaymentMethodInfo(hPair.Key, hPair.Value));
        }

        return hResult;
    }

    public struct PaymentMethodInfo
    {
        private short m_uDbCode;
        public short DBCode { get {return m_uDbCode;} }

        private IPaymentMethod m_hInstance;
        public IPaymentMethod Instance { get { return m_hInstance; } }


        public string Description
        {
            get { return m_hInstance.ToString(); }
        }


        public PaymentMethodInfo(short uDbCode, IPaymentMethod hInstance)
        {
            m_uDbCode = uDbCode;
            m_hInstance = hInstance;
        }            
    }
}