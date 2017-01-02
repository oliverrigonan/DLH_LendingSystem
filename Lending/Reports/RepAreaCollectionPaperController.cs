using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Lending.Reports
{
    public class RepAreaCollectionPaperController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();
        private Business.CollectionStatus collectionStatus = new Business.CollectionStatus();

        // Collection Paper
        public ActionResult areaCollectionPaper(String collectionDate, String areaId)
        {
            if (collectionDate != null && areaId != null)
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
                Font fontArial11Bold = FontFactory.GetFont("Arial", 11, Font.BOLD);
                Font fontArial11 = FontFactory.GetFont("Arial", 11);
                Font fontArial11Red = FontFactory.GetFont("Arial", 11, BaseColor.RED);
                Font fontArial11Green = FontFactory.GetFont("Arial", 11, BaseColor.GREEN);

                // line
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));

                // user company detail
                var userCompanyDetail = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d).FirstOrDefault();

                // table main header
                PdfPTable collectionHeader = new PdfPTable(2);
                float[] collectionHeaderWidthCells = new float[] { 50f, 50f };
                collectionHeader.SetWidths(collectionHeaderWidthCells);
                collectionHeader.WidthPercentage = 100;
                collectionHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.Company, fontArial17Bold)) { Border = 0 });
                collectionHeader.AddCell(new PdfPCell(new Phrase("Area Collection Paper", fontArial17Bold)) { Border = 0, HorizontalAlignment = 2 });
                collectionHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.Address, fontArial12)) { Border = 0, PaddingTop = 5f });
                collectionHeader.AddCell(new PdfPCell(new Phrase("Collection Date: " + collectionDate, fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2, });
                collectionHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.ContactNumber, fontArial12)) { Border = 0, PaddingTop = 5f });
                collectionHeader.AddCell(new PdfPCell(new Phrase("Printed " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt"), fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2 });
                document.Add(collectionHeader);

                // table collection label
                PdfPTable collectionLabel = new PdfPTable(1);
                float[] collectionLabelWidthCells = new float[] { 100f };
                collectionLabel.SetWidths(collectionLabelWidthCells);
                collectionLabel.WidthPercentage = 100;
                collectionLabel.AddCell(new PdfPCell(new Phrase("List of daily collections (Applicants)", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });
                document.Add(line);
                document.Add(collectionLabel);

                var dailyCollections = from d in db.trnDailyCollections
                                       where d.DailyCollectionDate == Convert.ToDateTime(collectionDate)
                                       && d.trnCollection.trnLoanApplication.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                       select new Models.TrnDailyCollection
                                       {
                                           Id = d.Id,
                                           CollectionId = d.CollectionId,
                                           CollectionNumber = d.trnCollection.CollectionNumber,
                                           Applicant = d.trnCollection.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnCollection.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + (d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName != null ? d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName : " "),
                                           DailyCollectionDate = d.DailyCollectionDate.ToShortDateString(),
                                           LoanId = d.trnCollection.LoanId,
                                           NetAmount = d.NetAmount,
                                           CollectibleAmount = d.CollectibleAmount,
                                           PenaltyAmount = d.PenaltyAmount,
                                           PaidAmount = d.PaidAmount,
                                           PreviousBalanceAmount = d.PreviousBalanceAmount,
                                           CurrentBalanceAmount = d.CurrentBalanceAmount,
                                           IsCurrentCollection = d.IsCurrentCollection,
                                           IsCleared = d.IsCleared,
                                           IsAbsent = d.IsAbsent,
                                           IsPartiallyPaid = d.IsPartiallyPaid,
                                           IsPaidInAdvanced = d.IsPaidInAdvanced,
                                           IsFullyPaid = d.IsFullyPaid,
                                           IsProcessed = d.IsProcessed,
                                           CanPerformAction = d.CanPerformAction,
                                           IsDueDate = d.IsDueDate,
                                           IsAllowanceDay = d.IsAllowanceDay,
                                           IsLastDay = d.IsLastDay,
                                           ReconstructId = d.ReconstructId != null ? d.ReconstructId : 0,
                                           IsReconstructed = d.IsReconstructed,
                                           Status = collectionStatus.getStatus(d.IsCleared, d.IsAbsent, d.IsPartiallyPaid, d.IsPaidInAdvanced, d.IsFullyPaid, d.trnCollection.IsOverdue, d.IsReconstructed),
                                           IsOverdue = d.trnCollection.IsOverdue,
                                           Duedate = d.trnCollection.trnLoanApplication.MaturityDate.ToShortDateString()
                                       };

                if (dailyCollections.Any())
                {
                    var area = from d in db.mstAreas where d.Id == Convert.ToInt32(areaId) select d;
                    if (area.Any())
                    {
                        PdfPTable areaLabel = new PdfPTable(1);
                        float[] areaLabelWithCells = new float[] { 100f };
                        areaLabel.SetWidths(areaLabelWithCells);
                        areaLabel.WidthPercentage = 100;
                        areaLabel.AddCell(new PdfPCell(new Phrase(area.FirstOrDefault().Area + " Daily Collections", fontArial12Bold)) { HorizontalAlignment = 0, Border = 0, PaddingTop = 20f, PaddingBottom = 10f });
                        document.Add(areaLabel);

                        PdfPTable dailyCollection = new PdfPTable(7);
                        float[] dailyCollectionWithCells = new float[] { 10f, 25f, 10f, 10f, 10f, 10f, 18f };
                        dailyCollection.SetWidths(dailyCollectionWithCells);
                        dailyCollection.WidthPercentage = 100;
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Collection No.", fontArial11Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Applicant", fontArial11Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Due Date", fontArial11Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Collectible", fontArial11Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Balance", fontArial11Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Collected", fontArial11Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Actions", fontArial11Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                        Decimal totalCollectibles = 0;
                        Decimal totalActives = 0;
                        Decimal totalOverdue = 0;
                        Decimal totalCollection = 0;
                        foreach (var collection in dailyCollections)
                        {
                            String paidAmount = "";
                            if (collection.PaidAmount > 0)
                            {
                                paidAmount = collection.PaidAmount.ToString("#,##0.00");
                            }

                            dailyCollection.AddCell(new PdfPCell(new Phrase(collection.CollectionNumber, fontArial11Green)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                            dailyCollection.AddCell(new PdfPCell(new Phrase(collection.Applicant, fontArial11Green)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                            dailyCollection.AddCell(new PdfPCell(new Phrase(Convert.ToDateTime(collection.Duedate).ToString("MMM-dd-yyyy", CultureInfo.InvariantCulture), fontArial11Green)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                            dailyCollection.AddCell(new PdfPCell(new Phrase(collection.CollectibleAmount.ToString("#,##0.00"), fontArial11Green)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                            dailyCollection.AddCell(new PdfPCell(new Phrase(collection.CurrentBalanceAmount.ToString("#,##0.00"), fontArial11Green)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                            dailyCollection.AddCell(new PdfPCell(new Phrase(paidAmount, fontArial11Green)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                            dailyCollection.AddCell(new PdfPCell(new Phrase("", fontArial11Green)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });

                            totalCollectibles += collection.CurrentBalanceAmount;
                            totalCollection += collection.PaidAmount;

                            if (!collection.IsOverdue)
                            {
                                totalActives += collection.PaidAmount;
                            }
                            else
                            {
                                totalOverdue += collection.PaidAmount;
                            }
                        }

                        document.Add(dailyCollection);
                        document.Add(Chunk.NEWLINE);

                        // expenses
                        var expenses = from d in db.trnExpenses
                                       where d.ExpenseDate == Convert.ToDateTime(collectionDate)
                                       && d.CollectorStaffId == area.FirstOrDefault().CollectorStaffId
                                       && d.IsLocked == true
                                       && d.ExpenseTransactionTypeId == 2
                                       select d;

                        Decimal gasAllowanceExpenses = 0;
                        if (expenses.Any())
                        {
                            gasAllowanceExpenses += expenses.Sum(d => d.ExpenseAmount);
                        }

                        Decimal netCollection = (totalActives + totalOverdue) - gasAllowanceExpenses;

                        PdfPTable collectionSummaryTotal = new PdfPTable(5);
                        float[] collectionSummaryTotalWidthCells = new float[] { 30f, 20, 10, 20f, 20f };
                        collectionSummaryTotal.SetWidths(collectionSummaryTotalWidthCells);
                        collectionSummaryTotal.WidthPercentage = 100;
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("Total Balance to be Collected", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f, PaddingBottom = 20f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase(totalCollectibles.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 30f, PaddingBottom = 20f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, PaddingBottom = 20f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f, PaddingBottom = 20f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f, PaddingBottom = 20f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("Active", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase(totalActives.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 30f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0 });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("Overdue", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase(totalOverdue.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 30f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0 });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("Total Collection", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase(totalCollection.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 30f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0 });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0 });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0 });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("Gas/Allowance and other Expenses", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f, PaddingBottom = 15f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase(gasAllowanceExpenses.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 30f, PaddingBottom = 15f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, PaddingBottom = 15f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, PaddingBottom = 15f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, PaddingBottom = 15f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("Net Collection", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f, PaddingBottom = 10f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase(netCollection.ToString("#,##0.00"), fontArial12)) { HorizontalAlignment = 2, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 30f, PaddingBottom = 10f });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0 });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0 });
                        collectionSummaryTotal.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0 });
                        document.Add(collectionSummaryTotal);
                        document.Add(Chunk.NEWLINE);
                        document.Add(line);
                        document.Add(Chunk.NEWLINE);
                        document.Add(Chunk.NEWLINE);

                        // Table for Footer
                        PdfPTable tableFooter = new PdfPTable(5);
                        tableFooter.WidthPercentage = 100;
                        float[] widthsCells2 = new float[] { 20f, 5f, 20f, 5f, 20f };
                        tableFooter.SetWidths(widthsCells2);
                        tableFooter.AddCell(new PdfPCell(new Phrase("Supervisor's Collection:", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0 });
                        tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0 });
                        tableFooter.AddCell(new PdfPCell(new Phrase("Collector's Name:", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0 });
                        tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0 });
                        tableFooter.AddCell(new PdfPCell(new Phrase("Checked by:", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0 });
                        tableFooter.AddCell(new PdfPCell(new Phrase(area.FirstOrDefault().mstStaff.Staff)) { Border = 0, PaddingTop = 30f, PaddingBottom = 10f, HorizontalAlignment = 1 });
                        tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 30f, PaddingBottom = 10f });
                        tableFooter.AddCell(new PdfPCell(new Phrase(area.FirstOrDefault().mstStaff1.Staff)) { Border = 0, PaddingTop = 30f, PaddingBottom = 10f, HorizontalAlignment = 1 });
                        tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 30f, PaddingBottom = 10f });
                        tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 30f, PaddingBottom = 10f });
                        tableFooter.AddCell(new PdfPCell(new Phrase("Signature Over Printed Name")) { Border = 1, HorizontalAlignment = 1, PaddingBottom = 5f });
                        tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingBottom = 5f });
                        tableFooter.AddCell(new PdfPCell(new Phrase("Signature Over Printed Name")) { Border = 1, HorizontalAlignment = 1, PaddingBottom = 5f });
                        tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingBottom = 5f });
                        tableFooter.AddCell(new PdfPCell(new Phrase("Signature Over Printed Name")) { Border = 1, HorizontalAlignment = 1, PaddingBottom = 5f });
                        document.Add(tableFooter);
                    }
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