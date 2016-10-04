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
    public class RepCollectionSummaryReportController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // get total amount in collection lines
        public Decimal getTotalPaidAmount(Int32 collectionId)
        {
            Decimal totalPaidAmount = 0;
            var collectionLines = from d in db.trnCollectionLines where d.CollectionId == collectionId select d;
            if (collectionLines.Any())
            {
                totalPaidAmount = collectionLines.Sum(d => d.Amount);
            }

            return totalPaidAmount;
        }

        // collection summary report
        public ActionResult collectionSummaryReport(String startDate, String endDate)
        {
            if (startDate != null && endDate != null)
            {
                var collections = from d in db.trnCollections.OrderBy(d => d.CollectionDate)
                                  where d.CollectionDate >= Convert.ToDateTime(startDate)
                                  && d.CollectionDate <= Convert.ToDateTime(endDate)
                                  && d.IsLocked == true
                                  select new Models.TrnCollection
                                  {
                                      Id = d.Id,
                                      CollectionNumber = d.CollectionNumber,
                                      CollectionDate = d.CollectionDate.ToShortDateString(),
                                      ApplicantId = d.ApplicantId,
                                      Applicant = d.mstApplicant.ApplicantLastName + " " + d.mstApplicant.ApplicantFirstName + ", " + d.mstApplicant.ApplicantMiddleName,
                                      Particulars = d.Particulars,
                                      PaidAmount = getTotalPaidAmount(d.Id),
                                      PreparedByUserId = d.PreparedByUserId,
                                      PreparedByUser = d.mstUser2.FullName,
                                      IsLocked = d.IsLocked,
                                      CreatedByUserId = d.CreatedByUserId,
                                      CreatedByUser = d.mstUser.FullName,
                                      CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                      UpdatedByUserId = d.UpdatedByUserId,
                                      UpdatedByUser = d.mstUser1.FullName,
                                      UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
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
                PdfPTable collectionSummaryReportheader = new PdfPTable(2);
                float[] collectionSummaryReportheaderWidthCells = new float[] { 50f, 50f };
                collectionSummaryReportheader.SetWidths(collectionSummaryReportheaderWidthCells);
                collectionSummaryReportheader.WidthPercentage = 100;
                collectionSummaryReportheader.AddCell(new PdfPCell(new Phrase("DLH Incorporated", fontArial17Bold)) { Border = 0 });
                collectionSummaryReportheader.AddCell(new PdfPCell(new Phrase("Collection Summary Report", fontArial17Bold)) { Border = 0, HorizontalAlignment = 2 });
                collectionSummaryReportheader.AddCell(new PdfPCell(new Phrase("Pardo", fontArial12)) { Border = 0, PaddingTop = 5f });
                collectionSummaryReportheader.AddCell(new PdfPCell(new Phrase("Date from " + startDate + " to " + endDate, fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2, });
                collectionSummaryReportheader.AddCell(new PdfPCell(new Phrase("0932-444-1234", fontArial12)) { Border = 0, PaddingTop = 5f });
                collectionSummaryReportheader.AddCell(new PdfPCell(new Phrase("Printed " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt"), fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2 });
                document.Add(collectionSummaryReportheader);

                // table collection data
                PdfPTable collectionHeaderLabel = new PdfPTable(1);
                float[] collectionHeaderLabelWidthCells = new float[] { 100f };
                collectionHeaderLabel.SetWidths(collectionHeaderLabelWidthCells);
                collectionHeaderLabel.WidthPercentage = 100;
                collectionHeaderLabel.AddCell(new PdfPCell(new Phrase("List of Collection Summary Report", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });

                document.Add(line);
                document.Add(collectionHeaderLabel);

                // table collection summary data
                PdfPTable collectionSummaryReportData = new PdfPTable(4);
                float[] collectionSummaryReportDataWidthCells = new float[] { 15f, 15f, 35f, 25f };
                collectionSummaryReportData.SetWidths(collectionSummaryReportDataWidthCells);
                collectionSummaryReportData.WidthPercentage = 100;
                collectionSummaryReportData.AddCell(new PdfPCell(new Phrase("Collection Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                collectionSummaryReportData.AddCell(new PdfPCell(new Phrase("Collection Number", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                collectionSummaryReportData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                collectionSummaryReportData.AddCell(new PdfPCell(new Phrase("Paid Amount", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });

                Decimal totalPaidAmount = 0;
                foreach (var collection in collections)
                {
                    collectionSummaryReportData.AddCell(new PdfPCell(new Phrase(collection.CollectionDate, fontArial12)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });
                    collectionSummaryReportData.AddCell(new PdfPCell(new Phrase(collection.CollectionNumber, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                    collectionSummaryReportData.AddCell(new PdfPCell(new Phrase(collection.Applicant, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                    collectionSummaryReportData.AddCell(new PdfPCell(new Phrase(collection.PaidAmount.ToString("#,##0.00"), fontArial12)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });

                    totalPaidAmount += collection.PaidAmount;
                }

                document.Add(line);
                document.Add(collectionSummaryReportData);

                document.Add(line);

                // table collection lines total data
                PdfPTable collectionSummaryTotalData = new PdfPTable(4);
                float[] collectionSummaryTotalDataWidthCells = new float[] { 15f, 15f, 35f, 25f };
                collectionSummaryTotalData.SetWidths(collectionSummaryTotalDataWidthCells);
                collectionSummaryTotalData.WidthPercentage = 100;
                collectionSummaryTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 10f, PaddingBottom = 6f, PaddingLeft = 5f });
                collectionSummaryTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 10f, PaddingBottom = 6f, PaddingRight = 5f });
                collectionSummaryTotalData.AddCell(new PdfPCell(new Phrase("TOTAL", fontArial12Bold)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 10f, PaddingBottom = 6f, PaddingLeft = 5f });
                collectionSummaryTotalData.AddCell(new PdfPCell(new Phrase(totalPaidAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 10f, PaddingBottom = 6f, PaddingRight = 5f });
                document.Add(collectionSummaryTotalData);

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