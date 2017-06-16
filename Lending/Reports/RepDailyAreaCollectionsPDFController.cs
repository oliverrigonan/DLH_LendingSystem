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
    public class RepDailyAreaCollectionsPDFController : Controller
    {
        private Data.LendingDataContext db = new Data.LendingDataContext();

        public List<Models.TrnLoan> loanApplicationsList(String date, String areaId)
        {
            if (areaId.Equals("0"))
            {
                var loanApplications = from d in db.trnLoans
                                       where d.IsLocked == true
                                       && d.TotalBalanceAmount > 0
                                       && d.IsReturnRelease == false
                                       select new Models.TrnLoan
                                       {
                                           ApplicantId = d.ApplicantId,
                                           Id = d.Id,
                                           Applicant = d.mstApplicant.ApplicantLastName + " " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                           LoanNumber = d.LoanNumber,
                                           DateTImeMaturityDate = d.MaturityDate,
                                           TotalBalanceAmount = d.TotalBalanceAmount,
                                           CollectibleAmount = d.CollectibleAmount,
                                           IsLoanRenew = d.IsLoanRenew,
                                           IsLoanReconstruct = d.IsLoanReconstruct,
                                           IsReturnRelease = d.IsReturnRelease,
                                           IsBlocked = d.mstApplicant.IsBlocked
                                       };

                var grouploanApplications = from d in loanApplications.OrderByDescending(d => d.Id)
                                            group d by d.ApplicantId into g
                                            select new Models.TrnLoan
                                            {
                                                ApplicantId = g.FirstOrDefault().ApplicantId,
                                                Id = g.FirstOrDefault().Id,
                                                Applicant = g.FirstOrDefault().Applicant,
                                                LoanNumber = g.FirstOrDefault().LoanNumber,
                                                DateTImeMaturityDate = g.FirstOrDefault().DateTImeMaturityDate,
                                                TotalBalanceAmount = g.FirstOrDefault().TotalBalanceAmount,
                                                CollectibleAmount = g.FirstOrDefault().CollectibleAmount,
                                                IsLoanRenew = g.FirstOrDefault().IsLoanRenew,
                                                IsLoanReconstruct = g.FirstOrDefault().IsLoanReconstruct,
                                                IsReturnRelease = g.FirstOrDefault().IsReturnRelease,
                                                IsBlocked = g.FirstOrDefault().IsBlocked,
                                            };

                var loanApplicationList = from d in grouploanApplications.OrderByDescending(d => d.Id)
                                          where d.IsLoanReconstruct == false
                                          && d.IsBlocked == false
                                          select new Models.TrnLoan
                                          {
                                              ApplicantId = d.ApplicantId,
                                              Id = d.Id,
                                              Applicant = d.Applicant,
                                              LoanNumber = d.LoanNumber,
                                              MaturityDate = d.DateTImeMaturityDate.ToShortDateString(),
                                              TotalBalanceAmount = d.TotalBalanceAmount,
                                              CollectibleAmount = d.CollectibleAmount,
                                              IsLoanRenew = d.IsLoanRenew,
                                              IsLoanReconstruct = d.IsLoanReconstruct,
                                              IsReturnRelease = d.IsReturnRelease,
                                              IsBlocked = d.IsBlocked
                                          };

                return loanApplicationList.OrderBy(d => d.Applicant).ToList();
            }
            else
            {
                var loanApplications = from d in db.trnLoans
                                       where d.IsLocked == true
                                       && d.TotalBalanceAmount > 0
                                       && d.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                       && d.IsReturnRelease == false
                                       select new Models.TrnLoan
                                       {
                                           ApplicantId = d.ApplicantId,
                                           Id = d.Id,
                                           Applicant = d.mstApplicant.ApplicantLastName + " " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                           LoanNumber = d.LoanNumber,
                                           DateTImeMaturityDate = d.MaturityDate,
                                           TotalBalanceAmount = d.TotalBalanceAmount,
                                           CollectibleAmount = d.CollectibleAmount,
                                           IsLoanRenew = d.IsLoanRenew,
                                           IsLoanReconstruct = d.IsLoanReconstruct,
                                           IsReturnRelease = d.IsReturnRelease,
                                           IsBlocked = d.mstApplicant.IsBlocked
                                       };

                var grouploanApplications = from d in loanApplications.OrderByDescending(d => d.Id)
                                            group d by d.ApplicantId into g
                                            select new Models.TrnLoan
                                            {
                                                ApplicantId = g.FirstOrDefault().ApplicantId,
                                                Id = g.FirstOrDefault().Id,
                                                Applicant = g.FirstOrDefault().Applicant,
                                                LoanNumber = g.FirstOrDefault().LoanNumber,
                                                DateTImeMaturityDate = g.FirstOrDefault().DateTImeMaturityDate,
                                                TotalBalanceAmount = g.FirstOrDefault().TotalBalanceAmount,
                                                CollectibleAmount = g.FirstOrDefault().CollectibleAmount,
                                                IsLoanRenew = g.FirstOrDefault().IsLoanRenew,
                                                IsLoanReconstruct = g.FirstOrDefault().IsLoanReconstruct,
                                                IsReturnRelease = g.FirstOrDefault().IsReturnRelease,
                                                IsBlocked = g.FirstOrDefault().IsBlocked
                                            };

                var loanApplicationList = from d in grouploanApplications.OrderByDescending(d => d.Id)
                                          where d.IsLoanReconstruct == false
                                          && d.IsBlocked == false
                                          select new Models.TrnLoan
                                          {
                                              ApplicantId = d.ApplicantId,
                                              Id = d.Id,
                                              Applicant = d.Applicant,
                                              LoanNumber = d.LoanNumber,
                                              MaturityDate = d.DateTImeMaturityDate.ToShortDateString(),
                                              TotalBalanceAmount = d.TotalBalanceAmount,
                                              CollectibleAmount = d.CollectibleAmount,
                                              IsLoanRenew = d.IsLoanRenew,
                                              IsReturnRelease = d.IsReturnRelease,
                                              IsBlocked = d.IsBlocked
                                          };

                return loanApplicationList.OrderBy(d => d.Applicant).ToList();
            }
        }

        public ActionResult dailyAreaCollectionsPDF(String date, String areaId)
        {
            if (date != null && areaId != null)
            {
                var loanApplications = from d in loanApplicationsList(date, areaId)
                                       select new Models.TrnLoan
                                       {
                                           ApplicantId = d.ApplicantId,
                                           Id = d.Id,
                                           Applicant = d.Applicant,
                                           LoanNumber = d.LoanNumber,
                                           MaturityDate = d.DateTImeMaturityDate.ToShortDateString(),
                                           TotalBalanceAmount = d.TotalBalanceAmount,
                                           CollectibleAmount = d.CollectibleAmount,
                                           IsLoanRenew = d.IsLoanRenew,
                                           IsLoanReconstruct = d.IsLoanReconstruct,
                                           IsReturnRelease = d.IsReturnRelease,
                                           DateTImeMaturityDate = d.DateTImeMaturityDate
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

                    var areaQuery = from d in db.mstAreas
                                    where d.Id == Convert.ToInt32(areaId)
                                    select d;

                    PdfPTable titleHeader = new PdfPTable(1);
                    float[] titleHeaderWithCells = new float[] { 100f };
                    titleHeader.SetWidths(titleHeaderWithCells);
                    titleHeader.WidthPercentage = 100;

                    if (areaQuery.Any())
                    {
                        titleHeader.AddCell(new PdfPCell(new Phrase(areaQuery.FirstOrDefault().Area + " Daily Collection (ACTIVE)", fontArial13Bold)) { Border = 0, PaddingBottom = 5f, PaddingTop = 1f, HorizontalAlignment = 1 });
                    }
                    else
                    {
                        titleHeader.AddCell(new PdfPCell(new Phrase("Daily Collection in All Areas (ACTIVE)", fontArial13Bold)) { Border = 0, PaddingBottom = 5f, PaddingTop = 1f, HorizontalAlignment = 1 });
                    }

                    titleHeader.AddCell(new PdfPCell(new Phrase(Convert.ToDateTime(date).ToString("MMMM dd, yyyy") + " - " + Convert.ToDateTime(date).DayOfWeek.ToString(), fontArial12)) { Border = 0, PaddingBottom = 12f, PaddingTop = 2f, HorizontalAlignment = 1 });
                    document.Add(titleHeader);

                    PdfPTable loanlData = new PdfPTable(6);
                    float[] loanlDataWithCells = new float[] { 15f, 15f, 15f, 30f, 10f, 25f };
                    loanlData.SetWidths(loanlDataWithCells);
                    loanlData.WidthPercentage = 100;
                    loanlData.AddCell(new PdfPCell(new Phrase("Doc No.", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanlData.AddCell(new PdfPCell(new Phrase("Balance", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanlData.AddCell(new PdfPCell(new Phrase("Collectible", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanlData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanlData.AddCell(new PdfPCell(new Phrase("Due Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanlData.AddCell(new PdfPCell(new Phrase("Paid Amount", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    Decimal totalCollectibles = 0;
                    foreach (var loanApplication in loanApplications)
                    {
                        totalCollectibles += loanApplication.CollectibleAmount;

                        if (loanApplication.IsLoanRenew)
                        {
                            loanlData.AddCell(new PdfPCell(new Phrase("RN-" + loanApplication.LoanNumber, fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        }
                        else
                        {
                            loanlData.AddCell(new PdfPCell(new Phrase("LN-" + loanApplication.LoanNumber, fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        }

                        loanlData.AddCell(new PdfPCell(new Phrase(loanApplication.TotalBalanceAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(loanApplication.CollectibleAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(loanApplication.Applicant, fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(loanApplication.DateTImeMaturityDate.ToShortDateString(), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12Bold)) { HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    }

                    loanlData.AddCell(new PdfPCell(new Phrase("TOTAL", fontArial11Bold)) { HorizontalAlignment = 2, Colspan = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanlData.AddCell(new PdfPCell(new Phrase(totalCollectibles.ToString("#,##0.00"), fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12Bold)) { HorizontalAlignment = 2, Colspan = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

                    var areaStaff = from d in db.mstAreaStaffs.OrderByDescending(d => d.Id)
                                    where d.AreaId == Convert.ToInt32(areaId)
                                    select d;

                    String collector = " ";
                    if (areaStaff.Any())
                    {
                        collector = areaStaff.FirstOrDefault().mstStaff.Staff;
                    }

                    loanlData.AddCell(new PdfPCell(new Phrase("Collector:", fontArial11Bold)) { HorizontalAlignment = 0, PaddingTop = 4f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanlData.AddCell(new PdfPCell(new Phrase(collector, fontArial11)) { HorizontalAlignment = 0, Colspan = 6, PaddingTop = 4f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });

                    loanlData.AddCell(new PdfPCell(new Phrase("Total Active:", fontArial11Bold)) { HorizontalAlignment = 0, PaddingTop = 4f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanlData.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { HorizontalAlignment = 0, Colspan = 2, PaddingTop = 2f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });

                    loanlData.AddCell(new PdfPCell(new Phrase("Remarks:", fontArial11Bold)) { Rowspan = 3, Colspan = 4, HorizontalAlignment = 0, PaddingTop = 4f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });

                    loanlData.AddCell(new PdfPCell(new Phrase("Total Overdue:", fontArial11Bold)) { HorizontalAlignment = 0, PaddingTop = 4f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanlData.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { HorizontalAlignment = 0, Colspan = 2, PaddingTop = 2f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });

                    loanlData.AddCell(new PdfPCell(new Phrase("Overall:", fontArial11Bold)) { HorizontalAlignment = 0, PaddingTop = 4f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanlData.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { HorizontalAlignment = 0, Colspan = 2, PaddingTop = 2f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });

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
    }
}