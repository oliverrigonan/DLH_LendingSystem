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
    public class RepDisbursementReportController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // disbursement report
        public ActionResult disbursementReport(String startDate, String endDate)
        {
            if (startDate != null && endDate != null)
            {
                var disbursements = from d in db.trnDisbursements.OrderBy(d => d.DisbursementDate)
                                    where d.DisbursementDate >= Convert.ToDateTime(startDate)
                                    && d.DisbursementDate <= Convert.ToDateTime(endDate)
                                    && d.IsLocked == true
                                    select new Models.TrnDisbursement
                                    {
                                        Id = d.Id,
                                        DisbursementNumber = d.DisbursementNumber,
                                        DisbursementDate = d.DisbursementDate.ToShortDateString(),
                                        AccountId = d.AccountId,
                                        Account = d.mstAccount.Account,
                                        Payee = d.Payee,
                                        Particulars = d.Particulars,
                                        Amount = d.Amount,
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
                PdfPTable disbursementheader = new PdfPTable(2);
                float[] disbursementheaderWidthCells = new float[] { 50f, 50f };
                disbursementheader.SetWidths(disbursementheaderWidthCells);
                disbursementheader.WidthPercentage = 100;
                disbursementheader.AddCell(new PdfPCell(new Phrase("DLH Incorporated", fontArial17Bold)) { Border = 0 });
                disbursementheader.AddCell(new PdfPCell(new Phrase("Disbursement Report (Expenses)", fontArial17Bold)) { Border = 0, HorizontalAlignment = 2 });
                disbursementheader.AddCell(new PdfPCell(new Phrase("Pardo", fontArial12)) { Border = 0, PaddingTop = 5f });
                disbursementheader.AddCell(new PdfPCell(new Phrase("Date from " + startDate + " to " + endDate, fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2, });
                disbursementheader.AddCell(new PdfPCell(new Phrase("0932-444-1234", fontArial12)) { Border = 0, PaddingTop = 5f });
                disbursementheader.AddCell(new PdfPCell(new Phrase("Printed " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt"), fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2 });
                document.Add(disbursementheader);

                // table data label
                PdfPTable disbursementheaderLabel = new PdfPTable(1);
                float[] disbursementheaderLabelWidthCells = new float[] { 100f };
                disbursementheaderLabel.SetWidths(disbursementheaderLabelWidthCells);
                disbursementheaderLabel.WidthPercentage = 100;
                disbursementheaderLabel.AddCell(new PdfPCell(new Phrase("List of Disbursement", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });

                document.Add(line);
                document.Add(disbursementheaderLabel);

                // table data
                PdfPTable disbursementData = new PdfPTable(5);
                float[] disbursementDataWidthCells = new float[] { 17f, 20f, 20f, 28f, 15f };
                disbursementData.SetWidths(disbursementDataWidthCells);
                disbursementData.WidthPercentage = 100;
                disbursementData.AddCell(new PdfPCell(new Phrase("Disbursement Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                disbursementData.AddCell(new PdfPCell(new Phrase("Disbursement Number", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                disbursementData.AddCell(new PdfPCell(new Phrase("Payee", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                disbursementData.AddCell(new PdfPCell(new Phrase("Particulars", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });
                disbursementData.AddCell(new PdfPCell(new Phrase("Amount", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, BackgroundColor = BaseColor.LIGHT_GRAY });

                Decimal totalDisburseAmount = 0;
                foreach (var disbursement in disbursements)
                {
                    disbursementData.AddCell(new PdfPCell(new Phrase(disbursement.DisbursementDate, fontArial12)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });
                    disbursementData.AddCell(new PdfPCell(new Phrase(disbursement.DisbursementNumber, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                    disbursementData.AddCell(new PdfPCell(new Phrase(disbursement.Payee, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });
                    disbursementData.AddCell(new PdfPCell(new Phrase(disbursement.Particulars, fontArial12)) { HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f });
                    disbursementData.AddCell(new PdfPCell(new Phrase(disbursement.Amount.ToString("#,##0.00"), fontArial12)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingRight = 5f });

                    totalDisburseAmount += disbursement.Amount;
                }

                document.Add(line);
                document.Add(disbursementData);

                document.Add(line);

                // table total data
                PdfPTable disbursementTotalData = new PdfPTable(5);
                float[] disbursementTotalDataWidthCells = new float[] { 17f, 20f, 20f, 28f, 15f };
                disbursementTotalData.SetWidths(disbursementTotalDataWidthCells);
                disbursementTotalData.WidthPercentage = 100;
                disbursementTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 10f, PaddingBottom = 6f, PaddingLeft = 5f });
                disbursementTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 10f, PaddingBottom = 6f, PaddingRight = 5f });
                disbursementTotalData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 10f, PaddingBottom = 6f, PaddingRight = 5f });
                disbursementTotalData.AddCell(new PdfPCell(new Phrase("TOTAL", fontArial12Bold)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 10f, PaddingBottom = 6f, PaddingLeft = 5f });
                disbursementTotalData.AddCell(new PdfPCell(new Phrase(totalDisburseAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 10f, PaddingBottom = 6f, PaddingRight = 5f });
                document.Add(disbursementTotalData);

                // Document End
                document.Close();

                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;

                return new FileStreamResult(workStream, "application/pdf");
            }
            else
            {
                return RedirectToAction("LoanApplicationList", "Software");
            }
        }
    }
}