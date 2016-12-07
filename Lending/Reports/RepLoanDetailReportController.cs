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
    public class RepLoanDetailReportController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan detail report
        public ActionResult loanDetailReport(String applicantId, String startDate, String endDate)
        {
            if (applicantId != null)
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
                    PdfPTable loanDetailHeader = new PdfPTable(2);
                    float[] loanDetailHeaderWidthCells = new float[] { 50f, 50f };
                    loanDetailHeader.SetWidths(loanDetailHeaderWidthCells);
                    loanDetailHeader.WidthPercentage = 100;
                    loanDetailHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.Company, fontArial17Bold)) { Border = 0 });
                    loanDetailHeader.AddCell(new PdfPCell(new Phrase("Loan Detail Report", fontArial17Bold)) { Border = 0, HorizontalAlignment = 2 });
                    loanDetailHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.Address, fontArial12)) { Border = 0, PaddingTop = 5f });
                    loanDetailHeader.AddCell(new PdfPCell(new Phrase("From " + startDate + " to " + endDate, fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2, });
                    loanDetailHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.ContactNumber, fontArial12)) { Border = 0, PaddingTop = 5f });
                    loanDetailHeader.AddCell(new PdfPCell(new Phrase("Printed " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt"), fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2 });
                    document.Add(loanDetailHeader);

                    // table loan detail label
                    PdfPTable loanDetailLabel = new PdfPTable(1);
                    float[] loanDetailLabelWidthCells = new float[] { 100f };
                    loanDetailLabel.SetWidths(loanDetailLabelWidthCells);
                    loanDetailLabel.WidthPercentage = 100;
                    loanDetailLabel.AddCell(new PdfPCell(new Phrase("List of Loans", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });
                    document.Add(line);
                    document.Add(loanDetailLabel);

                    // loan applications
                    var loanApplications = from d in db.trnLoanApplications
                                           where d.ApplicantId == Convert.ToInt32(applicantId)
                                           && d.LoanDate >= Convert.ToDateTime(startDate)
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
            else
            {
                return RedirectToAction("NotFound", "Software");
            }
        }
    }
}