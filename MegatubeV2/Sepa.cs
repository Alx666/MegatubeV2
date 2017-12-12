using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MegatubeV2
{
    //IBAN: IT45 G030 6901 7981 0000 0005 467
    //ABI: 03069
    //CUC: SIAB8VPN
    //Come CD(al posto di OTHR) metti SUPP

    [Serializable]
    public class Sepa
    {
        public CstmrCdtTrfInitn CstmrCdtTrfInitn { get; set; }

        public Sepa(string sMsgId, string sCompanyName, string sNationCode, DateTime vGenDate, string sCuc, string sEmitterIBAN, PaymentAlert[] hPayments, string sReason)
        {
            CstmrCdtTrfInitn = new CstmrCdtTrfInitn(sMsgId, sCompanyName, sNationCode, vGenDate, sCuc, sEmitterIBAN.Trim(), hPayments, sReason);
        }

        public Sepa()
        {

        }

    }

    [Serializable]
    public class CstmrCdtTrfInitn
    {
        public CstmrCdtTrfInitn(string sMsgId, string sCompanyName, string sNationCode, DateTime vGenDate, string sCuc, string sEmitterIBAN, PaymentAlert[] hPayments, string sReason)
        {
            sMsgId = sMsgId + "-" + vGenDate.ToString("yyyymmddhhmmss");

            GrpHdr = new GrpHdr(sMsgId, sCompanyName, vGenDate, sCuc, sEmitterIBAN, hPayments);
            PmtInf = new PmtInf(sMsgId, sCompanyName, sNationCode, vGenDate, sCuc, sEmitterIBAN, hPayments, sReason);
        }

        public CstmrCdtTrfInitn()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public GrpHdr GrpHdr { get; set; }
        public PmtInf PmtInf { get; set; }
    }

    [Serializable]
    public class GrpHdr
    {
        /// <summary>
        /// 
        /// </summary>
        public string MsgId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CreDtTm { get; set; }

        /// <summary>
        /// Number of transaction in the document
        /// </summary>
        public int NbOfTxs { get; set; }

        /// <summary>
        /// Total ammount of money across all transactions
        /// </summary>
        public decimal CtrlSum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public InitgPty InitPty { get; set; }

        public GrpHdr(string sMsgId, string sCompanyName, DateTime vGenDate, string sCuc, string sEmitterIBAN, PaymentAlert[] hPayments)
        {
            MsgId = sMsgId;
            CreDtTm = vGenDate.ToString("yyyy-mm-ddThh:mm:ss");
            NbOfTxs = hPayments.Length;
            CtrlSum = hPayments.Sum(x => x.Gross);
            InitPty = new InitgPty(sCompanyName, sCuc);
        }

        public GrpHdr()
        {

        }
    }

    [Serializable]
    public class InitgPty
    {
        /// <summary>
        /// Ragione sociale
        /// </summary>
        public string Nm { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Id Id { get; set; }

        public InitgPty(string sName, string sCUC)
        {
            Nm = sName;
            Id = new Id(sCUC);
        }

        public InitgPty()
        {
            Id = new Id();
        }
    }


    [Serializable]
    public class Id
    {
        /// <summary>
        /// 
        /// </summary>
        public OrigId OrigId { get; set; }

        public Id(string sCuc)
        {
            OrigId = new OrigId(sCuc);
        }

        public Id()
        {
            OrigId = new OrigId();
        }


    }

    [Serializable]
    public class OrigId
    {
        /// <summary>
        /// 
        /// </summary>
        public Othr Othr { get; set; }

        public OrigId(string sCuc)
        {

            Othr = new Othr(sCuc, "CBI");
        }

        public OrigId()
        {
            Othr = new Othr();
        }
    }

    [Serializable]
    public class Othr
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Issr { get; set; }

        public Othr(string sCuc, string sIssr)
        {
            Id = sCuc;
            Issr = sIssr;
        }

        public Othr()
        {
        }
    }

    [Serializable]
    public class PmtInf
    {
        /// <summary>
        /// The system generates an internal code.	Maximum of 35 characters.
        /// </summary>
        public string PmtInfId { get; set; }

        /// <summary>
        /// The system assigns the hard-coded value of TRF.
        /// </summary>
        public string PmtMtd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NbOfTxs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal CtrlSum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PmtTpInf PmtTpInf { get; set; }

        /// <summary>
        /// The system uses the payment date value from F04572. KKDGJ.
        /// </summary>
        public string ReqdExctnDt { get; set; }

        /// <summary>
        /// Debtor name, The system uses the mailing name from F0116.MLNM.Maximum of 70 characters.
        /// </summary>
        public Dbtr Dbtr { get; set; }

        /// <summary>
        /// Debtor postal code, The system includes or excludes the debtor's postal code as specified in the Debtor Postal Code processing option. The system retrieves this value from ADDZ.	None
        /// </summary>
        public DbtrAcct DbtrAcct { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbtrAgt DbtrAgt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ChrgBr { get; set; }


        [XmlElement("CdtTrfTxInf", Type = typeof(CdtTrfTxInf))]
        public CdtTrfTxInf[] CdtTrfTxInf { get; set; }

        public PmtInf(string sMsgId, string sCompanyName, string sNationCode, DateTime vGenDate, string sCuc, string sEmitterIBAN, PaymentAlert[] hPayments, string sReason)
        {
            PmtInfId = sMsgId;
            PmtMtd = "TODO: system generated code";
            NbOfTxs = hPayments.Length;
            CtrlSum = hPayments.Sum(x => x.Gross);
            PmtTpInf = new PmtTpInf();
            ReqdExctnDt = vGenDate.ToString("yyyy-mm-dd");
            Dbtr = new Dbtr(sCompanyName, sNationCode);
            DbtrAcct = new DbtrAcct(sEmitterIBAN);
            DbtrAgt = new DbtrAgt(sEmitterIBAN.Substring(5, 5)); //Extract ABI from IBAN
            ChrgBr = "SLEV";
            CdtTrfTxInf = hPayments.Select((p, i) => new CdtTrfTxInf(p, i, "IT", sReason)).ToArray();
        }

        public PmtInf()
        {
            PmtTpInf = new PmtTpInf();
            Dbtr = new Dbtr();
            DbtrAcct = new DbtrAcct();
            DbtrAgt = new DbtrAgt();
            ChrgBr = "SLEV";
            CdtTrfTxInf = new CdtTrfTxInf[5];

            for (int i = 0; i < CdtTrfTxInf.Length; i++)
            {
                CdtTrfTxInf[i] = new CdtTrfTxInf();
            }
        }
    }

    [Serializable]
    public class PmtTpInf
    {
        public SvcLvl SvcLvl { get; set; }

        public PmtTpInf()
        {
            SvcLvl = new SvcLvl();
        }
    }

    [Serializable]
    public class SvcLvl
    {
        public string Cd { get; set; }

        public SvcLvl()
        {
            Cd = "SEPA";
        }
    }


    [Serializable]
    public class Dbtr
    {
        public string Nm { get; set; }
        public PstlAdr PstlAdr { get; set; }

        public Dbtr(string sNm, string sNationCode)
        {
            Nm = sNm;
            PstlAdr = new PstlAdr(sNationCode);
        }

        public Dbtr()
        {

        }
    }

    [Serializable]
    public class PstlAdr
    {
        public string Ctry { get; set; }

        public PstlAdr(string sNationCode)
        {
            Ctry = sNationCode;
        }

        public PstlAdr()
        {

        }
    }

    [Serializable]
    public class DbtrAcct
    {
        public Id_IBAN Id { get; set; }

        public DbtrAcct(string sEmitterIban)
        {
            Id = new Id_IBAN(sEmitterIban);
        }

        public DbtrAcct()
        {
            Id = new Id_IBAN();
        }
    }

    [Serializable]
    public class Id_IBAN
    {
        public string IBAN { get; set; }

        public Id_IBAN(string sIBAN)
        {
            IBAN = sIBAN;
        }

        public Id_IBAN()
        {
        }
    }

    [Serializable]
    public class DbtrAgt
    {
        public FinInstnId FinInstnId { get; set; }
        public DbtrAgt(string sAbi)
        {
            FinInstnId = new FinInstnId(sAbi);
        }

        public DbtrAgt()
        {
            FinInstnId = new FinInstnId();
        }
    }


    [Serializable]
    public class FinInstnId
    {
        public ClrSysMmbId ClrSysMmbId { get; set; }
        public FinInstnId(string sAbi)
        {
            ClrSysMmbId = new ClrSysMmbId(sAbi);
        }

        public FinInstnId()
        {
            ClrSysMmbId = new ClrSysMmbId();
        }
    }


    [Serializable]
    public class ClrSysMmbId
    {
        public string MmbId { get; set; }

        public ClrSysMmbId(string sAbi)
        {
            MmbId = sAbi;
        }

        public ClrSysMmbId()
        {

        }
    }

    [Serializable]
    public class CdtTrfTxInf
    {
        /// <summary>
        /// Instruction identification, The system generates a unique key for each payment, comprised of the bank account, supplier, payment date, and the check control number.        
        /// </summary>
        /// <example>ABC/4562/2008-09-28</example>
        public PmtId PmtId { get; set; }
        public PmtTpInf_CdtTrfTxInf PmtTpInf { get; set; }
        public Amt Amt { get; set; }
        public Cdtr CdTr { get; set; }
        public CdtrAcct CdtrAcct { get; set; }
        public RmtInf RmtInf { get; set; }

        public CdtTrfTxInf(PaymentAlert hPaymentInfo, int iProgressive, string sNationCode, string sReason)
        {
            PmtId = new PmtId(iProgressive, "TODO: sEndToEndId");
            PmtTpInf = new PmtTpInf_CdtTrfTxInf();
            Amt = new Amt(hPaymentInfo.Gross);
            CdTr = new Cdtr(hPaymentInfo.User.Name, sNationCode);
            CdtrAcct = new CdtrAcct(hPaymentInfo.User.IBAN);
            RmtInf = new RmtInf(sReason);
        }

        public CdtTrfTxInf()
        {
            PmtId = new PmtId();
            PmtTpInf = new PmtTpInf_CdtTrfTxInf();
            Amt = new Amt();
            CdTr = new Cdtr();
            CdtrAcct = new CdtrAcct();
            RmtInf = new RmtInf();
        }
    }

    [Serializable]
    public class PmtId
    {
        public int InstrId { get; set; }
        public string EndToEndId { get; set; }

        public PmtId(int iId, string sEndToEndId)
        {
            InstrId = iId;
            EndToEndId = sEndToEndId;
        }

        public PmtId()
        {

        }

    }

    [Serializable]
    public class PmtTpInf_CdtTrfTxInf
    {
        public CtgyPurp CtgyPurp { get; set; }

        public PmtTpInf_CdtTrfTxInf()
        {
            CtgyPurp = new CtgyPurp();
        }
    }

    [Serializable]
    public class CtgyPurp
    {
        public string Cd { get; set; }

        public CtgyPurp()
        {
            Cd = "OTHR";
        }
    }

    [Serializable]
    public class Amt
    {
        public InstAmt InstAmt { get; set; }

        public Amt(decimal amount)
        {
            InstAmt = new InstAmt(amount);
        }

        public Amt()
        {
            InstAmt = new InstAmt();
        }
    }

    [Serializable]
    public class InstAmt
    {
        public InstAmt()
        {
            Ccy = "EUR";
        }

        public InstAmt(decimal ammount) : this()
        {
            Ammount = ammount;
        }

        [XmlAttribute("Ccy")]
        public string Ccy { get; set; }

        [XmlText]
        public decimal Ammount { get; set; }
    }

    [Serializable]
    public class Cdtr
    {
        public string Nm { get; set; }
        public PstlAdr PstlAdr { get; set; }

        public Cdtr(string sName, string sCountry)
        {
            Nm = sName;
            PstlAdr = new PstlAdr();
        }

        public Cdtr()
        {
            PstlAdr = new PstlAdr();
        }
    }


    [Serializable]
    public class CdtrAcct
    {
        public Id_IBAN Id { get; set; }

        public CdtrAcct(string sIBAN)
        {
            Id = new Id_IBAN(sIBAN);
        }
        public CdtrAcct()
        {
            Id = new Id_IBAN();
        }
    }


    [Serializable]
    public class RmtInf
    {
        public string Ustrd { get; set; }

        public RmtInf(string sUstrd)
        {
            Ustrd = sUstrd;
        }

        public RmtInf()
        {
        }
    }
}