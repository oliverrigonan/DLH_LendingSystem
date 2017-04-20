using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Diagnostics;

namespace Lending.ApiControllers
{
    public class ApiLoanReconstructController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan recontructs list
        [Authorize]
        [HttpGet]
        [Route("api/loanReconstruct/list/ByLoanId/{loanId}")]
        public List<Models.TrnLoanReconstruct> listReconstruct(String loanId)
        {
            var reconstructs = from d in db.trnLoanReconstructs.OrderByDescending(d => d.Id)
                               where d.LoanId == Convert.ToInt32(loanId)
                               select new Models.TrnLoanReconstruct
                               {
                                   Id = d.Id,
                                   LoanId = d.LoanId,
                                   ReconstructLoanId = d.ReconstructLoanId,
                                   ReconstructLoanNumber = d.trnLoan1.IsLoanApplication == true ? d.trnLoan1.IsReconstruct == true ? "LN - " + d.trnLoan1.LoanNumber + " (Reconstructed)" : "LN - " + d.trnLoan1.LoanNumber : d.trnLoan1.IsRenew == true ? "LN - " + d.trnLoan1.LoanNumber + " (Renewed)" : d.trnLoan1.IsLoanReconstruct == true ? d.trnLoan1.IsReconstruct == true ? "RC - " + d.trnLoan1.LoanNumber + " (Reconstructed)" : "RC - " + d.trnLoan1.LoanNumber : d.trnLoan1.IsRenew == true ? "RC - " + d.trnLoan1.LoanNumber + " (Renewed)" : d.trnLoan1.IsLoanRenew == true ? d.trnLoan1.IsReconstruct == true ? "RN - " + d.trnLoan1.LoanNumber + " (Reconstructed)" : "RN - " + d.trnLoan1.LoanNumber : d.trnLoan1.IsRenew == true ? "RN - " + d.trnLoan1.LoanNumber + " (Renewed)" : d.trnLoan1.LoanNumber,
                                   ReconstructLoanTotalBalanceAmount = d.ReconstructLoanTotalBalanceAmount,
                                   IsLoanApplication = d.trnLoan1.IsLoanApplication,
                                   IsLoanReconstruct = d.trnLoan1.IsLoanReconstruct,
                                   IsLoanRenew = d.trnLoan1.IsRenew
                               };

            return reconstructs.ToList();
        }

        // zero fill
        public String zeroFill(Int32 number, Int32 length)
        {
            var result = number.ToString();
            var pad = length - result.Length;
            while (pad > 0)
            {
                result = "0" + result;
                pad--;
            }

            return result;
        }

        // reconstruct list by loan date
        [Authorize]
        [HttpGet]
        [Route("api/reconstruct/listByLoanDate/{startLoanDate}/{endLoanDate}")]
        public List<Models.TrnLoan> listReconstructByLoanDate(String startLoanDate, String endLoanDate)
        {
            var reconstructs = from d in db.trnLoans.OrderByDescending(d => d.Id)
                               where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                               && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                               && d.IsLoanReconstruct == true
                               select new Models.TrnLoan
                               {
                                   Id = d.Id,
                                   LoanNumber = d.LoanNumber,
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
                                   IsFullyPaid = d.IsFullyPaid,
                                   IsLocked = d.IsLocked,
                                   CreatedByUserId = d.CreatedByUserId,
                                   CreatedByUser = d.mstUser1.FullName,
                                   CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                   UpdatedByUserId = d.UpdatedByUserId,
                                   UpdatedByUser = d.mstUser2.FullName,
                                   UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                   ReconstructedDocNumber = getReconstructedDocNumber(d.Id)
                               };

            return reconstructs.ToList();
        }

