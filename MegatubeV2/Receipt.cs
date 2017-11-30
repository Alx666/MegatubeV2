using System;
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
    public class Receipt
    {
        public Receipt(Payment payment)
        {
            using (Document hDocument = new Document(PageSize.A4, 50, 50, 25, 25))
            {
                using (MemoryStream hOutput = new MemoryStream())
                {
                    using (PdfWriter writer = PdfWriter.GetInstance(hDocument, hOutput))
                    {
                        //hDocument.Open();

                        //IPaymentMethod hPayMethod = PaymentMethodFactory.GetMethodFromDBCode(payment.User.PaymentMethod.Value);

                        //string sDateFrom        = payment.DateFrom.ToString(@"dd MM yyyy");
                        //string sDateTo          = payment.DateTo.ToString(@"dd MM yyyy");
                        //string sDateToday       = DateTime.Today.ToString(@"dd MM yyyy");

                        

                        //string sUserName        = !string.IsNullOrEmpty(hUser.TutorName) ? hUser.TutorName : hUser.Name;
                        //string sUserSurname     = !string.IsNullOrEmpty(hUser.TutorSurname) ? hUser.TutorSurname : hUser.Surname;
                        //string sUserFullAddress = !string.IsNullOrEmpty(hUser.TutorFullAddress) ? hUser.TutorFullAddress : hUser.Full_Address;
                        //string sUserPIVAOVAT = !string.IsNullOrEmpty(hUser.TutorPIVAORVAT) ? hUser.TutorPIVAORVAT : hUser.PIVAORVAT;
                        //string sPostalCode = !string.IsNullOrEmpty(hUser.TutorPostalCode) ? hUser.TutorPostalCode : hUser.PostalCode;

                        //float fTotalGross = float.Parse(TextTotalGross.Text);
                        //float fTotalNet = float.Parse(TextTotalNet.Text);
                        //float fTotalTax = fTotalGross - fTotalNet;
                        //string sPeriod = TextDateFrom.Text + " al " + TextDateTo.Text;


                        ////Format tables into a stringbuilder
                        //StringBuilder hSbTables = new StringBuilder();
                        //string sTable = "<table style=\"font-size: 8pt;\" border=\"0.1\" cellpadding=\"2\">" +
                        //                    "<tr bgcolor=\"#333333\">" +
                        //                        "<td style=\"font-weight:bold; color:#FFFFFF\" colspan=\"5\">[Heading]</td>" +
                        //                        "<td style=\"font-weight: bold; color: #FFFFFF\">Importo</td>" +
                        //                    "</tr>" +
                        //                    "[Rows]" +
                        //                "</table>";

                        //List<Accreditation> hAccPart    = hAccreditations.Where(x => x.Type == 0).ToList();
                        //List<Accreditation> hAccRec     = hAccreditations.Where(x => x.Type == 1).ToList();
                        //List<Accreditation> hAccExtra   = hAccreditations.Where(x => x.Type == 2).ToList();

                        //if (hAccPart.Count > 0)
                        //{
                        //    StringBuilder hSbRows = new StringBuilder();
                        //    hAccPart.ForEach(x => hSbRows.AppendFormat("<tr><td colspan=\"4\">{0}</td><td bgcolor=\"#cccccc\"></td><td>{1}</td></tr>{2}", x.Reason, x.GrossAmmount, Environment.NewLine));
                        //    string sTmpHTML = sTable.Replace("[Rows]", hSbRows.ToString()).Replace("[Heading]", string.Format("Partnership Canali da {0} a {1}", sDateFrom, sDateTo));
                        //    hSbTables.AppendLine(sTmpHTML);
                        //    hSbTables.AppendLine("<br>");
                        //    hSbTables.AppendLine("<br>");
                        //}

                        //if (hAccRec.Count > 0)
                        //{
                        //    StringBuilder hSbRows = new StringBuilder();
                        //    hAccRec.ForEach(x => hSbRows.AppendFormat("<tr><td colspan=\"4\">{0}</td><td bgcolor=\"#cccccc\"></td><td>{1}</td></tr>{2}", x.Reason, x.GrossAmmount, Environment.NewLine));
                        //    string sTmpHTML = sTable.Replace("[Rows]", hSbRows.ToString()).Replace("[Heading]", string.Format("Recruiting Canali da {0} a {1}", sDateFrom, sDateTo));
                        //    hSbTables.AppendLine(sTmpHTML);
                        //    hSbTables.AppendLine("<br>");
                        //    hSbTables.AppendLine("<br>");
                        //}

                        //if (hAccExtra.Count > 0)
                        //{
                        //    StringBuilder hSbRows = new StringBuilder();
                        //    hAccExtra.ForEach(x => hSbRows.AppendFormat("<tr><td colspan=\"4\">{0}</td><td bgcolor=\"#cccccc\"></td><td>{1}</td></tr>{2}", x.Reason, x.GrossAmmount, Environment.NewLine));
                        //    string sTmpHTML = sTable.Replace("[Rows]", hSbRows.ToString()).Replace("[Heading]", string.Format("Commissioni Extra da {0} a {1}", sDateFrom, sDateTo));
                        //    hSbTables.AppendLine(sTmpHTML);
                        //    hSbTables.AppendLine("<br>");
                        //    hSbTables.AppendLine("<br>");
                        //}

                        //bool bIsFiscal = hUser.Network.IsFiscal;



                        ////Load the form as a string
                        //string sReceiptForm = File.ReadAllText(Server.MapPath("~/ReceiptForm.htm"));
                        //sReceiptForm = sReceiptForm.Replace("[Ricevuta]", bIsFiscal ? "Ricevuta" : "Ricevuta non fiscale (solo a fini reportistici)");
                        //sReceiptForm = sReceiptForm.Replace("[User.Name]", sUserName);
                        //sReceiptForm = sReceiptForm.Replace("[Payment.Date]", sDateToday);
                        //sReceiptForm = sReceiptForm.Replace("[User.Surname]", sUserSurname);
                        //sReceiptForm = sReceiptForm.Replace("[Counter]", iReceiptId.ToString());
                        //sReceiptForm = sReceiptForm.Replace("[User.Residence]", sUserFullAddress);
                        //sReceiptForm = sReceiptForm.Replace("[User.Cf]", sUserPIVAOVAT);
                        //sReceiptForm = sReceiptForm.Replace("[ImportsRows]", hSbTables.ToString());
                        //sReceiptForm = sReceiptForm.Replace("[Periodo]", "Revenue periodo " + sPeriod);
                        //sReceiptForm = sReceiptForm.Replace("[Imponibile]", "€ " + fTotalGross.ToString("n2"));
                        //sReceiptForm = sReceiptForm.Replace("[DescFormulaPagamento]", bIsFiscal ? hPayMethod.ToString() : " - ");
                        //sReceiptForm = sReceiptForm.Replace("[Ritenuta]", "€ " + fTotalTax.ToString("n2"));
                        //sReceiptForm = sReceiptForm.Replace("[TotaleEuro]", "€ " + fTotalNet.ToString("n2"));
                        //sReceiptForm = sReceiptForm.Replace("[User.BirthPlace]", hUser.BirthPlace);
                        //sReceiptForm = sReceiptForm.Replace("[User.PostalCode]", sPostalCode);
                        //sReceiptForm = sReceiptForm.Replace("[Network.FiscalName]", "GROW UP NETWORK SRL"); //hUser.Network.FiscalName);
                        //sReceiptForm = sReceiptForm.Replace("[Street]", bIsFiscal ? "Via Varese 8 - 20121" : string.Empty);
                        //sReceiptForm = sReceiptForm.Replace("[City]", bIsFiscal ? "Milano(MI)" : string.Empty);
                        //sReceiptForm = sReceiptForm.Replace("[PIVA]", bIsFiscal ? "09970930963" : string.Empty);
                        //sReceiptForm = sReceiptForm.Replace("[Law]", bIsFiscal ? "Non imponibile Iva ex art 3 Dpr 633 / 72" : string.Empty);

                        //if (hUser.TutorBirthDate.HasValue)
                        //{
                        //    sReceiptForm = sReceiptForm.Replace("[User.BirthDate]", hUser.TutorBirthDate.Value.ToString(@"dd MM yyyy"));
                        //}
                        //else if (hUser.BirthDate.HasValue)
                        //{
                        //    sReceiptForm = sReceiptForm.Replace("[User.BirthDate]", hUser.BirthDate.Value.ToString(@"dd MM yyyy"));
                        //}
                        //else
                        //{
                        //    sReceiptForm = sReceiptForm.Replace("[User.BirthDate]", "");
                        //}



                        ////iTextSharp.text.Image hImage = iTextSharp.text.Image.GetInstance(Server.MapPath("~/images/MTReceiptTop.png"));
                        ////hImage.ScalePercent(20f);
                        ////hDocument.Add(hImage);


                        //List<IElement> hElements = HTMLWorker.ParseToList(new StringReader(sReceiptForm), null);
                        //for (int i = 0; i < hElements.Count; i++)
                        //{
                        //    hDocument.Add(hElements[i]);
                        //}

                        //hDocument.Close();

                        ////Save On Disk
                        //byte[] hDataBinary = hOutput.ToArray();
                        //string sFileName = string.Format("{0} {1} - {2}_{3}.pdf", sUserSurname, sUserName, iReceiptId, DateTime.Today.Year.ToString());
                        //sOutFileName = Server.MapPath("~/receipts/" + sFileName);
                        //File.WriteAllBytes(sOutFileName, hDataBinary);

                        ////Generate Receipt               
                        //hPayment.Receipt = sFileName;
                    }
                }
            }
        }
    }
}