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
    public class RepLoanRenewReconstructSummaryPDFController : Controller
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        public List<Models.TrnLoan> loanRenewReconstructSummaryReports(String loanType, String areaId, String startLoanDate, String endLoanDate)
        {
            if (loanType.Equals("loan"))
            {
                if (areaId.Equals("0"))
                {
                    var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                           where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                           && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                           && d.IsLocked == true
                                           && d.IsLoanApplication == true
                                           select new Models.TrnLoan
                                           {
                                               Id = d.Id,
                                               LoanNumber = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
                                               LoanDate = d.LoanDate.ToShortDateString(),
                                               ApplicantId = d.ApplicantId,
                                               Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                               Area = d.mstApplicant.mstArea.Area,
                                               Particulars = d.Particulars,
                                               PreparedByUserId = d.PreparedByUserId,
                                               PreparedByUser = d.mstUser.FullName,
                                               TermId = d.TermId,
                                               Term = d.mstTerm.Term,
                                               TermNoOfDays = d.TermNoOfDays,
                                               TermPaymentNoOfDays = d.TermPaymentNoOfDays,
                                               MaturityDate = d.MaturityDate.ToShortDateString(),
                                               PrincipalAmount = d.PrincipalAmount,
                                               InterestId = d.InterestId,
                                               Interest = d.mstInterest.Interest,
                                               InterestRate = d.InterestRate,
                                               InterestAmount = d.InterestAmount,
                                               PreviousBalanceAmount = d.PreviousBalanceAmount,
                                               DeductionAmount = d.DeductionAmount,
                                               NetAmount = d.NetAmount,
                                               NetCollectionAmount = d.NetCollectionAmount,
                                               TotalPaidAmount = d.TotalPaidAmount,
                                               TotalPenaltyAmount = d.TotalPenaltyAmount,
                                               TotalBalanceAmount = d.TotalBalanceAmount,
                                               IsReconstruct = d.IsReconstruct,
                                               IsRenew = d.IsRenew,
                                               IsLoanApplication = d.IsLoanApplication,
                                               IsLoanReconstruct = d.IsLoanReconstruct,
                                               IsLoanRenew = d.IsLoanRenew,
                                               IsLocked = d.IsLocked,
                                               CreatedByUserId = d.CreatedByUserId,
                                               CreatedByUser = d.mstUser1.FullName,
                                               CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                               UpdatedByUserId = d.UpdatedByUserId,
                                               UpdatedByUser = d.mstUser2.FullName,
                                               UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                                           };

                    return loanApplications.ToList();
                }
                else
                {
                    var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                           where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                           && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                           && d.IsLocked == true
                                           && d.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                           && d.IsLoanApplication == true
                                           select new Models.TrnLoan
                                           {
                                               Id = d.Id,
                                               LoanNumber = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
                                               LoanDate = d.LoanDate.ToShortDateString(),
                                               ApplicantId = d.ApplicantId,
                                               Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                               Area = d.mstApplicant.mstArea.Area,
                                               Particulars = d.Particulars,
                                               PreparedByUserId = d.PreparedByUserId,
                                               PreparedByUser = d.mstUser.FullName,
                                               TermId = d.TermId,
                                               Term = d.mstTerm.Term,
                                               TermNoOfDays = d.TermNoOfDays,
                                               TermPaymentNoOfDays = d.TermPaymentNoOfDays,
                                               MaturityDate = d.MaturityDate.ToShortDateString(),
                                               PrincipalAmount = d.PrincipalAmount,
                                               InterestId = d.InterestId,
                                               Interest = d.mstInterest.Interest,
                                               InterestRate = d.InterestRate,
                                               InterestAmount = d.InterestAmount,
                                               PreviousBalanceAmount = d.PreviousBalanceAmount,
                                               DeductionAmount = d.DeductionAmount,
                                               NetAmount = d.NetAmount,
                                               NetCollectionAmount = d.NetCollectionAmount,
                                               TotalPaidAmount = d.TotalPaidAmount,
                                               TotalPenaltyAmount = d.TotalPenaltyAmount,
                                               TotalBalanceAmount = d.TotalBalanceAmount,
                                               IsReconstruct = d.IsReconstruct,
                                               IsRenew = d.IsRenew,
                                               IsLoanApplication = d.IsLoanApplication,
                                               IsLoanReconstruct = d.IsLoanReconstruct,
                                               IsLoanRenew = d.IsLoanRenew,
                                               IsLocked = d.IsLocked,
                                               CreatedByUserId = d.CreatedByUserId,
                                               CreatedByUser = d.mstUser1.FullName,
                                               CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                               UpdatedByUserId = d.UpdatedByUserId,
                                               UpdatedByUser = d.mstUser2.FullName,
                                               UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                                           };

                    return loanApplications.ToList();
                }
            }
            else
            {
                if (loanType.Equals("renew"))
                {
                    if (areaId.Equals("0"))
                    {
                        var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                               join s in db.trnLoanRenews
                                               on d.Id equals s.LoanId
                                               into joinRenews
                                               from listRenews in joinRenews.DefaultIfEmpty()
                                               where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                               && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                               && d.IsLocked == true
                                               && d.IsLoanRenew == true
                                               select new Models.TrnLoan
                                               {
                                                   Id = d.Id,
                                                   LoanNumber = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
                                                   LoanDate = d.LoanDate.ToShortDateString(),
                                                   ApplicantId = d.ApplicantId,
                                                   Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                                   Area = d.mstApplicant.mstArea.Area,
                                                   Particulars = d.Particulars,
                                                   PreparedByUserId = d.PreparedByUserId,
                                                   PreparedByUser = d.mstUser.FullName,
                                                   TermId = d.TermId,
                                                   Term = d.mstTerm.Term,
                                                   TermNoOfDays = d.TermNoOfDays,
                                                   TermPaymentNoOfDays = d.TermPaymentNoOfDays,
                                                   MaturityDate = d.MaturityDate.ToShortDateString(),
                                                   PrincipalAmount = d.PrincipalAmount,
                                                   InterestId = d.InterestId,
                                                   Interest = d.mstInterest.Interest,
                                                   InterestRate = d.InterestRate,
                                                   InterestAmount = d.InterestAmount,
                                                   PreviousBalanceAmount = d.PreviousBalanceAmount,
                                                   DeductionAmount = d.DeductionAmount,
                                                   NetAmount = d.NetAmount,
                                                   NetCollectionAmount = d.NetCollectionAmount,
                                                   TotalPaidAmount = d.TotalPaidAmount,
                                                   TotalPenaltyAmount = d.TotalPenaltyAmount,
                                                   TotalBalanceAmount = d.TotalBalanceAmount,
                                                   IsReconstruct = d.IsReconstruct,
                                                   IsRenew = d.IsRenew,
                                                   IsLoanApplication = d.IsLoanApplication,
                                                   IsLoanReconstruct = d.IsLoanReconstruct,
                                                   IsLoanRenew = d.IsLoanRenew,
                                                   IsLocked = d.IsLocked,
                                                   CreatedByUserId = d.CreatedByUserId,
                                                   CreatedByUser = d.mstUser1.FullName,
                                                   CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                                   UpdatedByUserId = d.UpdatedByUserId,
                                                   UpdatedByUser = d.mstUser2.FullName,
                                                   UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                                   RenewedDocNumber = joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanApplication == true ? "LN-" + joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanReconstruct == true ? "RC-" + joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanRenew == true ? "RN-" + joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : " ",
                                                   ReconstructedDocNumber = " "
                                               };

                        return loanApplications.ToList();
                    }
                    else
                    {
                        var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                               join s in db.trnLoanRenews
                                               on d.Id equals s.LoanId
                                               into joinRenews
                                               from listRenews in joinRenews.DefaultIfEmpty()
                                               where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                               && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                               && d.IsLocked == true
                                               && d.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                               && d.IsLoanRenew == true
                                               select new Models.TrnLoan
                                               {
                                                   Id = d.Id,
                                                   LoanNumber = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
                                                   LoanDate = d.LoanDate.ToShortDateString(),
                                                   ApplicantId = d.ApplicantId,
                                                   Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                                   Area = d.mstApplicant.mstArea.Area,
                                                   Particulars = d.Particulars,
                                                   PreparedByUserId = d.PreparedByUserId,
                                                   PreparedByUser = d.mstUser.FullName,
                                                   TermId = d.TermId,
                                                   Term = d.mstTerm.Term,
                                                   TermNoOfDays = d.TermNoOfDays,
                                                   TermPaymentNoOfDays = d.TermPaymentNoOfDays,
                                                   MaturityDate = d.MaturityDate.ToShortDateString(),
                                                   PrincipalAmount = d.PrincipalAmount,
                                                   InterestId = d.InterestId,
                                                   Interest = d.mstInterest.Interest,
                                                   InterestRate = d.InterestRate,
                                                   InterestAmount = d.InterestAmount,
                                                   PreviousBalanceAmount = d.PreviousBalanceAmount,
                                                   DeductionAmount = d.DeductionAmount,
                                                   NetAmount = d.NetAmount,
                                                   NetCollectionAmount = d.NetCollectionAmount,
                                                   TotalPaidAmount = d.TotalPaidAmount,
                                                   TotalPenaltyAmount = d.TotalPenaltyAmount,
                                                   TotalBalanceAmount = d.TotalBalanceAmount,
                                                   IsReconstruct = d.IsReconstruct,
                                                   IsRenew = d.IsRenew,
                                                   IsLoanApplication = d.IsLoanApplication,
                                                   IsLoanReconstruct = d.IsLoanReconstruct,
                                                   IsLoanRenew = d.IsLoanRenew,
                                                   IsLocked = d.IsLocked,
                                                   CreatedByUserId = d.CreatedByUserId,
                                                   CreatedByUser = d.mstUser1.FullName,
                                                   CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                                   UpdatedByUserId = d.UpdatedByUserId,
                                                   UpdatedByUser = d.mstUser2.FullName,
                                                   UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                                   RenewedDocNumber = joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanApplication == true ? "LN-" + joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanReconstruct == true ? "RC-" + joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanRenew == true ? "RN-" + joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : " ",
                                                   ReconstructedDocNumber = " "
                                               };

                        return loanApplications.ToList();
                    }
                }
                else
                {
                    if (loanType.Equals("reconstruct"))
                    {
                        if (areaId.Equals("0"))
                        {
                            var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                                   join s in db.trnLoanReconstructs
                                                   on d.Id equals s.LoanId
                                                   into joinReconstructs
                                                   from listReconstructs in joinReconstructs.DefaultIfEmpty()
                                                   where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                                   && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                                   && d.IsLocked == true
                                                   && d.IsLoanReconstruct == true
                                                   select new Models.TrnLoan
                                                   {
                                                       Id = d.Id,
                                                       LoanNumber = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
                                                       LoanDate = d.LoanDate.ToShortDateString(),
                                                       ApplicantId = d.ApplicantId,
                                                       Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                                       Area = d.mstApplicant.mstArea.Area,
                                                       Particulars = d.Particulars,
                                                       PreparedByUserId = d.PreparedByUserId,
                                                       PreparedByUser = d.mstUser.FullName,
                                                       TermId = d.TermId,
                                                       Term = d.mstTerm.Term,
                                                       TermNoOfDays = d.TermNoOfDays,
                                                       TermPaymentNoOfDays = d.TermPaymentNoOfDays,
                                                       MaturityDate = d.MaturityDate.ToShortDateString(),
                                                       PrincipalAmount = d.PrincipalAmount,
                                                       InterestId = d.InterestId,
                                                       Interest = d.mstInterest.Interest,
                                                       InterestRate = d.InterestRate,
                                                       InterestAmount = d.InterestAmount,
                                                       PreviousBalanceAmount = d.PreviousBalanceAmount,
                                                       DeductionAmount = d.DeductionAmount,
                                                       NetAmount = d.NetAmount,
                                                       NetCollectionAmount = d.NetCollectionAmount,
                                                       TotalPaidAmount = d.TotalPaidAmount,
                                                       TotalPenaltyAmount = d.TotalPenaltyAmount,
                                                       TotalBalanceAmount = d.TotalBalanceAmount,
                                                       IsReconstruct = d.IsReconstruct,
                                                       IsRenew = d.IsRenew,
                                                       IsLoanApplication = d.IsLoanApplication,
                                                       IsLoanReconstruct = d.IsLoanReconstruct,
                                                       IsLoanRenew = d.IsLoanRenew,
                                                       IsLocked = d.IsLocked,
                                                       CreatedByUserId = d.CreatedByUserId,
                                                       CreatedByUser = d.mstUser1.FullName,
                                                       CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                                       UpdatedByUserId = d.UpdatedByUserId,
                                                       UpdatedByUser = d.mstUser2.FullName,
                                                       UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                                       RenewedDocNumber = " ",
                                                       ReconstructedDocNumber = joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanApplication == true ? "LN-" + joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanReconstruct == true ? "RC-" + joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanRenew == true ? "RN-" + joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : " "
                                                   };

                            return loanApplications.ToList();
                        }
                        else
                        {
                            var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                                   join s in db.trnLoanReconstructs
                                                   on d.Id equals s.LoanId
                                                   into joinReconstructs
                                                   from listReconstructs in joinReconstructs.DefaultIfEmpty()
                                                   where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                                   && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                                   && d.IsLocked == true
                                                   && d.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                                   && d.IsLoanReconstruct == true
                                                   select new Models.TrnLoan
                                                   {
                                                       Id = d.Id,
                                                       LoanNumber = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
                                                       LoanDate = d.LoanDate.ToShortDateString(),
                                                       ApplicantId = d.ApplicantId,
                                                       Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                                       Area = d.mstApplicant.mstArea.Area,
                                                       Particulars = d.Particulars,
                                                       PreparedByUserId = d.PreparedByUserId,
                                                       PreparedByUser = d.mstUser.FullName,
                                                       TermId = d.TermId,
                                                       Term = d.mstTerm.Term,
                                                       TermNoOfDays = d.TermNoOfDays,
                                                       TermPaymentNoOfDays = d.TermPaymentNoOfDays,
                                                       MaturityDate = d.MaturityDate.ToShortDateString(),
                                                       PrincipalAmount = d.PrincipalAmount,
                                                       InterestId = d.InterestId,
                                                       Interest = d.mstInterest.Interest,
                                                       InterestRate = d.InterestRate,
                                                       InterestAmount = d.InterestAmount,
                                                       PreviousBalanceAmount = d.PreviousBalanceAmount,
                                                       DeductionAmount = d.DeductionAmount,
                                                       NetAmount = d.NetAmount,
                                                       NetCollectionAmount = d.NetCollectionAmount,
                                                       TotalPaidAmount = d.TotalPaidAmount,
                                                       TotalPenaltyAmount = d.TotalPenaltyAmount,
                                                       TotalBalanceAmount = d.TotalBalanceAmount,
                                                       IsReconstruct = d.IsReconstruct,
                                                       IsRenew = d.IsRenew,
                                                       IsLoanApplication = d.IsLoanApplication,
                                                       IsLoanReconstruct = d.IsLoanReconstruct,
                                                       IsLoanRenew = d.IsLoanRenew,
                                                       IsLocked = d.IsLocked,
                                                       CreatedByUserId = d.CreatedByUserId,
                                                       CreatedByUser = d.mstUser1.FullName,
                                                       CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                                       UpdatedByUserId = d.UpdatedByUserId,
                                                       UpdatedByUser = d.mstUser2.FullName,
                                                       UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                                       RenewedDocNumber = " ",
                                                       ReconstructedDocNumber = joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanApplication == true ? "LN-" + joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanReconstruct == true ? "RC-" + joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanRenew == true ? "RN-" + joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : " "
                                                   };

                            return loanApplications.ToList();
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public ActionResult loanRenewReconstructSummaryReport(String loanType, String areaId, String startLoanDate, String endLoanDate)
        {
            if (startLoanDate != null && endLoanDate != null)
            {
                if (loanType != null && areaId != null)
                {
                    var loanApplications = from d in this.loanRenewReconstructSummaryReports(loanType, areaId, startLoanDate, endLoanDate).OrderBy(d => d.Applicant)
                                           select new Models.TrnLoan
                                           {
                                               Id = d.Id,
                                               LoanNumber = d.LoanNumber,
                                               LoanDate = d.LoanDate,
                                               ApplicantId = d.ApplicantId,
                                               Applicant = d.Applicant,
                                               Area = d.Area,
                                               Particulars = d.Particulars,
                                               PreparedByUserId = d.PreparedByUserId,
                                               PreparedByUser = d.PreparedByUser,
                                               TermId = d.TermId,
                                               Term = d.Term,
                                               TermNoOfDays = d.TermNoOfDays,
                                               TermPaymentNoOfDays = d.TermPaymentNoOfDays,
                                               MaturityDate = d.MaturityDate,
                                               PrincipalAmount = d.PrincipalAmount,
                                               InterestId = d.InterestId,
                                               Interest = d.Interest,
                                               InterestRate = d.InterestRate,
                                               InterestAmount = d.InterestAmount,
                                               PreviousBalanceAmount = d.PreviousBalanceAmount,
                                               DeductionAmount = d.DeductionAmount,
                                               NetAmount = d.NetAmount,
                                               NetCollectionAmount = d.NetCollectionAmount,
                                               TotalPaidAmount = d.TotalPaidAmount,
                                               TotalPenaltyAmount = d.TotalPenaltyAmount,
                                               TotalBalanceAmount = d.TotalBalanceAmount,
                                               IsReconstruct = d.IsReconstruct,
                                               IsRenew = d.IsRenew,
                                               IsLoanApplication = d.IsLoanApplication,
                                               IsLoanReconstruct = d.IsLoanReconstruct,
                                               IsLoanRenew = d.IsLoanRenew,
                                               IsLocked = d.IsLocked,
                                               CreatedByUserId = d.CreatedByUserId,
                                               CreatedByUser = d.CreatedByUser,
                                               CreatedDateTime = d.CreatedDateTime,
                                               UpdatedByUserId = d.UpdatedByUserId,
                                               UpdatedByUser = d.UpdatedByUser,
                                               UpdatedDateTime = d.UpdatedDateTime,
                                           };

                    if (loanApplications.Any())
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

                        if (loanType.Equals("loan"))
                        {
                            titleHeader.AddCell(new PdfPCell(new Phrase("Loan Summary Report", fontArial13Bold)) { Border = 0, PaddingBottom = 5f, PaddingTop = 1f, HorizontalAlignment = 1 });
                        }
                        else
                        {
                            if (loanType.Equals("renew"))
                            {
                                titleHeader.AddCell(new PdfPCell(new Phrase("Renew Summary Report", fontArial13Bold)) { Border = 0, PaddingBottom = 5f, PaddingTop = 1f, HorizontalAlignment = 1 });
                            }
                            else
                            {
                                if (loanType.Equals("reconstruct"))
                                {
                                    titleHeader.AddCell(new PdfPCell(new Phrase("Reconstruct / Overdues Summary Report", fontArial13Bold)) { Border = 0, PaddingBottom = 5f, PaddingTop = 1f, HorizontalAlignment = 1 });
                                }
                            }
                        }

                        titleHeader.AddCell(new PdfPCell(new Phrase("From " + Convert.ToDateTime(startLoanDate).ToString("MMMM dd, yyyy") + " to " + Convert.ToDateTime(endLoanDate).ToString("MMMM dd, yyyy"), fontArial12)) { Border = 0, PaddingBottom = 12f, PaddingTop = 2f, HorizontalAlignment = 1 });
                        document.Add(titleHeader);

                        PdfPTable loanlData = new PdfPTable(13);
                        float[] loanlDataWithCells = new float[] { 7f, 9f, 15f, 8f, 8f, 8f, 8f, 8f, 8f, 8f, 8f, 8f, 8f };
                        loanlData.SetWidths(loanlDataWithCells);
                        loanlData.WidthPercentage = 100;
                        loanlData.AddCell(new PdfPCell(new Phrase("Date", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Doc No.", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Applicant", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Area", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Principal", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Interest", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Prev. Bal.", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Deductions", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Net", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Collection", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Paid", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Penalty", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });
                        loanlData.AddCell(new PdfPCell(new Phrase("Balance", fontArial12Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f, BackgroundColor = BaseColor.LIGHT_GRAY });

                        Decimal totalPrincipalAmount = 0;
                        Decimal totalInterestAmount = 0;
                        Decimal totalPreviousBalanceAmount = 0;
                        Decimal totalDeductionAmount = 0;
                        Decimal totalNetAmount = 0;
                        Decimal totalNetCollectionAmount = 0;
                        Decimal totalTotalPaidAmount = 0;
                        Decimal totalTotalPenaltyAmount = 0;
                        Decimal totalTotalBalanceAmount = 0;
                        foreach (var loanLine in loanApplications)
                        {
                            totalPrincipalAmount += loanLine.PrincipalAmount;
                            totalInterestAmount += loanLine.InterestAmount;
                            totalPreviousBalanceAmount += loanLine.PreviousBalanceAmount;
                            totalDeductionAmount += loanLine.DeductionAmount;
                            totalNetAmount += loanLine.NetAmount;
                            totalNetCollectionAmount += loanLine.NetCollectionAmount;
                            totalTotalPaidAmount += loanLine.TotalPaidAmount;
                            totalTotalPenaltyAmount += loanLine.TotalPenaltyAmount;
                            totalTotalBalanceAmount += loanLine.TotalBalanceAmount;

                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.LoanDate, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.LoanNumber, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.Applicant, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.Area, fontArial11)) { PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.PrincipalAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.InterestAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.PreviousBalanceAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.DeductionAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.NetAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.NetCollectionAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.TotalPaidAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.TotalPenaltyAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                            loanlData.AddCell(new PdfPCell(new Phrase(loanLine.TotalBalanceAmount.ToString("#,##0.00"), fontArial11)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        }

                        loanlData.AddCell(new PdfPCell(new Phrase("TOTAL", fontArial11Bold)) { Colspan = 4, HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(totalPrincipalAmount.ToString("#,##0.00"), fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(totalInterestAmount.ToString("#,##0.00"), fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(totalPreviousBalanceAmount.ToString("#,##0.00"), fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(totalDeductionAmount.ToString("#,##0.00"), fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(totalNetAmount.ToString("#,##0.00"), fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(totalNetCollectionAmount.ToString("#,##0.00"), fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(totalTotalPaidAmount.ToString("#,##0.00"), fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(totalTotalPenaltyAmount.ToString("#,##0.00"), fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                        loanlData.AddCell(new PdfPCell(new Phrase(totalTotalBalanceAmount.ToString("#,##0.00"), fontArial11Bold)) { HorizontalAlignment = 2, PaddingTop = 6f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

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
            else
            {
                return RedirectToAction("NotFound", "Software");
            }
        }
    }
}