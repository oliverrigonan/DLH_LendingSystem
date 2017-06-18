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
    public class RepDailyCollectionRemittancePDFController : Controller
    {
        private Data.LendingDataContext db = new Data.LendingDataContext();

        public ActionResult dailyCollectionRemittance(String startCollectionDate, String endCollectionDate)
        {
            if (startCollectionDate != null && endCollectionDate != null)
            {
                var dailyCollectionRemittanceLists = from d in db.mstAreas.OrderBy(d => d.Area)
                                                     join c in db.trnCollections
                                                     on d.Id equals c.trnLoan.mstApplicant.AreaId
                                                     into joinAreaCollections
                                                     from listAreaCollections in joinAreaCollections.DefaultIfEmpty().GroupBy(c => c.trnLoan.mstApplicant.AreaId)
                                                     join r in db.trnRemittances
                                                     on d.Id equals r.AreaId
                                                     into joinAreaRemittances
                                                     from listAreaRemittances in joinAreaRemittances.DefaultIfEmpty().GroupBy(r => r.AreaId)
                                                     select new Models.RepDailyCollectionRemittance
                                                     {
                                                         Area = d.Area,
                                                         GrossCollection = joinAreaCollections.Where(c => c.CollectionDate >= Convert.ToDateTime(startCollectionDate) && c.CollectionDate <= Convert.ToDateTime(endCollectionDate) && c.IsLocked == true).Sum(c => c.TotalPaidAmount) != null ? joinAreaCollections.Where(c => c.CollectionDate >= Convert.ToDateTime(startCollectionDate) && c.CollectionDate <= Convert.ToDateTime(endCollectionDate) && c.IsLocked == true).Sum(c => c.TotalPaidAmount) : 0,
                                                         NetRemitted = joinAreaRemittances.Where(r => r.RemittanceDate >= Convert.ToDateTime(startCollectionDate) && r.RemittanceDate <= Convert.ToDateTime(endCollectionDate) && r.IsLocked == true).Sum(r => r.RemitAmount) != null ? joinAreaRemittances.Where(r => r.RemittanceDate >= Convert.ToDateTime(startCollectionDate) && r.RemittanceDate <= Convert.ToDateTime(endCollectionDate) && r.IsLocked == true).Sum(r => r.RemitAmount) : 0,
                                                         Remarks = " "
                                                     };

                if (dailyCollectionRemittanceLists.Any())
                {
                    MemoryStream workStream = new MemoryStream();
                    Rectangle rectangle = new Rectangle(612f, 936f).Rotate();
                    Document document = new Document(rectangle, 72, 72, 72, 72);
                    document.SetMargins(30f, 30f, 30f, 30f);
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
                    Font fontArial11Bold = FontFactory.GetFont("Arial", 10, Font.BOLD);
                    Font fontArial11 = FontFactory.GetFont("Arial", 10);
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

                    PdfPTable titleHeader = new PdfPTable(1);
                    float[] titleHeaderWithCells = new float[] { 100f };
                    titleHeader.SetWidths(titleHeaderWithCells);
                    titleHeader.WidthPercentage = 100;

                    titleHeader.AddCell(new PdfPCell(new Phrase("Daily Collection and Remittance Report", fontArial13Bold)) { Border = 0, PaddingBottom = 5f, PaddingTop = 1f, HorizontalAlignment = 1 });
                    titleHeader.AddCell(new PdfPCell(new Phrase("From " + Convert.ToDateTime(startCollectionDate).ToString("MMMM dd, yyyy") + " to " + Convert.ToDateTime(endCollectionDate).ToString("MMMM dd, yyyy"), fontArial12)) { Border = 0, PaddingBottom = 12f, PaddingTop = 2f, HorizontalAlignment = 1 });
                    document.Add(titleHeader);

                    PdfPTable collectionData = new PdfPTable(5);
                    float[] collectionDataWithCells = new float[] { 30f, 20f, 20f, 15f, 15f };
                    collectionData.SetWidths(collectionDataWithCells);
                    collectionData.WidthPercentage = 100;
                    collectionData.AddCell(new PdfPCell(new Phrase("Area", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Gross Collection", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Amount Remitted", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Remarks", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Signature", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                    Decimal totalGross = 0;
                    Decimal totalRemitted = 0;
                    foreach (var dailyCollectionRemittanceList in dailyCollectionRemittanceLists.OrderBy(d => d.Area))
                    {
                        totalGross += dailyCollectionRemittanceList.GrossCollection;
                        totalRemitted += dailyCollectionRemittanceList.NetRemitted;

                        collectionData.AddCell(new PdfPCell(new Phrase(dailyCollectionRemittanceList.Area, fontArial11Bold)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(dailyCollectionRemittanceList.GrossCollection.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(dailyCollectionRemittanceList.NetRemitted.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });

                        if (dailyCollectionRemittanceList.NetRemitted > dailyCollectionRemittanceList.GrossCollection)
                        {
                            Decimal diff = dailyCollectionRemittanceList.NetRemitted - dailyCollectionRemittanceList.GrossCollection;
                            collectionData.AddCell(new PdfPCell(new Phrase("Over " + diff.ToString("#,##0.00"), fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        }
                        else
                        {
                            if (dailyCollectionRemittanceList.NetRemitted < dailyCollectionRemittanceList.GrossCollection)
                            {
                                Decimal diff = dailyCollectionRemittanceList.GrossCollection - dailyCollectionRemittanceList.NetRemitted;
                                collectionData.AddCell(new PdfPCell(new Phrase("Less " + diff.ToString("#,##0.00"), fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                            }
                            else
                            {
                                collectionData.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                            }
                        }

                        collectionData.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    }

                    collectionData.AddCell(new PdfPCell(new Phrase("TOTAL GROSS COLLECTION", fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(totalGross.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(totalRemitted.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { Colspan = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

                    var vaults = from d in db.trnVaults
                                 where d.VaultDate >= Convert.ToDateTime(startCollectionDate)
                                 && d.VaultDate <= Convert.ToDateTime(startCollectionDate)
                                 && d.IsLocked == true
                                 select d;

                    Decimal addMoneyAmount = 0;
                    if (vaults.Any())
                    {
                        addMoneyAmount = vaults.Sum(d => d.Amount);
                    }

                    collectionData.AddCell(new PdfPCell(new Phrase("ADD MONEY FROM VAULT", fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(addMoneyAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase("Remarks / Particulars", fontArial11Bold)) { Rowspan = 5, Colspan = 3, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

                    var returnRelease = from d in db.trnReturnReleases
                                        where d.ReturnReleaseDate >= Convert.ToDateTime(startCollectionDate)
                                        && d.ReturnReleaseDate <= Convert.ToDateTime(startCollectionDate)
                                        && d.IsLocked == true
                                        select d;

                    Decimal totalReturnRelease = 0;
                    if (returnRelease.Any())
                    {
                        totalReturnRelease = returnRelease.Sum(d => d.ReturnAmount);
                    }

                    collectionData.AddCell(new PdfPCell(new Phrase("RETURN RELEASE", fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(totalReturnRelease.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

                    var expenses = from d in db.trnExpenses
                                   where d.ExpenseDate >= Convert.ToDateTime(startCollectionDate)
                                   && d.ExpenseDate <= Convert.ToDateTime(startCollectionDate)
                                   && d.IsLocked == true
                                   select d;

                    Decimal totalExpenses = 0;
                    if (expenses.Any())
                    {
                        totalExpenses = vaults.Sum(d => d.Amount);
                    }

                    collectionData.AddCell(new PdfPCell(new Phrase("TOTAL EXPENSES", fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(totalExpenses.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

                    var loan = from d in db.trnLoans
                               where d.LoanDate >= Convert.ToDateTime(startCollectionDate)
                               && d.LoanDate <= Convert.ToDateTime(endCollectionDate)
                               && d.IsLocked == true
                               && d.IsLoanReconstruct != true
                               select d;

                    Decimal totalRelease = 0;
                    if (loan.Any())
                    {
                        totalRelease = loan.Sum(d => d.NetAmount);
                    }

                    collectionData.AddCell(new PdfPCell(new Phrase("TOTAL RELEASE", fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(totalRelease.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

                    Decimal collectionBalance = (totalGross + addMoneyAmount + totalReturnRelease) - (totalRelease + totalExpenses);
                    collectionData.AddCell(new PdfPCell(new Phrase("COLLECTION BALANCE", fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(collectionBalance.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

                    document.Add(collectionData);
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