        public String getReconstructedDocNumber(Int32 loanId)
        {
            var reconstructedLoans = from d in db.trnLoanReconstructs
                                     where d.LoanId == loanId
                                     select d;

            String reconstructedDocNumber = " ";
            if (reconstructedLoans.Any())
            {
                reconstructedDocNumber = reconstructedLoans.FirstOrDefault().trnLoan1.IsLoanApplication == true ? reconstructedLoans.FirstOrDefault().trnLoan1.IsReconstruct == true ? "LN - " + reconstructedLoans.FirstOrDefault().trnLoan1.LoanNumber + " (Reconstructed)" : "LN - " + reconstructedLoans.FirstOrDefault().trnLoan1.LoanNumber : reconstructedLoans.FirstOrDefault().trnLoan1.IsRenew == true ? "LN - " + reconstructedLoans.FirstOrDefault().trnLoan1.LoanNumber + " (Renewed)" : reconstructedLoans.FirstOrDefault().trnLoan1.IsLoanReconstruct == true ? reconstructedLoans.FirstOrDefault().trnLoan1.IsReconstruct == true ? "RC - " + reconstructedLoans.FirstOrDefault().trnLoan1.LoanNumber + " (Reconstructed)" : "RC - " + reconstructedLoans.FirstOrDefault().trnLoan1.LoanNumber : reconstructedLoans.FirstOrDefault().trnLoan1.IsRenew == true ? "RC - " + reconstructedLoans.FirstOrDefault().trnLoan1.LoanNumber + " (Renewed)" : reconstructedLoans.FirstOrDefault().trnLoan1.IsLoanRenew == true ? reconstructedLoans.FirstOrDefault().trnLoan1.IsReconstruct == true ? "RN - " + reconstructedLoans.FirstOrDefault().trnLoan1.LoanNumber + " (Reconstructed)" : "RN - " + reconstructedLoans.FirstOrDefault().trnLoan1.LoanNumber : reconstructedLoans.FirstOrDefault().trnLoan1.IsRenew == true ? "RN - " + reconstructedLoans.FirstOrDefault().trnLoan1.LoanNumber + " (Renewed)" : reconstructedLoans.FirstOrDefault().trnLoan1.LoanNumber;
            }

            return reconstructedDocNumber;
        }

        // reconstruct get by id
        [Authorize]
        [HttpGet]
        [Route("api/reconstruct/getById/{id}")]
        public Models.TrnLoan getReconstructById(String id)
        {
            var reconstruct = from d in db.trnLoans
                              where d.Id == Convert.ToInt32(id)
                              && d.IsLoanReconstruct == true
                              select new Models.TrnLoan
                              {
                                  Id = d.Id,
                                  LoanNumber = d.LoanNumber,
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
                                  IsFullyPaid = d.IsFullyPaid,
                                  IsLocked = d.IsLocked,
                                  CreatedByUserId = d.CreatedByUserId,
                                  CreatedByUser = d.mstUser1.FullName,
                                  CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                  UpdatedByUserId = d.UpdatedByUserId,
                                  UpdatedByUser = d.mstUser2.FullName,
                                  UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                              };

            return (Models.TrnLoan)reconstruct.FirstOrDefault();
        }

        // add loan reconstruct
        [Authorize]
        [HttpPost]
        [Route("api/loanReconstruct/add")]
        public Int32 addLoanReconstruct(Models.TrnLoanReconstruct loanReconstruct)
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                var mstUserForms = from d in db.mstUserForms
                                   where d.UserId == userId
                                   select new Models.MstUserForm
                                   {
                                       Id = d.Id,
                                       Form = d.sysForm.Form,
                                       CanPerformActions = d.CanPerformActions
                                   };

