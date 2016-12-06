using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Lending.Reports
{
    public class RepDailyReleaseMonitoringSummaryReportController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();
        
        // loan summary report
        public ActionResult dailyReleaseMonitoringSummaryReport(String startDate, String endDate)
        {
            if (startDate != null && endDate != null)
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

                // line
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));

                // user company detail
                var userCompanyDetail = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d).FirstOrDefault();

                // table main header
                PdfPTable dailyReleaseMonitoringReportHeader = new PdfPTable(2);
                float[] dailyReleaseMonitoringReportHeaderWidthCells = new float[] { 50f, 50f };
                dailyReleaseMonitoringReportHeader.SetWidths(dailyReleaseMonitoringReportHeaderWidthCells);
                dailyReleaseMonitoringReportHeader.WidthPercentage = 100;
                dailyReleaseMonitoringReportHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.Company, fontArial17Bold)) { Border = 0 });
                dailyReleaseMonitoringReportHeader.AddCell(new PdfPCell(new Phrase("Daily Release Monitoring Summary Report", fontArial17Bold)) { Border = 0, HorizontalAlignment = 2 });
                dailyReleaseMonitoringReportHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.Address, fontArial12)) { Border = 0, PaddingTop = 5f });
                dailyReleaseMonitoringReportHeader.AddCell(new PdfPCell(new Phrase("From " + startDate + " to " + endDate, fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2, });
                dailyReleaseMonitoringReportHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.ContactNumber, fontArial12)) { Border = 0, PaddingTop = 5f });
                dailyReleaseMonitoringReportHeader.AddCell(new PdfPCell(new Phrase("Printed " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt"), fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2 });
                document.Add(dailyReleaseMonitoringReportHeader);

                // table loan label
                PdfPTable dailyReleaseMonitoringReportLabel = new PdfPTable(1);
                float[] dailyReleaseMonitoringReportLabelWidthCells = new float[] { 100f };
                dailyReleaseMonitoringReportLabel.SetWidths(dailyReleaseMonitoringReportLabelWidthCells);
                dailyReleaseMonitoringReportLabel.WidthPercentage = 100;
                dailyReleaseMonitoringReportLabel.AddCell(new PdfPCell(new Phrase("List of Loan Application Activities", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });
                document.Add(line);
                document.Add(dailyReleaseMonitoringReportLabel);

                // loan applications
                var loanApplications = from d in db.trnLoanApplications
                                       where d.LoanDate >= Convert.ToDateTime(startDate)
                                       && d.LoanDate <= Convert.ToDateTime(endDate)
                                       && d.IsLocked == true
                                       select new Models.TrnLoanApplication
                                       {
                                           Id = d.Id,
                                           LoanNumber = d.LoanNumber,
                                           LoanDate = d.LoanDate.ToShortDateString(),
                                           MaturityDate = d.MaturityDate.ToShortDateString(),
                                           AccountId = d.AccountId,
                                           Account = d.mstAccount.Account,
                                           ApplicantId = d.ApplicantId,
                                           Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                           Area = d.mstApplicant.mstArea.Area,
                                           Particulars = d.Particulars,
                                           LoanTypeId = d.LoanTypeId,
                                           LoanType = d.mstLoanType.LoanType,
                                           PreparedByUserId = d.PreparedByUserId,
                                           PreparedByUser = d.mstUser.FullName,
                                           PrincipalAmount = d.PrincipalAmount,
                                           ProcessingFeeAmount = d.ProcessingFeeAmount,
                                           PassbookAmount = d.PassbookAmount,
                                           BalanceAmount = d.BalanceAmount,
                                           PenaltyAmount = d.PenaltyAmount,
                                           LateIntAmount = d.LateIntAmount,
                                           AdvanceAmount = d.AdvanceAmount,
                                           RequirementsAmount = d.RequirementsAmount,
                                           InsuranceIPIorPPIAmount = d.InsuranceIPIorPPIAmount,
                                           NetAmount = d.NetAmount,
                                           IsFullyPaid = d.IsFullyPaid,
                                           IsLocked = d.IsLocked,
                                           CreatedByUserId = d.CreatedByUserId,
                                           CreatedByUser = d.mstUser1.FullName,
                                           CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                           UpdatedByUserId = d.UpdatedByUserId,
                                           UpdatedByUser = d.mstUser2.FullName,
                                           UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                                       };

                if (loanApplications.Any())
                {
                    PdfPTable spaceLabel = new PdfPTable(1);
                    float[] spaceLabelWithCells = new float[] { 100f };
                    spaceLabel.SetWidths(spaceLabelWithCells);
                    spaceLabel.WidthPercentage = 100;
                    spaceLabel.AddCell(new PdfPCell(new Phrase(" ")) { HorizontalAlignment = 0, Border = 0, PaddingTop = 5f, PaddingBottom = 5f });
                    document.Add(spaceLabel);

                    PdfPTable dailReleaseMonitoringSummaryReportActivities = new PdfPTable(6);
                    float[] dailReleaseMonitoringSummaryReportActivitiesWithCells = new float[] { 15f, 30f, 20f, 10f, 15f, 10f, };
                    dailReleaseMonitoringSummaryReportActivities.SetWidths(dailReleaseMonitoringSummaryReportActivitiesWithCells);
                    dailReleaseMonitoringSummaryReportActivities.WidthPercentage = 100;
                    dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Loan No.", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Area", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Amount", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Signature", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                    Decimal totalNetAmount = 0;

                    foreach (var loanApplication in loanApplications)
                    {
                        totalNetAmount += loanApplication.NetAmount;

                        dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase(loanApplication.LoanNumber, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                        dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase(loanApplication.Applicant, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                        dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase(loanApplication.Area, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                        dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase(loanApplication.LoanDate, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                        dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase(loanApplication.NetAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                        dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                    }

                    dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Total", fontArial11Bold)) { Colspan = 4, HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase(totalNetAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    dailReleaseMonitoringSummaryReportActivities.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    document.Add(dailReleaseMonitoringSummaryReportActivities);
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