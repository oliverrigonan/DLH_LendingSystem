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
    public class RepDailyReleaseMonitoringController : Controller
    {
        private Data.LendingDataContext db = new Data.LendingDataContext();

        public List<Models.TrnLoan> listLoanApplications(String startDate, String endDate, String areaId)
        {
            if (areaId.Equals("0"))
            {
                var loanApplications = from d in db.trnLoans.OrderBy(d => d.mstApplicant.ApplicantLastName)
                                       where d.LoanDate >= Convert.ToDateTime(startDate)
                                       && d.LoanDate <= Convert.ToDateTime(endDate)
                                       && d.IsLocked == true
                                       && d.IsLoanReconstruct != true
                                       select new Models.TrnLoan
                                       {
                                           Id = d.Id,
                                           LoanNumber = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
                                           LoanDate = d.LoanDate.ToShortDateString(),
                                           ApplicantId = d.ApplicantId,
                                           Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                           Area = d.mstApplicant.mstArea.Area,
                                           Particulars = d.Particulars,
                                           PreparedByUserId = d.PreparedByUserId,
                                           PreparedByUser = d.mstUser.FullName,
                                           TermId = d.TermId,
                                           Term = d.mstTerm.Term,
                                           TermNoOfDays = d.TermNoOfDays,
                                           MaturityDate = d.MaturityDate.ToShortDateString(),
                                           PrincipalAmount = d.PrincipalAmount,
                                           InterestId = d.InterestId,
                                           Interest = d.mstInterest.Interest,
                                           InterestRate = d.InterestRate,
                                           InterestAmount = d.InterestAmount,
                                           PreviousBalanceAmount = d.PreviousBalanceAmount,
                                           DeductionAmount = d.DeductionAmount,
                                           NetAmount = d.NetAmount,
                                           NetCollectionAmount = d.NetCollectionAmount,
                                           CollectibleAmount = d.CollectibleAmount,
                                           TotalPaidAmount = d.TotalPaidAmount,
                                           TotalPenaltyAmount = d.TotalPenaltyAmount,
                                           TotalBalanceAmount = d.TotalBalanceAmount,
                                           IsLoanApplication = d.IsLoanApplication,
                                           IsLoanReconstruct = d.IsLoanReconstruct,
                                           IsLoanRenew = d.IsLoanRenew,
                                           IsLocked = d.IsLocked,
                                           CreatedByUserId = d.CreatedByUserId,
                                           CreatedByUser = d.mstUser1.FullName,
                                           CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                           UpdatedByUserId = d.UpdatedByUserId,
                                           UpdatedByUser = d.mstUser2.FullName,
                                           UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                       };

                return loanApplications.ToList();
            }
            else
            {
                var loanApplications = from d in db.trnLoans.OrderBy(d => d.mstApplicant.ApplicantLastName)
                                       where d.LoanDate >= Convert.ToDateTime(startDate)
                                       && d.LoanDate <= Convert.ToDateTime(endDate)
                                       && d.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                       && d.IsLocked == true
                                       && d.IsLoanReconstruct != true
                                       select new Models.TrnLoan
                                       {
                                           Id = d.Id,
                                           LoanNumber = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
                                           LoanDate = d.LoanDate.ToShortDateString(),
                                           ApplicantId = d.ApplicantId,
                                           Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                           Area = d.mstApplicant.mstArea.Area,
                                           Particulars = d.Particulars,
                                           PreparedByUserId = d.PreparedByUserId,
                                           PreparedByUser = d.mstUser.FullName,
                                           TermId = d.TermId,
                                           Term = d.mstTerm.Term,
                                           TermNoOfDays = d.TermNoOfDays,
                                           MaturityDate = d.MaturityDate.ToShortDateString(),
                                           PrincipalAmount = d.PrincipalAmount,
                                           InterestId = d.InterestId,
                                           Interest = d.mstInterest.Interest,
                                           InterestRate = d.InterestRate,
                                           InterestAmount = d.InterestAmount,
                                           PreviousBalanceAmount = d.PreviousBalanceAmount,
                                           DeductionAmount = d.DeductionAmount,
                                           NetAmount = d.NetAmount,
                                           NetCollectionAmount = d.NetCollectionAmount,
                                           CollectibleAmount = d.CollectibleAmount,
                                           TotalPaidAmount = d.TotalPaidAmount,
                                           TotalPenaltyAmount = d.TotalPenaltyAmount,
                                           TotalBalanceAmount = d.TotalBalanceAmount,
                                           IsLoanApplication = d.IsLoanApplication,
                                           IsLoanReconstruct = d.IsLoanReconstruct,
                                           IsLoanRenew = d.IsLoanRenew,
                                           IsLocked = d.IsLocked,
                                           CreatedByUserId = d.CreatedByUserId,
                                           CreatedByUser = d.mstUser1.FullName,
                                           CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                           UpdatedByUserId = d.UpdatedByUserId,
                                           UpdatedByUser = d.mstUser2.FullName,
                                           UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                       };

                return loanApplications.ToList();
            }
        }

        public ActionResult dailyReleaseMonitoringPDF(String startDate, String endDate, String areaId)
        {
            if (startDate != null && endDate != null)
            {
                if (areaId != null)
                {
                    var loanApplications = from d in this.listLoanApplications(startDate, endDate, areaId)
                                           select new Models.TrnLoan
                                           {
                                               Id = d.Id,
                                               LoanNumber = d.LoanNumber,
                                               LoanDate = d.LoanDate,
                                               ApplicantId = d.ApplicantId,
                                               Applicant = d.Applicant,
                                               Area = d.Area,
                                               Particulars = d.Particulars,
                                               PreparedByUserId = d.PreparedByUserId,
                                               PreparedByUser = d.PreparedByUser,
                                               TermId = d.TermId,
                                               Term = d.Term,
                                               TermNoOfDays = d.TermNoOfDays,
                                               MaturityDate = d.MaturityDate,
                                               PrincipalAmount = d.PrincipalAmount,
                                               InterestId = d.InterestId,
                                               Interest = d.Interest,
                                               InterestRate = d.InterestRate,
                                               InterestAmount = d.InterestAmount,
                                               PreviousBalanceAmount = d.PreviousBalanceAmount,
                                               DeductionAmount = d.DeductionAmount,
                                               NetAmount = d.NetAmount,
                                               NetCollectionAmount = d.NetCollectionAmount,
                                               CollectibleAmount = d.CollectibleAmount,
                                               TotalPaidAmount = d.TotalPaidAmount,
                                               TotalPenaltyAmount = d.TotalPenaltyAmount,
                                               TotalBalanceAmount = d.TotalBalanceAmount,
                                               IsLoanApplication = d.IsLoanApplication,
                                               IsLoanReconstruct = d.IsLoanReconstruct,
                                               IsLoanRenew = d.IsLoanRenew,
                                               IsReconstructed = d.IsReconstructed,
                                               IsRenewed = d.IsRenewed,
                                               IsLocked = d.IsLocked,
                                               CreatedByUserId = d.CreatedByUserId,
                                               CreatedByUser = d.CreatedByUser,
                                               CreatedDateTime = d.CreatedDateTime,
                                               UpdatedByUserId = d.UpdatedByUserId,
                                               UpdatedByUser = d.UpdatedByUser,
                                               UpdatedDateTime = d.UpdatedDateTime,
                                           };

                    if (loanApplications.Any())
                    {
                        MemoryStream workStream = new MemoryStream();
                        Rectangle rectangle = new Rectangle(612f, 936f);
                        Document document = new Document(rectangle, 72, 72, 72, 72);
                        document.SetMargins(30f, 30f, 50f, 20f);
                        PdfWriter.GetInstance(document, workStream).CloseStream = false;

                        document.Open();

                        //Font fontArial19Bold = FontFactory.GetFont("Arial", 20, Font.BOLD);
                        //Font fontArial17Bold = FontFactory.GetFont("Arial", 17, Font.BOLD);
                        //Font fontArial16Bold = FontFactory.GetFont("Arial", 16, Font.BOLD);
                        //Font fontArial12Bold = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        //Font fontArial13Bold = FontFactory.GetFont("Arial", 13, Font.BOLD);
                        //Font fontArial12 = FontFactory.GetFont("Arial", 12);
                        //Font fontArial11Bold = FontFactory.GetFont("Arial", 11, Font.BOLD);
                        //Font fontArial11 = FontFactory.GetFont("Arial", 11);
                        //Font fontArial11ITALIC = FontFactory.GetFont("Arial", 12, Font.ITALIC);
                        //Font fontArial10Bold = FontFactory.GetFont("Arial", 10, Font.BOLD);
                        //Font fontArial10 = FontFactory.GetFont("Arial", 10);
                        //Font fontArial10ITALIC = FontFactory.GetFont("Arial", 10, Font.ITALIC);

                        // Fonts
                        Font fontArial19Bold = FontFactory.GetFont("Arial", 17, Font.BOLD);
                        Font fontArial17Bold = FontFactory.GetFont("Arial", 14, Font.BOLD);
                        Font fontArial16Bold = FontFactory.GetFont("Arial", 13, Font.BOLD);
                        Font fontArial12Bold = FontFactory.GetFont("Arial", 9, Font.BOLD);
                        Font fontArial13Bold = FontFactory.GetFont("Arial", 10, Font.BOLD);
                        Font fontArial12 = FontFactory.GetFont("Arial", 9);
                        Font fontArial11Bold = FontFactory.GetFont("Arial", 8, Font.BOLD);
                        Font fontArial11 = FontFactory.GetFont("Arial", 8);
                        Font fontArial11ITALIC = FontFactory.GetFont("Arial", 9, Font.ITALIC);
                        Font fontArial10Bold = FontFactory.GetFont("Arial", 7, Font.BOLD);
                        Font fontArial10 = FontFactory.GetFont("Arial", 7);
                        Font fontArial10ITALIC = FontFactory.GetFont("Arial", 7, Font.ITALIC);

                        Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));

                        var userCompanyDetail = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d).FirstOrDefault();

                        // image
                        string imagepath = Server.MapPath("~/Images/dlhicon.jpg");
                        Image logo = Image.GetInstance(imagepath);
                        logo.ScalePercent(11f);
                        PdfPCell imageCell = new PdfPCell(logo);

                        // header
                        PdfPTable loanApplicationheader = new PdfPTable(2);
                        float[] loanApplicationheaderWidthCells = new float[] { 7f, 100f };
                        loanApplicationheader.SetWidths(loanApplicationheaderWidthCells);
                        loanApplicationheader.WidthPercentage = 100;
                        loanApplicationheader.AddCell(new PdfPCell(imageCell) { Rowspan = 3, Border = 0, PaddingRight = 10f, PaddingBottom = 5f, PaddingTop = 4f });
                        loanApplicationheader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.Company, fontArial19Bold)) { HorizontalAlignment = 0, Border = 0, PaddingBottom = 2f });
                        loanApplicationheader.AddCell(new PdfPCell(new Phrase("Address: " + userCompanyDetail.mstCompany.Address, fontArial12)) { HorizontalAlignment = 0, Border = 0 });
                        loanApplicationheader.AddCell(new PdfPCell(new Phrase("Contact: " + userCompanyDetail.mstCompany.ContactNumber, fontArial12)) { HorizontalAlignment = 0, Border = 0 });
                        document.Add(loanApplicationheader);

                        // line header
                        PdfPTable lineHeader = new PdfPTable(1);
                        float[] lineHeaderWithCells = new float[] { 100f };
                        lineHeader.SetWidths(lineHeaderWithCells);
                        lineHeader.WidthPercentage = 100;
                        lineHeader.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { Border = 1, Padding = 0f });
                        document.Add(lineHeader);

                        var area = "";
                        var areaQuery = from d in db.mstAreas
                                        where d.Id == Convert.ToInt32(areaId)
                                        select d;

                        if (areaQuery.Any())
                        {
                            area = areaQuery.FirstOrDefault().Area;
                        }

                        PdfPTable titleHeader = new PdfPTable(1);
                        float[] titleHeaderWithCells = new float[] { 100f };
                        titleHeader.SetWidths(titleHeaderWithCells);
                        titleHeader.WidthPercentage = 100;
                        titleHeader.AddCell(new PdfPCell(new Phrase("Daily Release Monitoring", fontArial13Bold)) { Border = 0, PaddingBottom = 5f, PaddingTop = 1f, HorizontalAlignment = 1 });
                        titleHeader.AddCell(new PdfPCell(new Phrase("From " + Convert.ToDateTime(startDate).ToString("MMMM dd, yyyy") + " to " + Convert.ToDateTime(endDate).ToString("MMMM dd, yyyy"), fontArial12)) { Border = 0, PaddingBottom = 5f, PaddingTop = 2f, HorizontalAlignment = 1 });

                        if (areaId.Equals("0"))
                        {
                            titleHeader.AddCell(new PdfPCell(new Phrase("All Areas", fontArial13Bold)) { Border = 0, PaddingBottom = 12f, PaddingTop = 1f, HorizontalAlignment = 1 });
                        }
                        else
                        {
                            titleHeader.AddCell(new PdfPCell(new Phrase(area, fontArial13Bold)) { Border = 0, PaddingBottom = 12f, PaddingTop = 1f, HorizontalAlignment = 1 });
                        }

                        document.Add(titleHeader);

                        PdfPTable loanlData = new PdfPTable(6);
                        float[] loanlDataWithCells = new float[] { 10f, 15f, 30f, 20f, 15f, 10f };
                        loanlData.SetWidths(loanlDataWithCells);
                        loanlData.WidthPercentage = 100;
                        loanlData.AddCell(new PdfPCell(new Phrase("Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Doc No.", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Area", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Net Amount", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Signature", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                        Decimal totalNetAmount = 0;
                        foreach (var loanLine in loanApplications)
                        {
                            totalNetAmount += loanLine.NetAmount;
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.LoanDate, fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.LoanNumber, fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.Applicant, fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.Area, fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.NetAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        }

                        loanlData.AddCell(new PdfPCell(new Phrase("TOTAL", fontArial11Bold)) { Colspan = 4, HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(totalNetAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(" ", fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });

                        document.Add(loanlData);
                        document.Close();

                        byte[] byteInfo = workStream.ToArray();
                        workStream.Write(byteInfo, 0, byteInfo.Length);
                        workStream.Position = 0;

                        return new FileStreamResult(workStream, "application/pdf");

                    }
                    else
                    {
                        return RedirectToAction("Index", "Software");
                    }
                }
                else
                {
                    return RedirectToAction("NotFound", "Software");
                }
            }
            else
            {
                return RedirectToAction("NotFound", "Software");
            }
        }
    }
}