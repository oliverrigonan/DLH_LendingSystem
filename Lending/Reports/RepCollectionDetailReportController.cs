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
    public class RepCollectionDetailReportController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // collection detail report
        public ActionResult collectionDetailReport(String startDate, String endDate)
        {
             if (startDate != null && endDate != null)
             {
                 var collectionLines = from d in db.trnCollectionLines.OrderBy(d => d.trnCollection.CollectionDate)
                                       where d.trnCollection.CollectionDate >= Convert.ToDateTime(startDate)
                                       && d.trnCollection.CollectionDate <= Convert.ToDateTime(endDate)
                                       && d.trnCollection.IsLocked == true
                                       select new Models.TrnCollectionLines
                                       {
                                           Id = d.Id,
                                           CollectionId = d.CollectionId,
                                           CollectionDate = d.trnCollection.CollectionDate.ToShortDateString(),
                                           CollectionNumber = d.trnCollection.CollectionNumber,
                                           AccountId = d.AccountId,
                                           Account = d.mstAccount.Account,
                                           Applicant = d.trnCollection.mstApplicant.ApplicantLastName + " " + d.trnCollection.mstApplicant.ApplicantFirstName + ", " + d.trnCollection.mstApplicant.ApplicantMiddleName,
                                           LoanId = d.LoanId,
                                           LoanNumber = d.trnLoanApplication.LoanNumber,
                                           LoanDate = d.trnLoanApplication.LoanDate.ToShortDateString(),
                                           Particulars = d.Particulars,
                                           Amount = d.Amount,
                                           CollectedByCollectorId = d.CollectedByCollectorId,
                                           CollectedByCollector = d.mstCollector.Collector
                                       };

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
                 PdfPTable collectionDetailReportheader = new PdfPTable(2);
                 float[] collectionDetailReportheaderWidthCells = new float[] { 50f, 50f };
                 collectionDetailReportheader.SetWidths(collectionDetailReportheaderWidthCells);
                 collectionDetailReportheader.WidthPercentage = 100;
                 collectionDetailReportheader.AddCell(new PdfPCell(new Phrase("DLH Incorporated", fontArial17Bold)) { Border = 0 });
                 collectionDetailReportheader.AddCell(new PdfPCell(new Phrase("Collection Detail Report", fontArial17Bold)) { Border = 0, HorizontalAlignment = 2 });
                 collectionDetailReportheader.AddCell(new PdfPCell(new Phrase("Pardo", fontArial12)) { Border = 0, PaddingTop = 5f });
                 collectionDetailReportheader.AddCell(new PdfPCell(new Phrase("Date from " + startDate + " to " + endDate, fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2, });
                 collectionDetailReportheader.AddCell(new PdfPCell(new Phrase("0932-444-1234", fontArial12)) { Border = 0, PaddingTop = 5f });
                 collectionDetailReportheader.AddCell(new PdfPCell(new Phrase("Printed " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt"), fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2 });
                 document.Add(collectionDetailReportheader);

                 // table collection data
                 PdfPTable collectionHeaderLabel = new PdfPTable(1);
                 float[] collectionHeaderLabelWidthCells = new float[] { 100f };
                 collectionHeaderLabel.SetWidths(collectionHeaderLabelWidthCells);
                 collectionHeaderLabel.WidthPercentage = 100;
                 collectionHeaderLabel.AddCell(new PdfPCell(new Phrase("List of Collection Detail Report", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });

                 document.Add(line);
                 document.Add(collectionHeaderLabel);

                 // table collection summary data
                 PdfPTable collectionDetailReportData = new PdfPTable(6);
                 float[] collectionDetailReportDataWidthCells = new float[] { 15f, 15f, 30f, 10f, 15f, 15f };
                 collectionDetailReportData.SetWidths(collectionDetailReportDataWidthCells);
                 collectionDetailReportData.WidthPercentage = 100;
                 collectionDetailReportData.AddCell(new PdfPCell(new Phrase("Collection Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                 collectionDetailReportData.AddCell(new PdfPCell(new Phrase("Collection Number", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                 collectionDetailReportData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                 collectionDetailReportData.AddCell(new PdfPCell(new Phrase("Loan Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                 collectionDetailReportData.AddCell(new PdfPCell(new Phrase("Loan Number", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                 collectionDetailReportData.AddCell(new PdfPCell(new Phrase("Paid Amount", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });

                 Decimal totalPaidAmount = 0;
                 foreach (var collectionLine in collectionLines)
                 {
                     collectionDetailReportData.AddCell(new PdfPCell(new Phrase(collectionLine.CollectionDate, fontArial12)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });
                     collectionDetailReportData.AddCell(new PdfPCell(new Phrase(collectionLine.CollectionNumber, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                     collectionDetailReportData.AddCell(new PdfPCell(new Phrase(collectionLine.Applicant, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                     collectionDetailReportData.AddCell(new PdfPCell(new Phrase(collectionLine.LoanDate, fontArial12)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });
                     collectionDetailReportData.AddCell(new PdfPCell(new Phrase(collectionLine.LoanNumber, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });
                     collectionDetailReportData.AddCell(new PdfPCell(new Phrase(collectionLine.Amount.ToString("#,##0.00"), fontArial12)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });

                     totalPaidAmount += collectionLine.Amount;
                 }

                 document.Add(line);
                 document.Add(collectionDetailReportData);

                 document.Add(line);

                 // table collection lines total data
                 PdfPTable collectionDetailTotalData = new PdfPTable(6);
                 float[] collectionDetailTotalDataWidthCells = new float[] { 15f, 15f, 30f, 10f, 15f, 15f };
                 collectionDetailTotalData.SetWidths(collectionDetailTotalDataWidthCells);
                 collectionDetailTotalData.WidthPercentage = 100;
                 collectionDetailTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 10f, PaddingBottom = 6f, PaddingLeft = 5f });
                 collectionDetailTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 10f, PaddingBottom = 6f, PaddingLeft = 5f });
                 collectionDetailTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 10f, PaddingBottom = 6f, PaddingLeft = 5f });
                 collectionDetailTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 10f, PaddingBottom = 6f, PaddingRight = 5f });
                 collectionDetailTotalData.AddCell(new PdfPCell(new Phrase("TOTAL", fontArial12Bold)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 10f, PaddingBottom = 6f, PaddingLeft = 5f });
                 collectionDetailTotalData.AddCell(new PdfPCell(new Phrase(totalPaidAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 10f, PaddingBottom = 6f, PaddingRight = 5f });
                 document.Add(collectionDetailTotalData);

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