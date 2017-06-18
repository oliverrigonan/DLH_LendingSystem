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
    public class RepReturnReleasePDFController : Controller
    {
        private Data.LendingDataContext db = new Data.LendingDataContext();

        public List<Models.TrnReturnRelease> returnReleaseReport(String areaId, String startDate, String endDate)
        {
            if (areaId.Equals("0"))
            {
                var returnRelease = from d in db.trnReturnReleases.OrderByDescending(d => d.Id)
                                    where d.ReturnReleaseDate >= Convert.ToDateTime(startDate)
                                    && d.ReturnReleaseDate <= Convert.ToDateTime(endDate)
                                    && d.IsLocked == true
                                    select new Models.TrnReturnRelease
                                    {
                                        Id = d.Id,
                                        ReturnReleaseNumber = d.ReturnReleaseNumber,
                                        ReturnReleaseDate = d.ReturnReleaseDate.ToShortDateString(),
                                        Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                        LoanId = d.LoanId,
                                        LoanNumber = d.trnLoan.IsLoanApplication == true ? "LN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanRenew == true ? "RN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanReconstruct == true ? "RC-" + d.trnLoan.LoanNumber : " ",
                                        Particulars = d.Particulars,
                                        CollectorStaffId = d.CollectorStaffId,
                                        CollectorStaff = d.mstStaff.Staff,
                                        PreparedByUserId = d.PreparedByUserId,
                                        PreparedByUser = d.mstUser.FullName,
                                        ReturnAmount = d.ReturnAmount,
                                        IsLocked = d.IsLocked,
                                        CreatedByUserId = d.CreatedByUserId,
                                        CreatedByUser = d.mstUser1.FullName,
                                        CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                        UpdatedByUserId = d.UpdatedByUserId,
                                        UpdatedByUser = d.mstUser2.FullName,
                                        UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                                    };

                return returnRelease.ToList();
            }
            else
            {
                var returnRelease = from d in db.trnReturnReleases.OrderByDescending(d => d.Id)
                                    where d.ReturnReleaseDate >= Convert.ToDateTime(startDate)
                                    && d.ReturnReleaseDate <= Convert.ToDateTime(endDate)
                                    && d.trnLoan.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                    && d.IsLocked == true
                                    select new Models.TrnReturnRelease
                                    {
                                        Id = d.Id,
                                        ReturnReleaseNumber = d.ReturnReleaseNumber,
                                        ReturnReleaseDate = d.ReturnReleaseDate.ToShortDateString(),
                                        Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                        LoanId = d.LoanId,
                                        LoanNumber = d.trnLoan.IsLoanApplication == true ? "LN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanRenew == true ? "RN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanReconstruct == true ? "RC-" + d.trnLoan.LoanNumber : " ",
                                        Particulars = d.Particulars,
                                        CollectorStaffId = d.CollectorStaffId,
                                        CollectorStaff = d.mstStaff.Staff,
                                        PreparedByUserId = d.PreparedByUserId,
                                        PreparedByUser = d.mstUser.FullName,
                                        ReturnAmount = d.ReturnAmount,
                                        IsLocked = d.IsLocked,
                                        CreatedByUserId = d.CreatedByUserId,
                                        CreatedByUser = d.mstUser1.FullName,
                                        CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                        UpdatedByUserId = d.UpdatedByUserId,
                                        UpdatedByUser = d.mstUser2.FullName,
                                        UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                                    };

                return returnRelease.ToList();
            }
        }

        public ActionResult returnRelease(String areaId, String startReturnReleaseDate, String endReturnReleaseDate)
        {
            if (startReturnReleaseDate != null && endReturnReleaseDate != null)
            {
                var returnReleases = from d in returnReleaseReport(areaId, startReturnReleaseDate, endReturnReleaseDate)
                                     select new Models.TrnReturnRelease
                                     {
                                         Id = d.Id,
                                         ReturnReleaseNumber = "RL-" + d.ReturnReleaseNumber,
                                         ReturnReleaseDate = d.ReturnReleaseDate,
                                         Applicant = d.Applicant,
                                         LoanId = d.LoanId,
                                         LoanNumber = d.LoanNumber,
                                         Particulars = d.Particulars,
                                         CollectorStaffId = d.CollectorStaffId,
                                         CollectorStaff = d.CollectorStaff,
                                         PreparedByUserId = d.PreparedByUserId,
                                         PreparedByUser = d.PreparedByUser,
                                         ReturnAmount = d.ReturnAmount,
                                         IsLocked = d.IsLocked,
                                         CreatedByUserId = d.CreatedByUserId,
                                         CreatedByUser = d.CreatedByUser,
                                         CreatedDateTime = d.CreatedDateTime,
                                         UpdatedByUserId = d.UpdatedByUserId,
                                         UpdatedByUser = d.UpdatedByUser,
                                         UpdatedDateTime = d.UpdatedDateTime
                                     };

                if (returnReleases.Any())
                {
                    MemoryStream workStream = new MemoryStream();
                    Rectangle rectangle = new Rectangle(612f, 936f).Rotate();
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
                    float[] loanApplicationheaderWidthCells = new float[] { 4f, 100f };
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
                    titleHeader.AddCell(new PdfPCell(new Phrase("Return Release Report", fontArial13Bold)) { Border = 0, PaddingBottom = 5f, PaddingTop = 1f, HorizontalAlignment = 1 });
                    titleHeader.AddCell(new PdfPCell(new Phrase("From " + Convert.ToDateTime(startReturnReleaseDate).ToString("MMMM dd, yyyy") + " to " + Convert.ToDateTime(endReturnReleaseDate).ToString("MMMM dd, yyyy"), fontArial12)) { Border = 0, PaddingBottom = 5f, PaddingTop = 2f, HorizontalAlignment = 1 });

                    if (areaId.Equals("0"))
                    {
                        titleHeader.AddCell(new PdfPCell(new Phrase("All Areas", fontArial13Bold)) { Border = 0, PaddingBottom = 12f, PaddingTop = 1f, HorizontalAlignment = 1 });
                    }
                    else
                    {
                        titleHeader.AddCell(new PdfPCell(new Phrase(area, fontArial13Bold)) { Border = 0, PaddingBottom = 12f, PaddingTop = 1f, HorizontalAlignment = 1 });
                    }

                    document.Add(titleHeader);

                    PdfPTable collectionData = new PdfPTable(7);
                    float[] collectionDataWithCells = new float[] { 7f, 10f, 20f, 10f, 30f, 20f, 15f };
                    collectionData.SetWidths(collectionDataWithCells);
                    collectionData.WidthPercentage = 100;
                    collectionData.AddCell(new PdfPCell(new Phrase("Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Doc No.", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Loan No.", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Particulars", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Collector", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Amount", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                    Decimal totalReturnReleaseAmount = 0;
                    foreach (var returnRelease in returnReleases)
                    {
                        totalReturnReleaseAmount += returnRelease.ReturnAmount;

                        collectionData.AddCell(new PdfPCell(new Phrase(returnRelease.ReturnReleaseDate, fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(returnRelease.ReturnReleaseNumber, fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(returnRelease.Applicant, fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(returnRelease.LoanNumber, fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(returnRelease.Particulars, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(returnRelease.CollectorStaff, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(returnRelease.ReturnAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    }

                    collectionData.AddCell(new PdfPCell(new Phrase("TOTAL", fontArial11Bold)) { Colspan = 6, HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(totalReturnReleaseAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

                    document.Add(collectionData);
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