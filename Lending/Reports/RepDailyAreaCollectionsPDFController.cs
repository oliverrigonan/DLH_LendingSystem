﻿using iTextSharp.text;
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
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // GET: RepDailyAreaCollectionsPDF
        public ActionResult dailyAreaCollectionsPDF(String date, String areaId)
        {
            if (date != null && areaId != null)
            {
                var loanLines = from d in db.trnLoanLines
                                where d.trnLoan.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                && d.CollectibleDate == Convert.ToDateTime(date)
                                && d.trnLoan.IsReconstruct == false
                                && d.trnLoan.IsRenew == false
                                && d.trnLoan.IsLocked == true
                                && d.trnLoan.IsLoanReconstruct == false
                                && d.trnLoan.TotalBalanceAmount > 0
                                && d.PaidAmount == 0
                                select d;

                if (loanLines.Any())
                {
                    // PDF settings
                    MemoryStream workStream = new MemoryStream();
                    Rectangle rectangle = new Rectangle(PageSize.A3);
                    Document document = new Document(rectangle, 72, 72, 72, 72);
                    document.SetMargins(30f, 30f, 30f, 30f);
                    PdfWriter.GetInstance(document, workStream).CloseStream = false;

                    // Document Starts
                    document.Open();

                    // Fonts
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

                    // line
                    Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));

                    // user company detail
                    var userCompanyDetail = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d).FirstOrDefault();

                    // image
                    string imagepath = Server.MapPath("~/Images/dlhicon.jpg");
                    Image logo = Image.GetInstance(imagepath);
                    logo.ScalePercent(16f);
                    PdfPCell imageCell = new PdfPCell(logo);

                    // header
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
                    lineHeader.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { Border = 1 });
                    document.Add(lineHeader);

                    var area = "";
                    var areaQuery = from d in db.mstAreas
                                    where d.Id == Convert.ToInt32(areaId)
                                    select d;

                    if (areaQuery.Any())
                    {
                        area = areaQuery.FirstOrDefault().Area;
                    }

                    //  title
                    PdfPTable titleHeader = new PdfPTable(1);
                    float[] titleHeaderWithCells = new float[] { 100f };
                    titleHeader.SetWidths(titleHeaderWithCells);
                    titleHeader.WidthPercentage = 100;
                    titleHeader.AddCell(new PdfPCell(new Phrase(area + " Daily Collection (ACTIVE)", fontArial13Bold)) { Border = 0, PaddingBottom = 5f, PaddingTop = 2f, HorizontalAlignment = 1 });
                    titleHeader.AddCell(new PdfPCell(new Phrase(Convert.ToDateTime(date).ToString("MMMM dd, yyyy") + " - " + Convert.ToDateTime(date).DayOfWeek.ToString(), fontArial12)) { Border = 0, PaddingBottom = 12f, PaddingTop = 2f, HorizontalAlignment = 1 });
                    document.Add(titleHeader);

                    PdfPTable loanLineslData = new PdfPTable(5);
                    float[] loanLineslDataWithCells = new float[] { 26f, 35f, 10f, 14f, 14f };
                    loanLineslData.SetWidths(loanLineslDataWithCells);
                    loanLineslData.WidthPercentage = 100;
                    loanLineslData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanLineslData.AddCell(new PdfPCell(new Phrase("Day Reference", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanLineslData.AddCell(new PdfPCell(new Phrase("Due Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanLineslData.AddCell(new PdfPCell(new Phrase("Collectible", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    loanLineslData.AddCell(new PdfPCell(new Phrase("Paid / Remarks", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                    Decimal totalCollectibles = 0;
                    foreach (var loanLine in loanLines)
                    {
                        var applicant = loanLine.trnLoan.mstApplicant.ApplicantLastName + ", " + loanLine.trnLoan.mstApplicant.ApplicantFirstName + " " + loanLine.trnLoan.mstApplicant.ApplicantMiddleName;
                        loanLineslData.AddCell(new PdfPCell(new Phrase(applicant, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanLineslData.AddCell(new PdfPCell(new Phrase(loanLine.DayReference, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanLineslData.AddCell(new PdfPCell(new Phrase(loanLine.trnLoan.MaturityDate.ToShortDateString(), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanLineslData.AddCell(new PdfPCell(new Phrase(loanLine.CollectibleAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanLineslData.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        totalCollectibles += loanLine.CollectibleAmount;
                    }

                    loanLineslData.AddCell(new PdfPCell(new Phrase("Total Collectibles / Collected", fontArial12Bold)) { HorizontalAlignment = 2, Colspan = 3, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanLineslData.AddCell(new PdfPCell(new Phrase(totalCollectibles.ToString("#,##0.00"), fontArial12Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanLineslData.AddCell(new PdfPCell(new Phrase(" ", fontArial12Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

                    document.Add(loanLineslData);

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