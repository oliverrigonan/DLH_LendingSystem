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
    public class ApiLoanRenewController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan renews list
        [Authorize]
        [HttpGet]
        [Route("api/loanRenew/list/ByLoanId/{loanId}")]
        public List<Models.TrnLoanRenew> listRenew(String loanId)
        {
            var renews = from d in db.trnLoanRenews.OrderByDescending(d => d.Id)
                         where d.LoanId == Convert.ToInt32(loanId)
                         select new Models.TrnLoanRenew
                         {
                             Id = d.Id,
                             LoanId = d.LoanId,
                             RenewLoanId = d.RenewLoanId,
                             RenewLoanNumber = d.trnLoan1.IsLoanApplication == true ? d.trnLoan1.IsReconstruct == true ? "LN - " + d.trnLoan1.LoanNumber + " (Reconstructed)" : "LN - " + d.trnLoan1.LoanNumber : d.trnLoan1.IsRenew == true ? "LN - " + d.trnLoan1.LoanNumber + " (Renewed)" : d.trnLoan1.IsLoanReconstruct == true ? d.trnLoan1.IsReconstruct == true ? "RC - " + d.trnLoan1.LoanNumber + " (Reconstructed)" : "RC - " + d.trnLoan1.LoanNumber : d.trnLoan1.IsRenew == true ? "RC - " + d.trnLoan1.LoanNumber + " (Renewed)" : d.trnLoan1.IsLoanRenew == true ? d.trnLoan1.IsReconstruct == true ? "RN - " + d.trnLoan1.LoanNumber + " (Reconstructed)" : "RN - " + d.trnLoan1.LoanNumber : d.trnLoan1.IsRenew == true ? "RN - " + d.trnLoan1.LoanNumber + " (Renewed)" : d.trnLoan1.LoanNumber,
                             RenewPrincipalAmount = d.trnLoan.PrincipalAmount,
                             RenewLoanTotalBalanceAmount = d.RenewLoanTotalBalanceAmount,
                             RenewLoanTotalPenaltyAmount = d.RenewLoanTotalPenaltyAmount
                         };

            return renews.ToList();
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

        // renew list by loan date
        [Authorize]
        [HttpGet]
        [Route("api/renew/listByLoanDate/{startLoanDate}/{endLoanDate}")]
        public List<Models.TrnLoan> listRenewByLoanDate(String startLoanDate, String endLoanDate)
        {
            var renews = from d in db.trnLoans.OrderByDescending(d => d.Id)
                         where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                         && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                         && d.IsLoanRenew == true
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
                             PreviousPenaltyAmount = d.PreviousPenaltyAmount,
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

            return renews.ToList();
        }

        // renew get by id
        [Authorize]
        [HttpGet]
        [Route("api/renew/getById/{id}")]
        public Models.TrnLoan getRenewById(String id)
        {
            var renew = from d in db.trnLoans
                        where d.Id == Convert.ToInt32(id)
                        && d.IsLoanRenew == true
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
                            PreviousPenaltyAmount = d.PreviousPenaltyAmount,
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

            return (Models.TrnLoan)renew.FirstOrDefault();
        }

        // add loan renew
        [Authorize]
        [HttpPost]
        [Route("api/loanRenew/add")]
        public Int32 addLoanRenew(Models.TrnLoanRenew loanRenew)
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
                        var loan = from d in db.trnLoans.OrderByDescending(d => d.Id) where d.IsLoanRenew == true select d;
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
                                                where d.Id == loanRenew.RenewLoanId
                                                where d.IsLocked == true
                                                select d;

                                if (existLoan.Any())
                                {
                                    Data.trnLoan newLoan = new Data.trnLoan();
                                    newLoan.LoanNumber = zeroFill(Convert.ToInt32(loanNumber), 10);
                                    newLoan.LoanDate = DateTime.Today;
                                    newLoan.ApplicantId = loanRenew.ApplicantId;
                                    newLoan.Particulars = "NA";
                                    newLoan.PreparedByUserId = userId;
                                    newLoan.TermId = term.FirstOrDefault().Id;
                                    newLoan.TermNoOfDays = term.FirstOrDefault().NoOfDays;
                                    newLoan.TermPaymentNoOfDays = term.FirstOrDefault().PaymentNoOfDays;
                                    newLoan.ForOverdue = null;
                                    newLoan.MaturityDate = DateTime.Today;
                                    newLoan.PrincipalAmount = loanRenew.RenewPrincipalAmount;
                                    newLoan.InterestId = interest.FirstOrDefault().Id;
                                    newLoan.InterestRate = interest.FirstOrDefault().Rate;
                                    Decimal interestAmount = (loanRenew.RenewPrincipalAmount / 100) * interest.FirstOrDefault().Rate;
                                    newLoan.InterestAmount = interestAmount;
                                    newLoan.PreviousBalanceAmount = loanRenew.RenewLoanTotalBalanceAmount;
                                    newLoan.PreviousPenaltyAmount = loanRenew.RenewLoanTotalPenaltyAmount;
                                    newLoan.DeductionAmount = 0;
                                    newLoan.NetAmount = loanRenew.RenewPrincipalAmount + loanRenew.RenewLoanTotalBalanceAmount + loanRenew.RenewLoanTotalPenaltyAmount + interestAmount;
                                    newLoan.NetCollectionAmount = loanRenew.RenewPrincipalAmount + loanRenew.RenewLoanTotalBalanceAmount + loanRenew.RenewLoanTotalPenaltyAmount + interestAmount;
                                    newLoan.TotalPaidAmount = 0;
                                    newLoan.TotalPenaltyAmount = 0;
                                    newLoan.TotalBalanceAmount = loanRenew.RenewPrincipalAmount + loanRenew.RenewLoanTotalBalanceAmount + loanRenew.RenewLoanTotalPenaltyAmount + interestAmount;
                                    newLoan.IsReconstruct = false;
                                    newLoan.IsRenew = false;
                                    newLoan.IsLoanApplication = false;
                                    newLoan.IsLoanReconstruct = false;
                                    newLoan.IsLoanRenew = true;
                                    newLoan.IsFullyPaid = false;
                                    newLoan.IsLocked = false;
                                    newLoan.CreatedByUserId = userId;
                                    newLoan.CreatedDateTime = DateTime.Now;
                                    newLoan.UpdatedByUserId = userId;
                                    newLoan.UpdatedDateTime = DateTime.Now;
                                    db.trnLoans.InsertOnSubmit(newLoan);
                                    db.SubmitChanges();

                                    Data.trnLoanRenew newLoanRenew = new Data.trnLoanRenew();
                                    newLoanRenew.LoanId = newLoan.Id;
                                    newLoanRenew.RenewLoanId = loanRenew.RenewLoanId;
                                    newLoanRenew.RenewLoanTotalBalanceAmount = loanRenew.RenewLoanTotalBalanceAmount;
                                    newLoanRenew.RenewLoanTotalPenaltyAmount = loanRenew.RenewLoanTotalPenaltyAmount;
                                    db.trnLoanRenews.InsertOnSubmit(newLoanRenew);
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
    }
}