                if (mstUserForms.Any())
                {
                    String matchPageString = "LoanApplicationList";
                    Boolean canPerformActions = false;

                    foreach (var mstUserForm in mstUserForms)
                    {
                        if (mstUserForm.Form.Equals(matchPageString))
                        {
                            if (mstUserForm.CanPerformActions)
                            {
                                canPerformActions = true;
                            }

                            break;
                        }
                    }

                    if (canPerformActions)
                    {
                        String loanNumber = "0000000001";
                        var loan = from d in db.trnLoans.OrderByDescending(d => d.Id) where d.IsLoanReconstruct == true select d;
                        if (loan.Any())
                        {
                            var newLoanNumber = Convert.ToInt32(loan.FirstOrDefault().LoanNumber) + 0000000001;
                            loanNumber = newLoanNumber.ToString();
                        }

                        var term = from d in db.mstTerms select d;
                        if (term.Any())
                        {
                            var interest = from d in db.mstInterests select d;
                            if (interest.Any())
                            {
                                var existLoan = from d in db.trnLoans
                                                where d.Id == loanReconstruct.ReconstructLoanId
                                                where d.IsLocked == true
                                                select d;

                                if (existLoan.Any())
                                {
                                    Data.trnLoan newLoan = new Data.trnLoan();
                                    newLoan.LoanNumber = zeroFill(Convert.ToInt32(loanNumber), 10);
                                    newLoan.LoanDate = DateTime.Today;
                                    newLoan.ApplicantId = loanReconstruct.ApplicantId;
                                    newLoan.Particulars = "NA";
                                    newLoan.PreparedByUserId = userId;
                                    newLoan.TermId = term.FirstOrDefault().Id;
                                    newLoan.TermNoOfDays = term.FirstOrDefault().NoOfDays;
                                    newLoan.TermPaymentNoOfDays = term.FirstOrDefault().PaymentNoOfDays;
                                    newLoan.MaturityDate = DateTime.Today;
                                    newLoan.PrincipalAmount = loanReconstruct.ReconstructLoanTotalBalanceAmount;
                                    newLoan.InterestId = interest.FirstOrDefault().Id;
                                    newLoan.InterestRate = interest.FirstOrDefault().Rate;
                                    Decimal interestAmount = (loanReconstruct.ReconstructLoanTotalBalanceAmount / 100) * interest.FirstOrDefault().Rate;
                                    newLoan.InterestAmount = interestAmount;
                                    newLoan.PreviousBalanceAmount = 0;
                                    newLoan.DeductionAmount = 0;
                                    newLoan.NetAmount = 0;
                                    newLoan.NetCollectionAmount = loanReconstruct.ReconstructLoanTotalBalanceAmount + loanReconstruct.ReconstructLoanTotalPenaltyAmount + interestAmount;
                                    newLoan.TotalPaidAmount = 0;
                                    newLoan.TotalPenaltyAmount = 0;
                                    newLoan.TotalBalanceAmount = loanReconstruct.ReconstructLoanTotalBalanceAmount + loanReconstruct.ReconstructLoanTotalPenaltyAmount + interestAmount;
                                    newLoan.IsReconstruct = false;
                                    newLoan.IsRenew = false;
                                    newLoan.IsLoanApplication = false;
                                    newLoan.IsLoanReconstruct = true;
                                    newLoan.IsLoanRenew = false;
                                    newLoan.IsFullyPaid = false;
                                    newLoan.IsLocked = false;
                                    newLoan.CreatedByUserId = userId;
                                    newLoan.CreatedDateTime = DateTime.Now;
                                    newLoan.UpdatedByUserId = userId;
                                    newLoan.UpdatedDateTime = DateTime.Now;
                                    db.trnLoans.InsertOnSubmit(newLoan);
                                    db.SubmitChanges();

                                    Data.trnLoanReconstruct newLoanReconstruct = new Data.trnLoanReconstruct();
                                    newLoanReconstruct.LoanId = newLoan.Id;
                                    newLoanReconstruct.ReconstructLoanId = loanReconstruct.ReconstructLoanId;
                                    newLoanReconstruct.ReconstructLoanTotalBalanceAmount = loanReconstruct.ReconstructLoanTotalBalanceAmount;
                                    db.trnLoanReconstructs.InsertOnSubmit(newLoanReconstruct);
                                    db.SubmitChanges();

                                    return newLoan.Id;
                                }
                                else
                                {
                                    return 0;
                                }
                            }
                            else
                            {
                                return 0;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        // add loan reconstruct overdue
        [Authorize]
        [HttpPost]
        [Route("api/loanReconstruct/overdue/add")]
        public Int32 addLoanReconstructOverdue(Models.TrnLoanReconstruct loanReconstruct)
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                var mstUserForms = from d in db.mstUserForms
                                   where d.UserId == userId
                                   select new Models.MstUserForm
                                   {
                                       Id = d.Id,
                                       Form = d.sysForm.Form,
                                       CanPerformActions = d.CanPerformActions
                                   };

                if (mstUserForms.Any())
                {
                    String matchPageString = "LoanApplicationList";
                    Boolean canPerformActions = false;

                    foreach (var mstUserForm in mstUserForms)
                    {
                        if (mstUserForm.Form.Equals(matchPageString))
                        {
                            if (mstUserForm.CanPerformActions)
                            {
                                canPerformActions = true;
                            }

                            break;
                        }
                    }

                    if (canPerformActions)
                    {
                        String loanNumber = "0000000001";
                        var loan = from d in db.trnLoans.OrderByDescending(d => d.Id) where d.IsLoanReconstruct == true select d;
                        if (loan.Any())
                        {
                            var newLoanNumber = Convert.ToInt32(loan.FirstOrDefault().LoanNumber) + 0000000001;
                            loanNumber = newLoanNumber.ToString();
                        }

                        var term = from d in db.mstTerms select d;
                        if (term.Any())
                        {
                            var interest = from d in db.mstInterests select d;
                            if (interest.Any())
                            {
                                Data.trnLoan newLoan = new Data.trnLoan();
                                newLoan.LoanNumber = zeroFill(Convert.ToInt32(loanNumber), 10);
                                newLoan.LoanDate = Convert.ToDateTime(loanReconstruct.LoanDate);
                                newLoan.ApplicantId = loanReconstruct.ApplicantId;
                                newLoan.Particulars = loanReconstruct.Particulars;
                                newLoan.PreparedByUserId = userId;
                                newLoan.TermId = term.FirstOrDefault().Id;
                                newLoan.TermNoOfDays = term.FirstOrDefault().NoOfDays;
                                newLoan.TermPaymentNoOfDays = term.FirstOrDefault().PaymentNoOfDays;
                                newLoan.MaturityDate = Convert.ToDateTime(loanReconstruct.LoanDate);
                                newLoan.PrincipalAmount = loanReconstruct.ReconstructLoanTotalBalanceAmount;
                                newLoan.InterestId = interest.FirstOrDefault().Id;
                                newLoan.InterestRate = interest.FirstOrDefault().Rate;
                                Decimal interestAmount = (loanReconstruct.ReconstructLoanTotalBalanceAmount / 100) * interest.FirstOrDefault().Rate;
                                newLoan.InterestAmount = interestAmount;
                                newLoan.PreviousBalanceAmount = 0;
                                newLoan.DeductionAmount = 0;
                                newLoan.NetAmount = loanReconstruct.ReconstructLoanTotalBalanceAmount + loanReconstruct.ReconstructLoanTotalPenaltyAmount + interestAmount;
                                newLoan.NetCollectionAmount = loanReconstruct.ReconstructLoanTotalBalanceAmount + loanReconstruct.ReconstructLoanTotalPenaltyAmount + interestAmount;
                                newLoan.TotalPaidAmount = 0;
                                newLoan.TotalPenaltyAmount = 0;
                                newLoan.TotalBalanceAmount = loanReconstruct.ReconstructLoanTotalBalanceAmount + loanReconstruct.ReconstructLoanTotalPenaltyAmount + interestAmount;
                                newLoan.IsReconstruct = false;
                                newLoan.IsRenew = false;
                                newLoan.IsLoanApplication = false;
                                newLoan.IsLoanReconstruct = true;
                                newLoan.IsLoanRenew = false;
                                newLoan.IsFullyPaid = false;
                                newLoan.IsLocked = false;
                                newLoan.CreatedByUserId = userId;
                                newLoan.CreatedDateTime = DateTime.Now;
                                newLoan.UpdatedByUserId = userId;
                                newLoan.UpdatedDateTime = DateTime.Now;
                                db.trnLoans.InsertOnSubmit(newLoan);
                                db.SubmitChanges();

                                return newLoan.Id;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}
