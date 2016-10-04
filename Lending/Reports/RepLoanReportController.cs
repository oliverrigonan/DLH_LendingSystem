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
    public class RepLoanReportController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan report
        public ActionResult loanReport(String startDate, String endDate)
        {
            if (startDate != null && endDate != null)
            {
                var loanApplications = from d in db.trnLoanApplications.OrderBy(d => d.LoanDate)
                                       where d.LoanDate >= Convert.ToDateTime(startDate)
                                       && d.LoanDate <= Convert.ToDateTime(endDate)
                                       && d.IsLocked == true
                                       select new Models.TrnLoanApplication
                                       {
                                           Id = d.Id,
                                           LoanNumber = d.LoanNumber,
                                           LoanDate = d.LoanDate.ToShortDateString(),
                                           MaturityDate = d.MaturityDate.ToShortDateString(),
                                           //AccountId = d.AccountId,
                                           //Account = d.mstAccount.Account,
                                           ApplicantId = d.ApplicantId,
                                           Applicant = d.mstApplicant.ApplicantLastName + " " + d.mstApplicant.ApplicantFirstName + ", " + d.mstApplicant.ApplicantMiddleName,
                                           //AreaId = d.AreaId,
                                           //Area = d.mstArea.Area,
                                           Particulars = d.Particulars,
                                           //LoanAmount = d.LoanAmount,
                                           //PaidAmount = d.PaidAmount,
                                           //BalanceAmount = d.BalanceAmount,
                                           //CollectorId = d.CollectorId,
                                           //Collector = d.mstCollector.Collector,
                                           PreparedByUserId = d.PreparedByUserId,
                                           PreparedByUser = d.mstUser.FullName,
                                           IsLocked = d.IsLocked,
                                           CreatedByUserId = d.CreatedByUserId,
                                           CreatedByUser = d.mstUser1.FullName,
                                           CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                           UpdatedByUserId = d.UpdatedByUserId,
                                           UpdatedByUser = d.mstUser2.FullName,
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
                PdfPTable loanApplicationheader = new PdfPTable(2);
                float[] loanApplicationheaderWidthCells = new float[] { 50f, 50f };
                loanApplicationheader.SetWidths(loanApplicationheaderWidthCells);
                loanApplicationheader.WidthPercentage = 100;
                loanApplicationheader.AddCell(new PdfPCell(new Phrase("DLH Incorporated", fontArial17Bold)) { Border = 0 });
                loanApplicationheader.AddCell(new PdfPCell(new Phrase("Loan Report", fontArial17Bold)) { Border = 0, HorizontalAlignment = 2 });
                loanApplicationheader.AddCell(new PdfPCell(new Phrase("Pardo", fontArial12)) { Border = 0, PaddingTop = 5f });
                loanApplicationheader.AddCell(new PdfPCell(new Phrase("Date from " + startDate + " to " + endDate, fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2, });
                loanApplicationheader.AddCell(new PdfPCell(new Phrase("0932-444-1234", fontArial12)) { Border = 0, PaddingTop = 5f });
                loanApplicationheader.AddCell(new PdfPCell(new Phrase("Printed " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt"), fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2 });
                document.Add(loanApplicationheader);

                // table loan data
                PdfPTable loanHeaderLabel = new PdfPTable(1);
                float[] loanHeaderLabelWidthCells = new float[] { 100f };
                loanHeaderLabel.SetWidths(loanHeaderLabelWidthCells);
                loanHeaderLabel.WidthPercentage = 100;
                loanHeaderLabel.AddCell(new PdfPCell(new Phrase("List of Loan Transaction Report", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });

                document.Add(line);
                document.Add(loanHeaderLabel);

                // table loan application data
                PdfPTable loanApplicationData = new PdfPTable(6);
                float[] loanApplicationDataWidthCells = new float[] { 10f, 15f, 30f, 20f, 10f, 15f };
                loanApplicationData.SetWidths(loanApplicationDataWidthCells);
                loanApplicationData.WidthPercentage = 100;
                loanApplicationData.AddCell(new PdfPCell(new Phrase("Loan Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                loanApplicationData.AddCell(new PdfPCell(new Phrase("Loan Number", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                loanApplicationData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                loanApplicationData.AddCell(new PdfPCell(new Phrase("Area", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                loanApplicationData.AddCell(new PdfPCell(new Phrase("Due Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                loanApplicationData.AddCell(new PdfPCell(new Phrase("Amount", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });

                Decimal totalLoanAmount = 0;
                foreach (var loanApplication in loanApplications)
                {
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplication.LoanDate, fontArial12)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplication.LoanNumber, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplication.Applicant, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(" ", /* loanApplication.Area,*/ fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(loanApplication.MaturityDate, fontArial12)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });
                    loanApplicationData.AddCell(new PdfPCell(new Phrase(" ", /* loanApplication.LoanAmount.ToString("#,##0.00"),*/ fontArial12)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });

                    //totalLoanAmount += loanApplication.LoanAmount;
                }

                document.Add(line);
                document.Add(loanApplicationData);

                document.Add(line);

                // table collection lines total data
                PdfPTable loanApplicationTotalData = new PdfPTable(6);
                float[] loanApplicationTotalDataWidthCells = new float[] { 10f, 15f, 30f, 20f, 10f, 15f };
                loanApplicationTotalData.SetWidths(loanApplicationTotalDataWidthCells);
                loanApplicationTotalData.WidthPercentage = 100;
                loanApplicationTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 10f, PaddingBottom = 6f, PaddingLeft = 5f });
                loanApplicationTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 10f, PaddingBottom = 6f, PaddingRight = 5f });
                loanApplicationTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 10f, PaddingBottom = 6f, PaddingRight = 5f });
                loanApplicationTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 10f, PaddingBottom = 6f, PaddingRight = 5f });
                loanApplicationTotalData.AddCell(new PdfPCell(new Phrase("TOTAL", fontArial12Bold)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 10f, PaddingBottom = 6f, PaddingLeft = 5f });
                loanApplicationTotalData.AddCell(new PdfPCell(new Phrase(totalLoanAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 10f, PaddingBottom = 6f, PaddingRight = 5f });
                document.Add(loanApplicationTotalData);

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