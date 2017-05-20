using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Lending.Reports
{
    public class RepExpensesPDFController : Controller
    {
        private Data.LendingDataContext db = new Data.LendingDataContext();

        public ActionResult expensesPDF(String startExpenseDate, String endExpensesDate)
        {
            if (startExpenseDate != null && endExpensesDate != null)
            {
                var expenses = from d in db.trnExpenses.OrderByDescending(d => d.Id)
                               where d.ExpenseDate >= Convert.ToDateTime(startExpenseDate)
                               && d.ExpenseDate <= Convert.ToDateTime(endExpensesDate)
                               select new Models.TrnExpenses
                               {
                                   Id = d.Id,
                                   ExpenseNumber = "EX-" + d.ExpenseNumber,
                                   ExpenseDate = d.ExpenseDate.ToShortDateString(),
                                   AssignedStaffId = d.AssignedStaffId,
                                   AssignedStaff = d.mstStaff.Staff,
                                   Particulars = d.Particulars,
                                   ExpenseAmount = d.ExpenseAmount,
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

                if (expenses.Any())
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

                    PdfPTable titleHeader = new PdfPTable(1);
                    float[] titleHeaderWithCells = new float[] { 100f };
                    titleHeader.SetWidths(titleHeaderWithCells);
                    titleHeader.WidthPercentage = 100;
                    titleHeader.AddCell(new PdfPCell(new Phrase("Expenses Report", fontArial13Bold)) { Border = 0, PaddingBottom = 5f, PaddingTop = 1f, HorizontalAlignment = 1 });
                    titleHeader.AddCell(new PdfPCell(new Phrase("From " + Convert.ToDateTime(startExpenseDate).ToString("MMMM dd, yyyy") + " to " + Convert.ToDateTime(endExpensesDate).ToString("MMMM dd, yyyy"), fontArial12)) { Border = 0, PaddingBottom = 12f, PaddingTop = 2f, HorizontalAlignment = 1 });
                    document.Add(titleHeader);

                    PdfPTable collectionData = new PdfPTable(5);
                    float[] collectionDataWithCells = new float[] { 10f, 15f, 25f, 30f, 20f };
                    collectionData.SetWidths(collectionDataWithCells);
                    collectionData.WidthPercentage = 100;
                    collectionData.AddCell(new PdfPCell(new Phrase("Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Doc No.", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Staff", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Particulars", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    collectionData.AddCell(new PdfPCell(new Phrase("Amount", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                    Decimal totalExpenseAmount = 0;
                    foreach (var expense in expenses)
                    {
                        totalExpenseAmount += expense.ExpenseAmount;

                        collectionData.AddCell(new PdfPCell(new Phrase(expense.ExpenseDate, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(expense.ExpenseNumber, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(expense.AssignedStaff, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(expense.Particulars, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(expense.ExpenseAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    }

                    collectionData.AddCell(new PdfPCell(new Phrase("TOTAL", fontArial11Bold)) { Colspan = 4, HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                    collectionData.AddCell(new PdfPCell(new Phrase(totalExpenseAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

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