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
    public class RepDisbursementDetailController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // disbursement detail
        public ActionResult disbursementDetail(Int32? disbursementId)
        {
            if (disbursementId != null)
            {
                var disbursementIsLocked = from d in db.trnDisbursements where d.Id == disbursementId select d;
                if (disbursementIsLocked.Any())
                {
                    if (disbursementIsLocked.FirstOrDefault().IsLocked)
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

                        // line
                        Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));

                        // table main header
                        PdfPTable disbursementHeader = new PdfPTable(2);
                        float[] disbursementHeaderWidthCells = new float[] { 50f, 50f };
                        disbursementHeader.SetWidths(disbursementHeaderWidthCells);
                        disbursementHeader.WidthPercentage = 100;
                        disbursementHeader.AddCell(new PdfPCell(new Phrase("DLH Incorporated", fontArial17Bold)) { Border = 0 });
                        disbursementHeader.AddCell(new PdfPCell(new Phrase("Disbursement Detail Report", fontArial17Bold)) { Border = 0, HorizontalAlignment = 2 });
                        disbursementHeader.AddCell(new PdfPCell(new Phrase("Pardo", fontArial12)) { Border = 0, PaddingTop = 5f });
                        disbursementHeader.AddCell(new PdfPCell(new Phrase("Pardo Branch", fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2, });
                        disbursementHeader.AddCell(new PdfPCell(new Phrase("0932-444-1234", fontArial12)) { Border = 0, PaddingTop = 5f });
                        disbursementHeader.AddCell(new PdfPCell(new Phrase("Printed " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt"), fontArial12)) { Border = 0, PaddingTop = 5f, HorizontalAlignment = 2 });
                        document.Add(disbursementHeader);

                        // table disbursement lines data
                        PdfPTable disbursementHeaderLabel = new PdfPTable(1);
                        float[] disbursementHeaderLabelWidthCells = new float[] { 100f };
                        disbursementHeaderLabel.SetWidths(disbursementHeaderLabelWidthCells);
                        disbursementHeaderLabel.WidthPercentage = 100;
                        disbursementHeaderLabel.AddCell(new PdfPCell(new Phrase("Header Detail", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });

                        document.Add(line);
                        document.Add(disbursementHeaderLabel);

                        // queries
                        var disbursements = from d in db.trnDisbursements where d.Id == disbursementId select d;
                        if (disbursements.Any())
                        {
                            // table data
                            PdfPTable disbursementData = new PdfPTable(4);
                            float[] disbursementDataWidthCells = new float[] { 15f, 40, 20f, 25f };
                            disbursementData.SetWidths(disbursementDataWidthCells);
                            disbursementData.WidthPercentage = 100;
                            disbursementData.AddCell(new PdfPCell(new Phrase("Payee", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase(disbursements.FirstOrDefault().Payee, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase("Disbursement Number", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase(disbursements.FirstOrDefault().DisbursementNumber, fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 15f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase("Branch", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase(disbursements.FirstOrDefault().mstBranch.Branch, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase("Disbursement Date", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase(disbursements.FirstOrDefault().DisbursementDate.ToLongDateString(), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase("Pay Type", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase(disbursements.FirstOrDefault().mstPayType.PayType, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase("", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase("Particulars", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase(disbursements.FirstOrDefault().Particulars, fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase("", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementData.AddCell(new PdfPCell(new Phrase("", fontArial12)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            document.Add(disbursementData);

                            document.Add(Chunk.NEWLINE);

                            // table collection lines data
                            PdfPTable loanHeader2Label = new PdfPTable(1);
                            float[] loanHeaderLabel2WidthCells = new float[] { 100f };
                            loanHeader2Label.SetWidths(loanHeaderLabel2WidthCells);
                            loanHeader2Label.WidthPercentage = 100;
                            loanHeader2Label.AddCell(new PdfPCell(new Phrase("Disburse Amount", fontArial12White)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 9f, PaddingLeft = 5f, BackgroundColor = BaseColor.BLACK });

                            document.Add(line);
                            document.Add(loanHeader2Label);

                            PdfPTable disbursementAmount = new PdfPTable(2);
                            float[] disbursementAmountWidthCells = new float[] { 80f, 20f };
                            disbursementAmount.SetWidths(disbursementAmountWidthCells);
                            disbursementAmount.WidthPercentage = 100;
                            disbursementAmount.AddCell(new PdfPCell(new Phrase("DISBURSE AMOUNT", fontArial12Bold)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 15f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            disbursementAmount.AddCell(new PdfPCell(new Phrase(disbursements.FirstOrDefault().Amount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 15f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            document.Add(disbursementAmount);

                            document.Add(Chunk.NEWLINE);

                            document.Add(line);

                            document.Add(Chunk.NEWLINE);
                            document.Add(Chunk.NEWLINE);
                            document.Add(Chunk.NEWLINE);
                            document.Add(Chunk.NEWLINE);
                            document.Add(Chunk.NEWLINE);

                            // Table for Footer
                            PdfPTable tableFooter = new PdfPTable(3);
                            tableFooter.WidthPercentage = 100;
                            float[] widthsCells2 = new float[] { 40, 20, 40 };
                            tableFooter.SetWidths(widthsCells2);
                            tableFooter.AddCell(new PdfPCell(new Phrase("Prepared by:", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingLeft = 5f, PaddingRight = 5f });
                            tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0 });
                            tableFooter.AddCell(new PdfPCell(new Phrase("Verified by:", fontArial12Bold)) { Border = 0, HorizontalAlignment = 0, PaddingLeft = 5f, PaddingRight = 5f });

                            tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 10f, PaddingBottom = 10f });
                            tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 10f, PaddingBottom = 10f });
                            tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 10f, PaddingBottom = 10f });

                            tableFooter.AddCell(new PdfPCell(new Phrase(disbursements.FirstOrDefault().mstUser.FullName)) { Border = 1, HorizontalAlignment = 1, PaddingBottom = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            tableFooter.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingBottom = 5f });
                            tableFooter.AddCell(new PdfPCell(new Phrase(disbursements.FirstOrDefault().mstUser1.FullName)) { Border = 1, HorizontalAlignment = 1, PaddingBottom = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                            document.Add(tableFooter);
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
                        return RedirectToAction("DisbursementList", "Software");
                    }
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