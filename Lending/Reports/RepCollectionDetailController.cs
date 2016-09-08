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
    public class RepCollectionDetailController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();


        // loan application detail
        [Authorize]
        public ActionResult collectionDetail(Int32? collectionId)
        {
            if (collectionId != null)
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
                PdfPTable collectionHeader = new PdfPTable(2);
                float[] collectionHeaderrWidthCells = new float[] { 50f, 50f };
                collectionHeader.SetWidths(collectionHeaderrWidthCells);
                collectionHeader.WidthPercentage = 100;
                collectionHeader.AddCell(new PdfPCell(new Phrase("DLH Incorporated", fontArial17Bold)) { Border = 0 });
                collectionHeader.AddCell(new PdfPCell(new Phrase("Collection Detail Report", fontArial17Bold)) { Border = 0, HorizontalAlignment = 2 });
                collectionHeader.AddCell(new PdfPCell(new Phrase("Pardo", fontArial12)) { Border = 0, PaddingTop = 5f });
                collectionHeader.AddCell(new PdfPCell(new Phrase("Pardo Branch", fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2, });
                collectionHeader.AddCell(new PdfPCell(new Phrase("0932-444-1234", fontArial12)) { Border = 0, PaddingTop = 5f });
                collectionHeader.AddCell(new PdfPCell(new Phrase("Printed " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt"), fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2 });
                document.Add(collectionHeader);

                // table collection lines data
                PdfPTable collectionHeaderLabel = new PdfPTable(1);
                float[] collectionHeaderLabelWidthCells = new float[] { 100f };
                collectionHeaderLabel.SetWidths(collectionHeaderLabelWidthCells);
                collectionHeaderLabel.WidthPercentage = 100;
                collectionHeaderLabel.AddCell(new PdfPCell(new Phrase("Header Detail", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });

                document.Add(line);
                document.Add(collectionHeaderLabel);

                // queries
                var collections = from d in db.trnCollections where d.Id == collectionId select d;
                if (collections.Any())
                {
                    // table data
                    PdfPTable collectionData = new PdfPTable(4);
                    float[] collectionDataWidthCells = new float[] { 15f, 40, 20f, 25f };
                    collectionData.SetWidths(collectionDataWidthCells);
                    collectionData.WidthPercentage = 100;
                    collectionData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(collections.FirstOrDefault().mstApplicant.ApplicantFullName, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase("Collection Number", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(collections.FirstOrDefault().CollectionNumber, fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase("Branch", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(collections.FirstOrDefault().mstBranch.Branch, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase("Collection Date", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(collections.FirstOrDefault().CollectionDate.ToLongDateString(), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase("Particulars", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(collections.FirstOrDefault().Particulars, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase("", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    document.Add(collectionData);

                    // table collection lines data
                    PdfPTable collectionLinesData = new PdfPTable(7);
                    float[] collectionLinesDataWidthCells = new float[] { 12f, 10f, 19f, 14f, 15f, 15f, 15f };
                    collectionLinesData.SetWidths(collectionLinesDataWidthCells);
                    collectionLinesData.WidthPercentage = 100;
                    collectionLinesData.AddCell(new PdfPCell(new Phrase("Loan Number", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionLinesData.AddCell(new PdfPCell(new Phrase("Loan Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionLinesData.AddCell(new PdfPCell(new Phrase("Collected By", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionLinesData.AddCell(new PdfPCell(new Phrase("Pay Type", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionLinesData.AddCell(new PdfPCell(new Phrase("Loan Amount", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionLinesData.AddCell(new PdfPCell(new Phrase("Paid Amount", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionLinesData.AddCell(new PdfPCell(new Phrase("Balance", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });

                    // collection lines
                    var collectionLines = from d in db.trnCollectionLines.OrderByDescending(d => d.Id)
                                          where d.CollectionId == collectionId
                                          select new Models.TrnCollectionLines
                                          {
                                              Id = d.Id,
                                              CollectionId = d.CollectionId,
                                              AccountId = d.AccountId,
                                              Account = d.mstAccount.Account,
                                              LoanId = d.LoanId,
                                              LoanNumber = d.trnLoanApplication.LoanNumber,
                                              LoanDate = d.trnLoanApplication.LoanDate.ToShortDateString(),
                                              PaytypeId = d.PaytypeId,
                                              Paytype = d.mstPayType.PayType,
                                              CheckNumber = d.CheckNumber,
                                              CheckDate = d.CheckDate.ToShortDateString(),
                                              CheckBank = d.CheckBank,
                                              Particulars = d.Particulars,
                                              Amount = d.Amount,
                                              LoanAmount = d.trnLoanApplication.LoanAmount,
                                              PaidAmount = d.trnLoanApplication.PaidAmount,
                                              Balance = d.trnLoanApplication.BalanceAmount,
                                              CollectedByCollectorId = d.CollectedByCollectorId,
                                              CollectedByCollector = d.mstCollector.Collector
                                          };

                    Decimal totalLoanAmount = 0;
                    Decimal totalPaidAmount = 0;
                    Decimal totalBalanceAmount = 0;

                    if (collectionLines.Any())
                    {
                        foreach (var collectionLine in collectionLines)
                        {
                            collectionLinesData.AddCell(new PdfPCell(new Phrase(collectionLine.LoanNumber, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });
                            collectionLinesData.AddCell(new PdfPCell(new Phrase(collectionLine.LoanDate, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                            collectionLinesData.AddCell(new PdfPCell(new Phrase(collectionLine.CollectedByCollector, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });
                            collectionLinesData.AddCell(new PdfPCell(new Phrase(collectionLine.Paytype, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                            collectionLinesData.AddCell(new PdfPCell(new Phrase(collectionLine.LoanAmount.ToString("#,##0.00"), fontArial12)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                            collectionLinesData.AddCell(new PdfPCell(new Phrase(collectionLine.PaidAmount.ToString("#,##0.00"), fontArial12)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                            collectionLinesData.AddCell(new PdfPCell(new Phrase(collectionLine.Balance.ToString("#,##0.00"), fontArial12)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });

                            totalLoanAmount += collectionLine.LoanAmount;
                            totalPaidAmount += collectionLine.PaidAmount;
                            totalBalanceAmount += collectionLine.Balance;
                            
                        }
                    }

                    document.Add(Chunk.NEWLINE);
                    document.Add(line);

                    // table collection lines data
                    PdfPTable collectionHeader2Label = new PdfPTable(1);
                    float[] collectionHeaderLabel2WidthCells = new float[] { 100f };
                    collectionHeader2Label.SetWidths(collectionHeaderLabel2WidthCells);
                    collectionHeader2Label.WidthPercentage = 100;
                    collectionHeader2Label.AddCell(new PdfPCell(new Phrase("Paid Amounts", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });

                    document.Add(collectionHeader2Label);
                    document.Add(line);
                    document.Add(collectionLinesData);

                    document.Add(Chunk.NEWLINE);

                    // table collection lines total data
                    PdfPTable collectionLinesTotalData = new PdfPTable(7);
                    float[] collectionLinesTotalDataWidthCells = new float[] { 12f, 10f, 19f, 14f, 15f, 15f, 15f };
                    collectionLinesTotalData.SetWidths(collectionLinesTotalDataWidthCells);
                    collectionLinesTotalData.WidthPercentage = 100;
                    collectionLinesTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) {Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });
                    collectionLinesTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                    collectionLinesTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });
                    collectionLinesTotalData.AddCell(new PdfPCell(new Phrase("TOTAL", fontArial12Bold)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                    collectionLinesTotalData.AddCell(new PdfPCell(new Phrase(totalLoanAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                    collectionLinesTotalData.AddCell(new PdfPCell(new Phrase(totalPaidAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                    collectionLinesTotalData.AddCell(new PdfPCell(new Phrase(totalBalanceAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                    document.Add(collectionLinesTotalData);

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
                    tableFooter.AddCell(new PdfPCell(new Phrase("Prepared by:", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingLeft = 5f, PaddingRight = 5f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0 });
                    tableFooter.AddCell(new PdfPCell(new Phrase("Verified by:", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingLeft = 5f, PaddingRight = 5f });

                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 10f, PaddingBottom = 10f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 10f, PaddingBottom = 10f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 10f, PaddingBottom = 10f });

                    tableFooter.AddCell(new PdfPCell(new Phrase(collections.FirstOrDefault().mstUser.FullName)) { Border = 1, HorizontalAlignment = 1, PaddingBottom = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingBottom = 5f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(collections.FirstOrDefault().mstUser1.FullName)) { Border = 1, HorizontalAlignment = 1, PaddingBottom = 5f, PaddingLeft = 5f, PaddingRight = 5f });
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