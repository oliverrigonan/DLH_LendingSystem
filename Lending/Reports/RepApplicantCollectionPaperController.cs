using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using iTextSharp.text.pdf;

namespace Lending.Reports
{
    public class RepApplicantCollectionPaperController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();
        private Business.CollectionStatus collectionStatus = new Business.CollectionStatus();

        // Collection Paper
        public ActionResult applicantCollectionPaper(String applicantId, String collectionId)
        {
            if (applicantId != null && collectionId != null)
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
                collectionHeader.AddCell(new PdfPCell(new Phrase("Applicant Collection Paper", fontArial17Bold)) { Border = 0, HorizontalAlignment = 2 });
                collectionHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.Address, fontArial12)) { Border = 0, PaddingTop = 5f });
                collectionHeader.AddCell(new PdfPCell(new Phrase(" ", fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2, });
                collectionHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.ContactNumber, fontArial12)) { Border = 0, PaddingTop = 5f });
                collectionHeader.AddCell(new PdfPCell(new Phrase("Printed " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt"), fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2 });
                document.Add(collectionHeader);

                var dailyCollections = from d in db.trnDailyCollections
                                       where d.CollectionId == Convert.ToInt32(collectionId)
                                       && d.trnCollection.trnLoanApplication.ApplicantId == Convert.ToInt32(applicantId)
                                       select new Models.TrnCollectionLines
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
                                           Status = collectionStatus.getStatus(d.IsCleared, d.IsAbsent, d.IsPartiallyPaid, d.IsPaidInAdvanced, d.IsFullyPaid, d.trnCollection.IsOverdue, d.IsReconstructed)
                                       };

                // table collection lines data
                PdfPTable collectionHeaderLabel = new PdfPTable(1);
                float[] collectionHeaderLabelWidthCells = new float[] { 100f };
                collectionHeaderLabel.SetWidths(collectionHeaderLabelWidthCells);
                collectionHeaderLabel.WidthPercentage = 100;
                collectionHeaderLabel.AddCell(new PdfPCell(new Phrase("Header Detail", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });
                document.Add(line);
                document.Add(collectionHeaderLabel);

                if (dailyCollections.Any())
                {
                    var loanApplications = from d in db.trnLoanApplications where d.Id == Convert.ToInt32(dailyCollections.FirstOrDefault().LoanId) select d;
                    if (loanApplications.Any())
                    {
                        // table data
                        PdfPTable loanApplicationData = new PdfPTable(4);
                        float[] loanApplicationDataWidthCells = new float[] { 15f, 45, 15f, 25f };
                        loanApplicationData.SetWidths(loanApplicationDataWidthCells);
                        loanApplicationData.WidthPercentage = 100;
                        loanApplicationData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().mstApplicant.ApplicantLastName + ", " + loanApplications.FirstOrDefault().mstApplicant.ApplicantFirstName + " " + loanApplications.FirstOrDefault().mstApplicant.ApplicantMiddleName, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 30f });
                        loanApplicationData.AddCell(new PdfPCell(new Phrase("Loan Number", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().LoanNumber, fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanApplicationData.AddCell(new PdfPCell(new Phrase("Area", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().mstApplicant.mstArea.Area, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 30f });
                        loanApplicationData.AddCell(new PdfPCell(new Phrase("Loan Date", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().LoanDate.ToLongDateString(), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanApplicationData.AddCell(new PdfPCell(new Phrase("Net Amount", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().NetAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 30f });
                        loanApplicationData.AddCell(new PdfPCell(new Phrase("Maturity Date", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().MaturityDate.ToLongDateString(), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                        document.Add(loanApplicationData);
                        document.Add(Chunk.NEWLINE);

                        // table collection lines data
                        PdfPTable collectionHeader2Label = new PdfPTable(1);
                        float[] collectionHeader2LabelWidthCells = new float[] { 100f };
                        collectionHeader2Label.SetWidths(collectionHeader2LabelWidthCells);
                        collectionHeader2Label.WidthPercentage = 100;
                        collectionHeader2Label.AddCell(new PdfPCell(new Phrase("List of Daily Collections", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });
                        document.Add(line);
                        document.Add(collectionHeader2Label);

                        PdfPTable spaceLabel = new PdfPTable(1);
                        float[] spaceLabelWithCells = new float[] { 100f };
                        spaceLabel.SetWidths(spaceLabelWithCells);
                        spaceLabel.WidthPercentage = 100;
                        spaceLabel.AddCell(new PdfPCell(new Phrase(" ")) { HorizontalAlignment = 0, Border = 0, PaddingTop = 5f, PaddingBottom = 5f });
                        document.Add(spaceLabel);

                        PdfPTable dailyCollection = new PdfPTable(8);
                        float[] dailyCollectionWithCells = new float[] { 12f, 12f, 12f, 12f, 12f, 12f, 12f, 15f };
                        dailyCollection.SetWidths(dailyCollectionWithCells);
                        dailyCollection.WidthPercentage = 100;
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Date", fontArial11Bold)) { Rowspan = 2, HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Collectible", fontArial11Bold)) { Rowspan = 2, HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Penalty", fontArial11Bold)) { Rowspan = 2, HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Balances", fontArial11Bold)) { Colspan = 2, HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Paid Amount", fontArial11Bold)) { Rowspan = 2, HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Status", fontArial11Bold)) { Rowspan = 2, HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Others", fontArial11Bold)) { Rowspan = 2, HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Previous", fontArial11Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase("Current", fontArial11Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                        Decimal totalCollectible = 0;
                        Decimal totalPenaltyAmount = 0;
                        Decimal totalCurrentBalance = 0;
                        Decimal totalPreviousBalance = 0;
                        Decimal totalPaidAmount = 0;
                        foreach (var collection in dailyCollections)
                        {
                            dailyCollection.AddCell(new PdfPCell(new Phrase(collection.DailyCollectionDate, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                            dailyCollection.AddCell(new PdfPCell(new Phrase(collection.CollectibleAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                            dailyCollection.AddCell(new PdfPCell(new Phrase(collection.PenaltyAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                            dailyCollection.AddCell(new PdfPCell(new Phrase(collection.CurrentBalanceAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                            dailyCollection.AddCell(new PdfPCell(new Phrase(collection.PreviousBalanceAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                            dailyCollection.AddCell(new PdfPCell(new Phrase(collection.PaidAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                            dailyCollection.AddCell(new PdfPCell(new Phrase(collection.Status, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                            dailyCollection.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { PaddingTop = 3f, PaddingBottom = 5f });

                            totalCollectible += collection.CollectibleAmount;
                            totalPenaltyAmount += collection.PenaltyAmount;
                            totalCurrentBalance += collection.CurrentBalanceAmount;
                            totalPreviousBalance += collection.PreviousBalanceAmount;
                            totalPaidAmount += collection.PaidAmount;
                        }

                        dailyCollection.AddCell(new PdfPCell(new Phrase("Total", fontArial11Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase(totalCollectible.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase(totalPenaltyAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase(totalCurrentBalance.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase(totalPreviousBalance.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase(totalPaidAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        dailyCollection.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { Colspan = 2, HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                        document.Add(dailyCollection);
                        document.Add(Chunk.NEWLINE);
                    }

                    // Table for Footer
                    PdfPTable tableFooter = new PdfPTable(5);
                    tableFooter.WidthPercentage = 100;
                    float[] widthsCells2 = new float[] { 20f, 5f, 20f, 5f, 20f };
                    tableFooter.SetWidths(widthsCells2);
                    tableFooter.AddCell(new PdfPCell(new Phrase("Prepared by:", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0 });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0 });
                    tableFooter.AddCell(new PdfPCell(new Phrase("Verified by:", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0 });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0 });
                    tableFooter.AddCell(new PdfPCell(new Phrase("Checked by:", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0 });
                    tableFooter.AddCell(new PdfPCell(new Phrase(loanApplications.FirstOrDefault().mstUser.FullName.ToUpper())) { Border = 0, PaddingTop = 50f, PaddingBottom = 10f, HorizontalAlignment = 1 });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 50f, PaddingBottom = 10f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 50f, PaddingBottom = 10f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 50f, PaddingBottom = 10f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 50f, PaddingBottom = 10f });
                    tableFooter.AddCell(new PdfPCell(new Phrase("Signature Over Printed Name")) { Border = 1, HorizontalAlignment = 1, PaddingBottom = 5f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingBottom = 5f });
                    tableFooter.AddCell(new PdfPCell(new Phrase("Signature Over Printed Name")) { Border = 1, HorizontalAlignment = 1, PaddingBottom = 5f });
                    tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingBottom = 5f });
                    tableFooter.AddCell(new PdfPCell(new Phrase("Signature Over Printed Name")) { Border = 1, HorizontalAlignment = 1, PaddingBottom = 5f });
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