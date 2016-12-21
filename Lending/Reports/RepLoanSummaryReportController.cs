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
    public class RepLoanSummaryReportController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan summary report
        public ActionResult loanSummaryReport(String startDate, String endDate)
        {
            if (startDate != null && endDate != null)
            {
                // PDF settings
                MemoryStream workStream = new MemoryStream();
                Document document = new Document();
                document.SetPageSize(iTextSharp.text.PageSize.A3.Rotate());
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
                PdfPTable loanSummaryHeader = new PdfPTable(2);
                float[] loanSummaryHeaderWidthCells = new float[] { 50f, 50f };
                loanSummaryHeader.SetWidths(loanSummaryHeaderWidthCells);
                loanSummaryHeader.WidthPercentage = 100;
                loanSummaryHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.Company, fontArial17Bold)) { Border = 0 });
                loanSummaryHeader.AddCell(new PdfPCell(new Phrase("Loan Summary Report", fontArial17Bold)) { Border = 0, HorizontalAlignment = 2 });
                loanSummaryHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.Address, fontArial12)) { Border = 0, PaddingTop = 5f });
                loanSummaryHeader.AddCell(new PdfPCell(new Phrase("From " + startDate + " to " + endDate, fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2, });
                loanSummaryHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.ContactNumber, fontArial12)) { Border = 0, PaddingTop = 5f });
                loanSummaryHeader.AddCell(new PdfPCell(new Phrase("Printed " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt"), fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2 });
                document.Add(loanSummaryHeader);

                // table loan summary label
                PdfPTable loanSummaryLabel = new PdfPTable(1);
                float[] loanSummaryLabelWidthCells = new float[] { 100f };
                loanSummaryLabel.SetWidths(loanSummaryLabelWidthCells);
                loanSummaryLabel.WidthPercentage = 100;
                loanSummaryLabel.AddCell(new PdfPCell(new Phrase("List of Loan Application Activities", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });
                document.Add(line);
                document.Add(loanSummaryLabel);

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
                                           TermId = d.TermId,
                                           Term = d.mstTerm.Term,
                                           InterestId = d.InterestId,
                                           Interest = d.mstInterest.Interest,
                                           InterestRate = d.InterestRate,
                                           PenaltyId = d.PenaltyId,
                                           Penalty = d.mstPenalty.Penalty,
                                           PreparedByUser = d.mstUser.FullName,
                                           PrincipalAmount = d.PrincipalAmount,
                                           ProcessingFeeAmountDeduction = d.ProcessingFeeAmountDeduction,
                                           PassbookAmountDeduction = d.PassbookAmountDeduction,
                                           BalanceAmountDeduction = d.BalanceAmountDeduction,
                                           PenaltyAmountDeduction = d.PenaltyAmountDeduction,
                                           LateIntAmountDeduction = d.LateIntAmountDeduction,
                                           AdvanceAmountDeduction = d.AdvanceAmountDeduction,
                                           RequirementsAmountDeduction = d.RequirementsAmountDeduction,
                                           InsuranceIPIorPPIAmountDeduction = d.InsuranceIPIorPPIAmountDeduction,
                                           NetAmount = d.NetAmount,
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

                    PdfPTable loanApplicationActivities = new PdfPTable(14);
                    float[] loanApplicationActivitiesWithCells = new float[] { 6f, 12f, 8f, 6f, 6f, 6f, 6f, 6f, 6f, 6f, 6f, 6f, 6f, 6f };
                    loanApplicationActivities.SetWidths(loanApplicationActivitiesWithCells);
                    loanApplicationActivities.WidthPercentage = 100;
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("Loan No.", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("Area", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("Principal", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("PF", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("PB", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("BAL", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("PEN", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("LATE INT", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("ADV", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("REQ", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("PP/IPI", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("TD", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("Net", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                    Decimal totalPricipal = 0;
                    Decimal totalProcessingFee = 0;
                    Decimal totalPassbook = 0;
                    Decimal totalBalance = 0;
                    Decimal totalPenalty = 0;
                    Decimal totalLateInt = 0;
                    Decimal totalAdvance = 0;
                    Decimal totalRequirements = 0;
                    Decimal totalPPIPI = 0;
                    Decimal totalAllDeductions = 0;
                    Decimal totalNetAmount = 0;

                    foreach (var loanApplication in loanApplications)
                    {
                        Decimal totalDeductions = loanApplication.ProcessingFeeAmountDeduction + loanApplication.PassbookAmountDeduction + loanApplication.BalanceAmountDeduction + loanApplication.PenaltyAmountDeduction + loanApplication.LateIntAmountDeduction + loanApplication.AdvanceAmountDeduction + loanApplication.RequirementsAmountDeduction + loanApplication.InsuranceIPIorPPIAmountDeduction;

                        totalPricipal += loanApplication.PrincipalAmount;
                        totalProcessingFee += loanApplication.ProcessingFeeAmountDeduction;
                        totalPassbook += loanApplication.PassbookAmountDeduction;
                        totalBalance += loanApplication.BalanceAmountDeduction;
                        totalPenalty += loanApplication.PenaltyAmountDeduction;
                        totalLateInt += loanApplication.LateIntAmountDeduction;
                        totalAdvance += loanApplication.AdvanceAmountDeduction;
                        totalRequirements += loanApplication.RequirementsAmountDeduction;
                        totalPPIPI += loanApplication.InsuranceIPIorPPIAmountDeduction;
                        totalAllDeductions += totalDeductions;
                        totalNetAmount += loanApplication.NetAmount;

                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(loanApplication.LoanNumber, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(loanApplication.Applicant, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(loanApplication.Area, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(loanApplication.PrincipalAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(loanApplication.ProcessingFeeAmountDeduction.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(loanApplication.PassbookAmountDeduction.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(loanApplication.BalanceAmountDeduction.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(loanApplication.PenaltyAmountDeduction.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(loanApplication.LateIntAmountDeduction.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(loanApplication.AdvanceAmountDeduction.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(loanApplication.RequirementsAmountDeduction.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(loanApplication.InsuranceIPIorPPIAmountDeduction.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(totalDeductions.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                        loanApplicationActivities.AddCell(new PdfPCell(new Phrase(loanApplication.NetAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                    }

                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase("Total", fontArial11Bold)) { Colspan = 3, HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase(totalPricipal.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase(totalProcessingFee.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase(totalPassbook.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase(totalBalance.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase(totalPenalty.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase(totalLateInt.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase(totalAdvance.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase(totalRequirements.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase(totalPPIPI.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase(totalAllDeductions.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanApplicationActivities.AddCell(new PdfPCell(new Phrase(totalNetAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                    document.Add(loanApplicationActivities);
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