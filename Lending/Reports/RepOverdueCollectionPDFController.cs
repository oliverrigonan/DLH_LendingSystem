using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Globalization;

namespace Lending.Reports
{
    public class RepOverdueCollectionPDFController : Controller
    {
        private Data.LendingDataContext db = new Data.LendingDataContext();

        public ActionResult overdueCollection(String date, String areaId)
        {
            if (date != null && areaId != null)
            {
                var loanApplications = from d in db.trnLoans.OrderBy(d => d.mstApplicant.ApplicantLastName)
                                       where d.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                       && d.IsReconstructed == false
                                       && d.IsRenewed == false
                                       && d.IsLocked == true
                                       && d.IsLoanReconstruct == true
                                       && d.TotalBalanceAmount > 0
                                       select d;

                if (loanApplications.Any())
                {
                    MemoryStream workStream = new MemoryStream();
                    Rectangle rectangle = new Rectangle(PageSize.A3);
                    Document document = new Document(rectangle, 72, 72, 72, 72);
                    document.SetMargins(30f, 30f, 50f, 20f);
                    PdfWriter.GetInstance(document, workStream).CloseStream = false;

                    document.Open();

                    Font fontArial19Bold = FontFactory.GetFont("Arial", 20, Font.BOLD);
                    Font fontArial17Bold = FontFactory.GetFont("Arial", 17, Font.BOLD);
                    Font fontArial16Bold = FontFactory.GetFont("Arial", 16, Font.BOLD);
                    Font fontArial12Bold = FontFactory.GetFont("Arial", 12, Font.BOLD);
                    Font fontArial13Bold = FontFactory.GetFont("Arial", 13, Font.BOLD);
                    Font fontArial12 = FontFactory.GetFont("Arial", 12);
                    Font fontArial11Bold = FontFactory.GetFont("Arial", 11, Font.BOLD);
                    Font fontArial11 = FontFactory.GetFont("Arial", 11);
                    Font fontArial11ITALIC = FontFactory.GetFont("Arial", 12, Font.ITALIC);
                    Font fontArial10Bold = FontFactory.GetFont("Arial", 10, Font.BOLD);
                    Font fontArial10 = FontFactory.GetFont("Arial", 10);
                    Font fontArial10ITALIC = FontFactory.GetFont("Arial", 10, Font.ITALIC);
                    Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));

                    var userCompanyDetail = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d).FirstOrDefault();

                    string imagepath = Server.MapPath("~/Images/dlhicon.jpg");
                    Image logo = Image.GetInstance(imagepath);
                    logo.ScalePercent(16f);
                    PdfPCell imageCell = new PdfPCell(logo);

                    PdfPTable loanApplicationheader = new PdfPTable(2);
                    float[] loanApplicationheaderWidthCells = new float[] { 7f, 100f };
                    loanApplicationheader.SetWidths(loanApplicationheaderWidthCells);
                    loanApplicationheader.WidthPercentage = 100;
                    loanApplicationheader.AddCell(new PdfPCell(imageCell) { Rowspan = 3, Border = 0, PaddingRight = 10f, PaddingBottom = 5f });
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
                    titleHeader.AddCell(new PdfPCell(new Phrase(area + " OVERDUE - " + Convert.ToDateTime(date).ToString("MMMM", CultureInfo.InvariantCulture).ToUpper(), fontArial13Bold)) { Border = 0, PaddingBottom = 10f, PaddingTop = 2f, HorizontalAlignment = 0 });
                    document.Add(titleHeader);

                    PdfPTable loanData = new PdfPTable(10);
                    float[] loanDataWithCells = new float[] { 26f, 10f, 10f, 7f, 7f, 7f, 7f, 7f, 7f, 22f };
                    loanData.SetWidths(loanDataWithCells);
                    loanData.WidthPercentage = 100;
                    loanData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanData.AddCell(new PdfPCell(new Phrase("Balance", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanData.AddCell(new PdfPCell(new Phrase("Collectible", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    DateTime weekFirstDay = Convert.ToDateTime(date).AddDays(DayOfWeek.Sunday - Convert.ToDateTime(date).DayOfWeek);
                    for (var i = 1; i <= 6; i++)
                    {
                        DateTime weekLastDay = weekFirstDay.AddDays(i);
                        loanData.AddCell(new PdfPCell(new Phrase(weekLastDay.Day.ToString(), fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    }
                    loanData.AddCell(new PdfPCell(new Phrase("Particulars", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    var loanYears = loanApplications.GroupBy(year => year.LoanDate.Year).Select(group =>
                            new
                            {
                                Name = group.Key,
                                Elements = group.OrderByDescending(y => y.LoanDate.Year)
                            }
                        ).OrderByDescending(group => group.Elements.First().LoanDate.Year);
                    if (loanYears.Any())
                    {
                        foreach (var loanYear in loanYears)
                        {
                            loanData.AddCell(new PdfPCell(new Phrase(loanYear.Elements.First().LoanDate.Year.ToString(), fontArial13Bold)) { Colspan = 10, PaddingTop = 10f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

                            foreach (var loanApplication in loanApplications)
                            {
                                if (loanApplication.LoanDate.Year == loanYear.Elements.First().LoanDate.Year)
                                {
                                    var applicant = loanApplication.mstApplicant.ApplicantLastName + ", " + loanApplication.mstApplicant.ApplicantFirstName + " " + loanApplication.mstApplicant.ApplicantMiddleName;
                                    loanData.AddCell(new PdfPCell(new Phrase(applicant, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                                    loanData.AddCell(new PdfPCell(new Phrase(loanApplication.TotalBalanceAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                                    loanData.AddCell(new PdfPCell(new Phrase(loanApplication.CollectibleAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });

                                    for (var i = 1; i <= 6; i++)
                                    {
                                        loanData.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                                    }

                                    loanData.AddCell(new PdfPCell(new Phrase(loanApplication.Particulars, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                                }
                            }
                        }
                    }
                    document.Add(loanData);

                    // Document End
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