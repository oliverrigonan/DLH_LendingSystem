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
    public class RepExpensesSummaryReportController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // expenses summary report
        public ActionResult expensesSummaryReport(String startDate, String endDate)
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
                PdfPTable expenseSummaryReportHeader = new PdfPTable(2);
                float[] expenseSummaryReportHeaderWidthCells = new float[] { 50f, 50f };
                expenseSummaryReportHeader.SetWidths(expenseSummaryReportHeaderWidthCells);
                expenseSummaryReportHeader.WidthPercentage = 100;
                expenseSummaryReportHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.Company, fontArial17Bold)) { Border = 0 });
                expenseSummaryReportHeader.AddCell(new PdfPCell(new Phrase("Expenses Summary Report", fontArial17Bold)) { Border = 0, HorizontalAlignment = 2 });
                expenseSummaryReportHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.Address, fontArial12)) { Border = 0, PaddingTop = 5f });
                expenseSummaryReportHeader.AddCell(new PdfPCell(new Phrase("From " + startDate + " to " + endDate, fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2, });
                expenseSummaryReportHeader.AddCell(new PdfPCell(new Phrase(userCompanyDetail.mstCompany.ContactNumber, fontArial12)) { Border = 0, PaddingTop = 5f });
                expenseSummaryReportHeader.AddCell(new PdfPCell(new Phrase("Printed " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt"), fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2 });
                document.Add(expenseSummaryReportHeader);

                // table loan label
                PdfPTable expenseSummaryReportLabel = new PdfPTable(1);
                float[] expenseSummaryReportLabelWidthCells = new float[] { 100f };
                expenseSummaryReportLabel.SetWidths(expenseSummaryReportLabelWidthCells);
                expenseSummaryReportLabel.WidthPercentage = 100;
                expenseSummaryReportLabel.AddCell(new PdfPCell(new Phrase("List of Expenses Activities", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });
                document.Add(line);
                document.Add(expenseSummaryReportLabel);

                // expenses
                var expenses = from d in db.trnExpenses
                               where d.ExpenseDate >= Convert.ToDateTime(startDate)
                               && d.ExpenseDate <= Convert.ToDateTime(endDate)
                               && d.IsLocked == true
                               select new Models.TrnExpenses
                               {
                                   Id = d.Id,
                                   ExpenseNumber = d.ExpenseNumber,
                                   ExpenseDate = d.ExpenseDate.ToShortDateString(),
                                   AccountId = d.AccountId,
                                   Account = d.mstAccount.Account,
                                   CollectorStaffId = d.CollectorStaffId,
                                   CollectorStaff = d.mstStaff.Staff,
                                   ExpenseTypeId = d.ExpenseTypeId,
                                   ExpenseType = d.mstExpenseType.ExpenseType,
                                   ExpenseTransactionTypeId = d.ExpenseTransactionTypeId,
                                   ExpenseTransactionType = d.sysTransactionType.TransactionType,
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
                    PdfPTable spaceLabel = new PdfPTable(1);
                    float[] spaceLabelWithCells = new float[] { 100f };
                    spaceLabel.SetWidths(spaceLabelWithCells);
                    spaceLabel.WidthPercentage = 100;
                    spaceLabel.AddCell(new PdfPCell(new Phrase(" ")) { HorizontalAlignment = 0, Border = 0, PaddingTop = 5f, PaddingBottom = 5f });
                    document.Add(spaceLabel);

                    PdfPTable expenseSummaryReportActivities = new PdfPTable(6);
                    float[] expenseSummaryReportActivitiesWithCells = new float[] { 12f, 12f, 20f, 16f, 25f, 15f, };
                    expenseSummaryReportActivities.SetWidths(expenseSummaryReportActivitiesWithCells);
                    expenseSummaryReportActivities.WidthPercentage = 100;
                    expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Expense No.", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Expense Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Staff / Collector", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Type", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Particulars", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Amount", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                    Decimal totalExpenseAmount = 0;
                    foreach (var expense in expenses)
                    {
                        totalExpenseAmount += expense.ExpenseAmount;

                        expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase(expense.ExpenseNumber, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                        expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase(expense.ExpenseDate, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                        expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase(expense.CollectorStaff, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                        expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase(expense.ExpenseType, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                        expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase(expense.Particulars, fontArial11)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 5f });
                        expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase(expense.ExpenseAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f });
                    }

                    expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase("Total", fontArial11Bold)) { Colspan = 5, HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    expenseSummaryReportActivities.AddCell(new PdfPCell(new Phrase(totalExpenseAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                    document.Add(expenseSummaryReportActivities);
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