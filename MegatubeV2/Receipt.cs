﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.IO;
using System.Text;

namespace MegatubeV2
{
    #pragma warning disable CS0612 //receipt gneration based on now deprecated class, don't want to rewrite..
    public class Receipt
    {
        public byte[] Data { get; private set; }

        public Receipt(Payment payment, bool isFiscal)
        {
            using (Document hDocument = new Document(PageSize.A4, 50, 50, 25, 25))
            {
                using (MemoryStream hOutput = new MemoryStream())
                {
                    using (PdfWriter writer = PdfWriter.GetInstance(hDocument, hOutput))
                    {
                        hDocument.Open();
                        
                        User receiver = payment.Administrator ?? payment.User;

                        IPaymentMethod hPayMethod = PaymentMethodFactory.GetMethodFromDBCode(payment.User.PaymentMethod.Value);

                        float fTotalGross = (float)payment.Gross;
                        float fTotalNet = (float)payment.Net;
                        float fTotalTax = fTotalGross - fTotalNet;
                        string timeSpan = payment.DateFrom.ToShortDateString() + " al " + payment.DateTo.ToShortDateString();


                        //Format tables into a stringbuilder
                        StringBuilder hSbTables = new StringBuilder();
                        string sTable = "<table style=\"font-size: 8pt;\" border=\"0.1\" cellpadding=\"2\">" +
                                            "<tr bgcolor=\"#333333\">" +
                                                "<td style=\"font-weight:bold; color:#FFFFFF\" colspan=\"5\">[Heading]</td>" +
                                                "<td style=\"font-weight: bold; color: #FFFFFF\">Importo</td>" +
                                            "</tr>" +
                                            "[Rows]" +
                                        "</table>";

                        List<Accreditation> ownershipCredits    = payment.Accreditations.Where(a => (Accreditation.AccreditationSubType)a.SubType == Accreditation.AccreditationSubType.Ownership).ToList();
                        List<Accreditation> recruitingCredits   = payment.Accreditations.Where(a => (Accreditation.AccreditationSubType)a.SubType == Accreditation.AccreditationSubType.Recruiting).ToList();
                        List<Accreditation> extraCredits        = payment.Accreditations.Where(a => (Accreditation.AccreditationSubType)a.SubType == Accreditation.AccreditationSubType.Manual).ToList();


                        if (ownershipCredits.Count > 0)
                        {
                            StringBuilder hSbRows = new StringBuilder();
                            ownershipCredits.ForEach(x => hSbRows.AppendFormat("<tr><td colspan=\"4\">{0}</td><td bgcolor=\"#cccccc\"></td><td>{1}</td></tr>{2}", x.ToString(), x.GrossAmmount, Environment.NewLine));
                            string sTmpHTML = sTable.Replace("[Rows]", hSbRows.ToString()).Replace("[Heading]", string.Format("Partnership Canali da {0} a {1}", payment.DateFrom.ToShortDateString(), payment.DateTo.ToShortDateString()));
                            hSbTables.AppendLine(sTmpHTML);
                            hSbTables.AppendLine("<br>");
                            hSbTables.AppendLine("<br>");
                        }

                        if (recruitingCredits.Count > 0)
                        {
                            StringBuilder hSbRows = new StringBuilder();
                            recruitingCredits.ForEach(x => hSbRows.AppendFormat("<tr><td colspan=\"4\">{0}</td><td bgcolor=\"#cccccc\"></td><td>{1}</td></tr>{2}", x.ToString(), x.GrossAmmount, Environment.NewLine));
                            string sTmpHTML = sTable.Replace("[Rows]", hSbRows.ToString()).Replace("[Heading]", string.Format("Recruiting Canali da {0} a {1}", payment.DateFrom.ToShortDateString(), payment.DateTo.ToShortDateString()));
                            hSbTables.AppendLine(sTmpHTML);
                            hSbTables.AppendLine("<br>");
                            hSbTables.AppendLine("<br>");
                        }

                        if (extraCredits.Count > 0)
                        {
                            StringBuilder hSbRows = new StringBuilder();
                            extraCredits.ForEach(x => hSbRows.AppendFormat("<tr><td colspan=\"4\">{0}</td><td bgcolor=\"#cccccc\"></td><td>{1}</td></tr>{2}", x.ToString(), x.GrossAmmount, Environment.NewLine));
                            string sTmpHTML = sTable.Replace("[Rows]", hSbRows.ToString()).Replace("[Heading]", string.Format("Commissioni Extra da {0} a {1}", payment.DateFrom.ToShortDateString(), payment.DateTo.ToShortDateString()));
                            hSbTables.AppendLine(sTmpHTML);
                            hSbTables.AppendLine("<br>");
                            hSbTables.AppendLine("<br>");
                        }

                        
                        
                        //Load the form as a string
                        string sReceiptForm = File.ReadAllText(HttpContext.Current.Server.MapPath("~/ReceiptForm.htm"));
                        sReceiptForm = sReceiptForm.Replace("[Ricevuta]", $"Ricevuta N {payment.ReceiptCount.ToString()} del {payment.Date.ToString(@"dd MM yyyy")}");
                        sReceiptForm = sReceiptForm.Replace("[User.Name]", receiver.Name);                        
                        sReceiptForm = sReceiptForm.Replace("[User.Surname]", receiver.LastName);                        
                        sReceiptForm = sReceiptForm.Replace("[User.Residence]", receiver.FullAddress);
                        sReceiptForm = sReceiptForm.Replace("[User.BirthPlace]", receiver.BirthPlace);
                        sReceiptForm = sReceiptForm.Replace("[User.BirthDate]", receiver.BirthDate.Value.ToString(@"dd MM yyyy"));
                        sReceiptForm = sReceiptForm.Replace("[User.Cf]", receiver.PIVAorVAT);
                        sReceiptForm = sReceiptForm.Replace("[User.PostalCode]", receiver.PostalCode);
                        sReceiptForm = sReceiptForm.Replace("[ImportsRows]", hSbTables.ToString());
                        sReceiptForm = sReceiptForm.Replace("[Periodo]", "Revenue periodo " + timeSpan);
                        sReceiptForm = sReceiptForm.Replace("[Imponibile]", "€ " + fTotalGross.ToString("n2"));
                        sReceiptForm = sReceiptForm.Replace("[DescFormulaPagamento]", isFiscal ? hPayMethod.ToString() : " - ");
                        sReceiptForm = sReceiptForm.Replace("[Ritenuta]", "€ " + fTotalTax.ToString("n2"));
                        sReceiptForm = sReceiptForm.Replace("[TotaleEuro]", "€ " + fTotalNet.ToString("n2"));
                        sReceiptForm = sReceiptForm.Replace("[Network.FiscalName]", "GROW UP NETWORK SRL");
                        sReceiptForm = sReceiptForm.Replace("[Street]", isFiscal ? "Via Varese 8 - 20121" : string.Empty);
                        sReceiptForm = sReceiptForm.Replace("[City]", "Milano(MI)");
                        sReceiptForm = sReceiptForm.Replace("[PIVA]", isFiscal ? "09970930963" : string.Empty);
                        sReceiptForm = sReceiptForm.Replace("[Law]", "Operazione non soggetta ad IVA ex out.3 DPR 633/72");

                        

                        if (receiver.BirthDate.HasValue)
                        {
                            sReceiptForm = sReceiptForm.Replace("[User.BirthDate]", receiver.BirthDate.Value.ToString(@"dd MM yyyy"));
                        }
                        else
                        {
                            sReceiptForm = sReceiptForm.Replace("[User.BirthDate]", "");
                        }

                        iTextSharp.text.Image hImage = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/assets/images/MTReceiptTop.png"));

                        //hImage.SetAbsolutePosition((PageSize.A4.Width - hImage.ScaledWidth) / 2, 354f);
                        ////hImage.Alignment = 
                        ////float fScale = 20f;
                        ////float fRight = 1484f;
                        ////float fTop = 354f;

                        ////hImage.Right = fRight;
                        ////hImage.Top = fTop;
                        hImage.ScalePercent(20f);
                        hDocument.Add(hImage);


                        List<IElement> hElements = HTMLWorker.ParseToList(new StringReader(sReceiptForm), null);                        


                        for (int i = 0; i < hElements.Count; i++)
                        {
                            hDocument.Add(hElements[i]);
                        }

                        hDocument.Close();

                        Data                 = hOutput.ToArray();
                    }
                }
            }
        }
    }
}