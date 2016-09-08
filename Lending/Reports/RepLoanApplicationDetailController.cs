using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lending.Reports
{
    public class RepLoanApplicationDetailController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan application detail
        [Authorize]
        public ActionResult loanApplicationDetail(Int32? loanId)
        {
            if (loanId != null)
            {
                // PDF settings
                MemoryStream workStream = new MemoryStream();
                Rectangle rectangle = new Rectangle(PageSize.A3);
                Document document = new Document(rectangle, 72, 72, 72, 72);
                document.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter.GetInstance(document, workStream).CloseStream = false;

                // Document Starts
                document.Open();

                // Fonts Customization
                Font fontArial17Bold = FontFactory.GetFont("Arial", 17, Font.BOLD);
                Font fontArial12Bold = FontFactory.GetFont("Arial", 12, Font.BOLD);
                Font fontArial12 = FontFactory.GetFont("Arial", 12);
                Font fontArial12White = FontFactory.GetFont("Arial", 13, BaseColor.WHITE);

                // line
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                
                // table main header
                PdfPTable loanApplicationheader = new PdfPTable(2);
                float[] loanApplicationheaderWidthCells = new float[] { 50f, 50f };
                loanApplicationheader.SetWidths(loanApplicationheaderWidthCells);
                loanApplicationheader.WidthPercentage = 100;
                loanApplicationheader.AddCell(new PdfPCell(new Phrase("DLH Incorporated", fontArial17Bold)) { Border = 0 });
                loanApplicationheader.AddCell(new PdfPCell(new Phrase("Loan Detail Report", fontArial17Bold)) { Border = 0, HorizontalAlignment = 2 });
                loanApplicationheader.AddCell(new PdfPCell(new Phrase("Pardo", fontArial12)) { Border = 0, PaddingTop = 5f });
                loanApplicationheader.AddCell(new PdfPCell(new Phrase("Pardo Branch", fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2, });
                loanApplicationheader.AddCell(new PdfPCell(new Phrase("0932-444-1234", fontArial12)) { Border = 0, PaddingTop = 5f });
                loanApplicationheader.AddCell(new PdfPCell(new Phrase("Printed " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt"), fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2 });
                document.Add(loanApplicationheader);

                // table collection lines data
                PdfPTable loanHeaderLabel = new PdfPTable(1);
                float[] loanHeaderLabelWidthCells = new float[] { 100f };
                loanHeaderLabel.SetWidths(loanHeaderLabelWidthCells);
                loanHeaderLabel.WidthPercentage = 100;
                loanHeaderLabel.AddCell(new PdfPCell(new Phrase("Header Detail", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });

                document.Add(line);
                document.Add(loanHeaderLabel);

                // queries
                var loanApplications = from d in db.trnLoanApplications where d.Id == loanId select d;
                if (loanApplications.Any())
                {
                    // table data
                    PdfPTable loanApplicationData = new PdfPTable(4);
                    float[] loanApplicationDataWidthCells = new float[] { 15f, 45, 15f, 25f };
                    loanApplicationData.SetWidths(loanApplicationDataWidthCells);
                    loanApplicationData.WidthPercentage = 100;
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().mstApplicant.ApplicantFullName, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("Loan Number", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().LoanNumber, fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("Area", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().mstArea.Area, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("Loan Date", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().LoanDate.ToLongDateString(), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("Collector", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().mstCollector.Collector, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("Maturity Date", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().MaturityDate.ToLongDateString(), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("Branch", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().mstBranch.Branch, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("Promises", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().Promises, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("Particulars", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().Particulars, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    document.Add(loanApplicationData);

                    document.Add(Chunk.NEWLINE);

                    // table collection lines data
                    PdfPTable loanHeader2Label = new PdfPTable(1);
                    float[] loanHeaderLabel2WidthCells = new float[] { 100f };
                    loanHeader2Label.SetWidths(loanHeaderLabel2WidthCells);
                    loanHeader2Label.WidthPercentage = 100;
                    loanHeader2Label.AddCell(new PdfPCell(new Phrase("Loan, Paid and Balance (Amounts)", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });

                    document.Add(line);
                    document.Add(loanHeader2Label);

                    PdfPTable loanApplicationBalance = new PdfPTable(2);
                    float[] loanApplicationBalanceWidthCells = new float[] { 80f, 20f };
                    loanApplicationBalance.SetWidths(loanApplicationBalanceWidthCells);
                    loanApplicationBalance.WidthPercentage = 100;
                    loanApplicationBalance.AddCell(new PdfPCell(new Phrase("TOTAL LOAN AMOUNT", fontArial12Bold)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 15f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationBalance.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().LoanAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 15f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationBalance.AddCell(new PdfPCell(new Phrase("TOTAL COLLECTED / PAID AMOUNT", fontArial12Bold)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationBalance.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().PaidAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationBalance.AddCell(new PdfPCell(new Phrase("BALANCE AMOUNT", fontArial12Bold)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 30f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanApplicationBalance.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().BalanceAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 30f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    document.Add(loanApplicationBalance);

                    document.Add(Chunk.NEWLINE);

                    document.Add(line);

                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);

                    // Table for Footer
                    PdfPTable tableFooter = new PdfPTable(3);
                    tableFooter.WidthPercentage = 100;
                    float[] widthsCells2 = new float[] { 40, 20, 40 };
                    tableFooter.SetWidths(widthsCells2);
                    tableFooter.AddCell(new PdfPCell(new Phrase("Prepared by:", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0 });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0 });
                    tableFooter.AddCell(new PdfPCell(new Phrase("Verified by:", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0 });

                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 10f, PaddingBottom = 10f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 10f, PaddingBottom = 10f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 10f, PaddingBottom = 10f });

                    tableFooter.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().mstUser.FullName)) { Border = 1, HorizontalAlignment = 1, PaddingBottom = 5f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingBottom = 5f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().mstUser1.FullName)) { Border = 1, HorizontalAlignment = 1, PaddingBottom = 5f });
                    document.Add(tableFooter);
                }

                // Document End
                document.Close();

                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;

                return new FileStreamResult(workStream, "application/pdf");
            }
            else
            {
                return RedirectToAction("NotFound", "Software");
            }
        }
    }
}