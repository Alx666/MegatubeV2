using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public interface IPaymentMethod
    {
        decimal ComputeNet(decimal dGross);
    }

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


        public string FormatChannelPayText(Channel c)
        {
            return string.Format($"Cessione diritto d'autore per traffico monetizzato sul canale YOUTUBE \"{c.Name}\"");
        }
    }

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


        public string FormatChannelPayText(Channel c)
        {
            return string.Format($"Cessione diritto d'autore per traffico monetizzato sul canale YOUTUBE \"{c.Name}\"");
        }
    }

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


        public string FormatChannelPayText(Channel c)
        {
            return "Not Implemented";
        }
    }


    public class PaymentMethodFattura : IPaymentMethod
    {
        public decimal ComputeNet(decimal dGross)
        {
            return dGross + (dGross * 0.22m);
        }

        public override string ToString()
        {
            return "Fattura";
        }

        public string FormatChannelPayText(Channel c)
        {
            return string.Format($"Traffico monetizzato sul canale YOUTUBE \"{c.Name}\"");
        }
    }


}