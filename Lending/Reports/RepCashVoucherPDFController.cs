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
    public class RepCashVoucherPDFController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // GET: RepCashVoucherPDF
        public ActionResult CashVoucherPDF(String loanId)
        {
            if (loanId != null)
            {
                var loan = from d in db.trnLoans
                           where d.Id == Convert.ToInt32(loanId)
                           && d.IsLocked == true
                           select d;

                if (loan.Any())
                {
                    // PDF settings
                    MemoryStream workStream = new MemoryStream();
                    Rectangle rectangle = new Rectangle(612f, 936f);
                    Document document = new Document(rectangle, 72, 72, 72, 72);
                    document.SetMargins(30f, 30f, 20f, 20f);
                    PdfWriter.GetInstance(document, workStream).CloseStream = false;

                    // Document Starts
                    document.Open();

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

                    // line
                    Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 0.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));

                    // user company detail
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

                    // line header
                    PdfPTable lineHeader = new PdfPTable(1);
                    float[] lineHeaderWithCells = new float[] { 100f };
                    lineHeader.SetWidths(lineHeaderWithCells);
                    lineHeader.WidthPercentage = 100;
                    lineHeader.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { Border = 1, Padding = 0f });

                    //  title
                    PdfPTable titleHeader = new PdfPTable(1);
                    float[] titleHeaderWithCells = new float[] { 100f };
                    titleHeader.SetWidths(titleHeaderWithCells);
                    titleHeader.WidthPercentage = 100;
                    titleHeader.AddCell(new PdfPCell(new Phrase("CASH VOUCHER", fontArial13Bold)) { Border = 0, PaddingBottom = 10f, PaddingTop = 1f, HorizontalAlignment = 1 });

                    String docNumber = "";
                    if (loan.FirstOrDefault().IsLoanApplication)
                    {
                        docNumber = "LN-" + loan.FirstOrDefault().LoanNumber;
                    }
                    else
                    {
                        if (loan.FirstOrDefault().IsLoanReconstruct)
                        {
                            docNumber = "RC-" + loan.FirstOrDefault().LoanNumber;
                        }
                        else
                        {
                            if (loan.FirstOrDefault().IsLoanRenew)
                            {
                                docNumber = "RN-" + loan.FirstOrDefault().LoanNumber;
                            }
                        }
                    }

                    PdfPTable loanDetailData = new PdfPTable(4);
                    float[] loanDetailDataHeaderWithCells = new float[] { 15f, 50f, 15f, 20f };
                    loanDetailData.SetWidths(loanDetailDataHeaderWithCells);
                    loanDetailData.WidthPercentage = 100;
                    loanDetailData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().mstApplicant.ApplicantLastName + ", " + loan.FirstOrDefault().mstApplicant.ApplicantFirstName + " " + loan.FirstOrDefault().mstApplicant.ApplicantMiddleName, fontArial12)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase("Doc. No", fontArial12Bold)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase(docNumber, fontArial12)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase("Address", fontArial12Bold)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().mstApplicant.CityAddress, fontArial12)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase("Start Date", fontArial12Bold)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().LoanDate.ToShortDateString(), fontArial12)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase("Area", fontArial12Bold)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().mstApplicant.mstArea.Area, fontArial12)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase("Maturity Date", fontArial12Bold)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().MaturityDate.ToShortDateString(), fontArial12)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase("Particulars", fontArial12Bold)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().Particulars, fontArial12)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase("Term", fontArial12Bold)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailData.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().mstTerm.Term, fontArial12)) { PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });

                    //  space
                    PdfPTable spaceTable = new PdfPTable(1);
                    float[] spaceTableWithCells = new float[] { 100f };
                    spaceTable.SetWidths(spaceTableWithCells);
                    spaceTable.WidthPercentage = 100;
                    spaceTable.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, PaddingTop = 2f, HorizontalAlignment = 1 });

                    PdfPTable loanDetaiAmountlData = new PdfPTable(5);
                    float[] lloanDetaiAmountlDataWithCells = new float[] { 18f, 22f, 10f, 15f, 35f };
                    loanDetaiAmountlData.SetWidths(lloanDetaiAmountlDataWithCells);
                    loanDetaiAmountlData.WidthPercentage = 100;
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase("Principal Amount", fontArial12Bold)) { Border = 0, PaddingTop = 1f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().PrincipalAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase("Interest", fontArial12Bold)) { Border = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().mstInterest.Interest, fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 10f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().mstInterest.Rate.ToString("#,##0") + " %", fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().InterestAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });

                    var loanDeductions = from d in db.trnLoanDeductions
                                         where d.LoanId == Convert.ToInt32(loanId)
                                         select d;

                    if (loanDeductions.Any())
                    {
                        var countDeductions = loanDeductions.Count();
                        loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase("Deductions", fontArial12Bold)) { Border = 0, Rowspan = countDeductions, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        foreach (var loanDeduction in loanDeductions)
                        {
                            var percentageRate = " ";
                            if (loanDeduction.mstDeduction.PercentageRate > 0)
                            {
                                percentageRate = loanDeduction.mstDeduction.PercentageRate.ToString("#,##0") + " %";
                            }

                            loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(loanDeduction.mstDeduction.Deduction, fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 10f, PaddingRight = 5f });
                            loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(percentageRate, fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(loanDeduction.DeductionAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                        }
                    }

                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase("Previous Balance", fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 10f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().PreviousBalanceAmount.ToString("#,##0.00"), fontArial12)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase("Net Amount", fontArial12Bold)) { Border = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().NetAmount.ToString("#,##0.00"), fontArial12)) { HorizontalAlignment = 2, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetaiAmountlData.AddCell(new PdfPCell(new Phrase(" ", fontArial12)) { Border = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });

                    // users
                    PdfPTable loanDetailUser = new PdfPTable(3);
                    float[] loanDetailUserWithCells = new float[] { 30f, 30f, 30f };
                    loanDetailUser.SetWidths(loanDetailUserWithCells);
                    loanDetailUser.WidthPercentage = 100;
                    loanDetailUser.AddCell(new PdfPCell(new Phrase("Prepared By:", fontArial12Bold)) { HorizontalAlignment = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailUser.AddCell(new PdfPCell(new Phrase("Released By:", fontArial12Bold)) { HorizontalAlignment = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailUser.AddCell(new PdfPCell(new Phrase("Received By:", fontArial12Bold)) { HorizontalAlignment = 0, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailUser.AddCell(new PdfPCell(new Phrase(loan.FirstOrDefault().mstUser.FullName, fontArial12)) { HorizontalAlignment = 1, PaddingTop = 20f, PaddingBottom = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailUser.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { HorizontalAlignment = 1, PaddingTop = 20f, PaddingBottom = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailUser.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { PaddingTop = 20f, PaddingBottom = 5f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailUser.AddCell(new PdfPCell(new Phrase("Signature over printed name", fontArial12)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailUser.AddCell(new PdfPCell(new Phrase("Signature over printed name", fontArial12)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                    loanDetailUser.AddCell(new PdfPCell(new Phrase("Signature over printed name", fontArial12)) { HorizontalAlignment = 1, PaddingTop = 1f, PaddingBottom = 3f, PaddingLeft = 5f, PaddingRight = 5f });

                    PdfPTable tableDivide = new PdfPTable(1);
                    float[] tableDivideWithCells = new float[] { 100f };
                    tableDivide.SetWidths(tableDivideWithCells);
                    tableDivide.WidthPercentage = 100;
                    tableDivide.AddCell(new PdfPCell(loanApplicationheader) { Border = 0, Padding = 0 });
                    tableDivide.AddCell(new PdfPCell(lineHeader) { Border = 0, Padding = 0 });
                    tableDivide.AddCell(new PdfPCell(titleHeader) { Border = 0, Padding = 0 });
                    tableDivide.AddCell(new PdfPCell(loanDetailData) { Border = 0, Padding = 0 });
                    tableDivide.AddCell(new PdfPCell(spaceTable) { Border = 0, Padding = 0 });
                    tableDivide.AddCell(new PdfPCell(loanDetaiAmountlData) { Border = 0, Padding = 0 });
                    tableDivide.AddCell(new PdfPCell(spaceTable) { Border = 0, Padding = 0 });
                    tableDivide.AddCell(new PdfPCell(loanDetailUser) { Border = 0, Padding = 0 });
                    tableDivide.AddCell(new PdfPCell(spaceTable) { Border = 0, Padding = 0 });

                    float LAST_CELL_HEIGHT = 50f;
                    PdfPTable tableDivide2 = new PdfPTable(1);
                    float[] tableDivide2WithCells = new float[] { 100f };
                    tableDivide2.SetWidths(tableDivide2WithCells);
                    tableDivide2.WidthPercentage = 100;
                    PdfPCell cell = new PdfPCell(tableDivide) { Border = 0, Padding = 0 };
                    cell.MinimumHeight = (document.PageSize.Height - LAST_CELL_HEIGHT) / 2;
                    tableDivide2.AddCell(cell);
                    tableDivide2.AddCell(cell);
                    document.Add(tableDivide2);

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
            else
            {
                return RedirectToAction("NotFound", "Software");
            }
        }
    }
}