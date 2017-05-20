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
    public class RepActiveOverdueCollectionPDFController : Controller
    {
        private Data.LendingDataContext db = new Data.LendingDataContext();

        public List<Models.TrnCollection> listCollectionByCollectionDateActiveAndOverdue(String collectionType, String areaId, String startCollectionDate, String endCollectionDate)
        {
            if (collectionType.Equals("active"))
            {
                if (areaId.Equals("0"))
                {
                    var collections = from d in db.trnCollections
                                      where d.CollectionDate >= Convert.ToDateTime(startCollectionDate)
                                      && d.CollectionDate <= Convert.ToDateTime(endCollectionDate)
                                      && d.trnLoan.IsLoanReconstruct != true
                                      && d.IsLocked == true
                                      select new Models.TrnCollection
                                      {
                                          Id = d.Id,
                                          CollectionNumber = d.CollectionNumber,
                                          CollectionDate = d.CollectionDate.ToShortDateString(),
                                          ApplicantId = d.trnLoan.ApplicantId,
                                          Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                          LoanId = d.LoanId,
                                          LoanNumberDetail = d.trnLoan.IsLoanApplication == true ? "LN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanReconstruct == true ? "RC-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanRenew == true ? "RN-" + d.trnLoan.LoanNumber : " ",
                                          Particulars = d.Particulars,
                                          TotalPaidAmount = d.TotalPaidAmount,
                                          TotalPenaltyAmount = d.TotalPenaltyAmount,
                                          PreparedByUserId = d.PreparedByUserId,
                                          PreparedByUser = d.mstUser.FullName,
                                          IsLocked = d.IsLocked,
                                          CreatedByUserId = d.CreatedByUserId,
                                          CreatedByUser = d.mstUser.FullName,
                                          CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                          UpdatedByUserId = d.UpdatedByUserId,
                                          UpdatedByUser = d.mstUser1.FullName,
                                          UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                          Area = d.trnLoan.mstApplicant.mstArea.Area
                                      };

                    return collections.ToList();
                }
                else
                {
                    var collections = from d in db.trnCollections
                                      where d.trnLoan.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                      && d.CollectionDate >= Convert.ToDateTime(startCollectionDate)
                                      && d.CollectionDate <= Convert.ToDateTime(endCollectionDate)
                                      && d.trnLoan.IsLoanReconstruct != true
                                      && d.IsLocked == true
                                      select new Models.TrnCollection
                                      {
                                          Id = d.Id,
                                          CollectionNumber = d.CollectionNumber,
                                          CollectionDate = d.CollectionDate.ToShortDateString(),
                                          ApplicantId = d.trnLoan.ApplicantId,
                                          Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                          LoanId = d.LoanId,
                                          LoanNumberDetail = d.trnLoan.IsLoanApplication == true ? "LN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanReconstruct == true ? "RC-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanRenew == true ? "RN-" + d.trnLoan.LoanNumber : " ",
                                          Particulars = d.Particulars,
                                          TotalPaidAmount = d.TotalPaidAmount,
                                          TotalPenaltyAmount = d.TotalPenaltyAmount,
                                          PreparedByUserId = d.PreparedByUserId,
                                          PreparedByUser = d.mstUser.FullName,
                                          IsLocked = d.IsLocked,
                                          CreatedByUserId = d.CreatedByUserId,
                                          CreatedByUser = d.mstUser.FullName,
                                          CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                          UpdatedByUserId = d.UpdatedByUserId,
                                          UpdatedByUser = d.mstUser1.FullName,
                                          UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                          Area = d.trnLoan.mstApplicant.mstArea.Area
                                      };

                    return collections.ToList();
                }
            }
            else
            {
                if (collectionType.Equals("overdue"))
                {
                    if (areaId.Equals("0"))
                    {
                        var collections = from d in db.trnCollections
                                          where d.CollectionDate >= Convert.ToDateTime(startCollectionDate)
                                          && d.CollectionDate <= Convert.ToDateTime(endCollectionDate)
                                          && d.trnLoan.IsLoanReconstruct == true
                                          && d.IsLocked == true
                                          select new Models.TrnCollection
                                          {
                                              Id = d.Id,
                                              CollectionNumber = d.CollectionNumber,
                                              CollectionDate = d.CollectionDate.ToShortDateString(),
                                              ApplicantId = d.trnLoan.ApplicantId,
                                              Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                              LoanId = d.LoanId,
                                              LoanNumberDetail = d.trnLoan.IsLoanApplication == true ? "LN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanReconstruct == true ? "RC-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanRenew == true ? "RN-" + d.trnLoan.LoanNumber : " ",
                                              Particulars = d.Particulars,
                                              TotalPaidAmount = d.TotalPaidAmount,
                                              TotalPenaltyAmount = d.TotalPenaltyAmount,
                                              PreparedByUserId = d.PreparedByUserId,
                                              PreparedByUser = d.mstUser.FullName,
                                              IsLocked = d.IsLocked,
                                              CreatedByUserId = d.CreatedByUserId,
                                              CreatedByUser = d.mstUser.FullName,
                                              CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                              UpdatedByUserId = d.UpdatedByUserId,
                                              UpdatedByUser = d.mstUser1.FullName,
                                              UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                              Area = d.trnLoan.mstApplicant.mstArea.Area
                                          };

                        return collections.ToList();
                    }
                    else
                    {
                        var collections = from d in db.trnCollections
                                          where d.trnLoan.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                          && d.CollectionDate >= Convert.ToDateTime(startCollectionDate)
                                          && d.CollectionDate <= Convert.ToDateTime(endCollectionDate)
                                          && d.trnLoan.IsLoanReconstruct == true
                                          && d.IsLocked == true
                                          select new Models.TrnCollection
                                          {
                                              Id = d.Id,
                                              CollectionNumber = d.CollectionNumber,
                                              CollectionDate = d.CollectionDate.ToShortDateString(),
                                              ApplicantId = d.trnLoan.ApplicantId,
                                              Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                              LoanId = d.LoanId,
                                              LoanNumberDetail = d.trnLoan.IsLoanApplication == true ? "LN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanReconstruct == true ? "RC-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanRenew == true ? "RN-" + d.trnLoan.LoanNumber : " ",
                                              Particulars = d.Particulars,
                                              TotalPaidAmount = d.TotalPaidAmount,
                                              TotalPenaltyAmount = d.TotalPenaltyAmount,
                                              PreparedByUserId = d.PreparedByUserId,
                                              PreparedByUser = d.mstUser.FullName,
                                              IsLocked = d.IsLocked,
                                              CreatedByUserId = d.CreatedByUserId,
                                              CreatedByUser = d.mstUser.FullName,
                                              CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                              UpdatedByUserId = d.UpdatedByUserId,
                                              UpdatedByUser = d.mstUser1.FullName,
                                              UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                              Area = d.trnLoan.mstApplicant.mstArea.Area
                                          };

                        return collections.ToList();
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public ActionResult activeOverdueReport(String collectionType, String areaId, String startCollectionDate, String endCollectionDate)
        {
            if (startCollectionDate != null && endCollectionDate != null)
            {
                if (collectionType != null && areaId != null)
                {
                    var collectionActiveAndOverdues = from d in this.listCollectionByCollectionDateActiveAndOverdue(collectionType, areaId, startCollectionDate, endCollectionDate).OrderBy(d => d.Applicant)
                                                      select new Models.TrnCollection
                                                      {
                                                          Id = d.Id,
                                                          CollectionNumber = d.CollectionNumber,
                                                          CollectionDate = d.CollectionDate,
                                                          ApplicantId = d.ApplicantId,
                                                          Applicant = d.Applicant,
                                                          LoanId = d.LoanId,
                                                          LoanNumberDetail = d.LoanNumberDetail,
                                                          Particulars = d.Particulars,
                                                          TotalPaidAmount = d.TotalPaidAmount,
                                                          TotalPenaltyAmount = d.TotalPenaltyAmount,
                                                          PreparedByUserId = d.PreparedByUserId,
                                                          PreparedByUser = d.PreparedByUser,
                                                          IsLocked = d.IsLocked,
                                                          CreatedByUserId = d.CreatedByUserId,
                                                          CreatedByUser = d.CreatedByUser,
                                                          CreatedDateTime = d.CreatedDateTime,
                                                          UpdatedByUserId = d.UpdatedByUserId,
                                                          UpdatedByUser = d.UpdatedByUser,
                                                          UpdatedDateTime = d.UpdatedDateTime,
                                                          Area = d.Area
                                                      };

                    if (collectionActiveAndOverdues.Any())
                    {
                        MemoryStream workStream = new MemoryStream();
                        Rectangle rectangle = new Rectangle(PageSize.A3.Rotate());
                        Document document = new Document(rectangle, 72, 72, 72, 72);
                        document.SetMargins(30f, 30f, 20f, 20f);
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

                        if (collectionType.Equals("active"))
                        {
                            titleHeader.AddCell(new PdfPCell(new Phrase("Active Collection Report", fontArial13Bold)) { Border = 0, PaddingBottom = 5f, PaddingTop = 1f, HorizontalAlignment = 1 });
                        }
                        else
                        {
                            if (collectionType.Equals("overdue"))
                            {
                                titleHeader.AddCell(new PdfPCell(new Phrase("Overdue Collection Report", fontArial13Bold)) { Border = 0, PaddingBottom = 5f, PaddingTop = 1f, HorizontalAlignment = 1 });
                            }
                        }

                        titleHeader.AddCell(new PdfPCell(new Phrase("From " + Convert.ToDateTime(startCollectionDate).ToString("MMMM dd, yyyy") + " to " + Convert.ToDateTime(endCollectionDate).ToString("MMMM dd, yyyy"), fontArial12)) { Border = 0, PaddingBottom = 12f, PaddingTop = 2f, HorizontalAlignment = 1 });
                        document.Add(titleHeader);

                        PdfPTable collectionData = new PdfPTable(8);
                        float[] collectionDataWithCells = new float[] { 7f, 8f, 25f, 10f, 20f, 10f, 10f, 10f };
                        collectionData.SetWidths(collectionDataWithCells);
                        collectionData.WidthPercentage = 100;
                        collectionData.AddCell(new PdfPCell(new Phrase("Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        collectionData.AddCell(new PdfPCell(new Phrase("Doc No.", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        collectionData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        collectionData.AddCell(new PdfPCell(new Phrase("Area", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        collectionData.AddCell(new PdfPCell(new Phrase("Particulars", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        collectionData.AddCell(new PdfPCell(new Phrase("Status", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        collectionData.AddCell(new PdfPCell(new Phrase("Paid", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        collectionData.AddCell(new PdfPCell(new Phrase("Penalty", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                        Decimal totalPaidAmount = 0;
                        Decimal totalPenaltyAmount = 0;
                        foreach (var collectionActiveAndOverdue in collectionActiveAndOverdues)
                        {
                            totalPaidAmount += collectionActiveAndOverdue.TotalPaidAmount;
                            totalPenaltyAmount += collectionActiveAndOverdue.TotalPenaltyAmount;

                            collectionData.AddCell(new PdfPCell(new Phrase(collectionActiveAndOverdue.CollectionDate, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            collectionData.AddCell(new PdfPCell(new Phrase(collectionActiveAndOverdue.CollectionNumber, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            collectionData.AddCell(new PdfPCell(new Phrase(collectionActiveAndOverdue.Applicant, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            collectionData.AddCell(new PdfPCell(new Phrase(collectionActiveAndOverdue.Area, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            collectionData.AddCell(new PdfPCell(new Phrase(collectionActiveAndOverdue.Particulars, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            collectionData.AddCell(new PdfPCell(new Phrase(" ", fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            collectionData.AddCell(new PdfPCell(new Phrase(collectionActiveAndOverdue.TotalPaidAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            collectionData.AddCell(new PdfPCell(new Phrase(collectionActiveAndOverdue.TotalPenaltyAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        }

                        collectionData.AddCell(new PdfPCell(new Phrase("TOTAL", fontArial11)) { Colspan = 6, HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(totalPaidAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                        collectionData.AddCell(new PdfPCell(new Phrase(totalPenaltyAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

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
            else
            {
                return RedirectToAction("NotFound", "Software");
            }
        }
    }
}