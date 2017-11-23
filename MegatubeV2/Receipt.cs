using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.IO;

namespace MegatubeV2
{
    public class Receipt
    {
        public Receipt()
        {
            using (Document hDocument = new Document(PageSize.A4, 50, 50, 25, 25))
            {
                using (MemoryStream hOutput = new MemoryStream())
                {
                    using (PdfWriter writer = PdfWriter.GetInstance(hDocument, hOutput))
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }
    }
}