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
            var reconstructs = from d in db.trnLoanReconstructs
                               where d.LoanId == Convert.ToInt32(loanId)
                               select new Models.TrnLoanReconstruct
                               {
                                   Id = d.Id,
                                   LoanId = d.LoanId,
                                   ReconstructLoanId = d.ReconstructLoanId,
                                   ReconstructLoanNumber = d.trnLoan1.LoanNumber,
                                   ReconstuctLoanTotalBalanceAmount = d.ReconstuctLoanTotalBalanceAmount
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
                        var loan = from d in db.trnLoans.OrderByDescending(d => d.Id) select d;
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
                                var loanApplicationReconstruct = from d in db.trnLoanReconstructs
                                                                 where d.ReconstructLoanId == loanReconstruct.ReconstructLoanId
                                                                 select d;

                                if (!loanApplicationReconstruct.Any())
                                {
                                    var existLoan = from d in db.trnLoans
                                                    where d.Id == loanReconstruct.ReconstructLoanId
                                                    where d.IsLocked == true
                                                    select d;

                                    if (existLoan.Any())
                                    {
                                        var updateLoan = existLoan.FirstOrDefault();
                                        updateLoan.IsReconstruct = true;
                                        db.SubmitChanges();

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
                                        newLoan.PrincipalAmount = loanReconstruct.ReconstuctLoanTotalBalanceAmount;
                                        newLoan.IsAdvanceInterest = false;
                                        newLoan.InterestId = interest.FirstOrDefault().Id;
                                        newLoan.InterestRate = interest.FirstOrDefault().Rate;
                                        Decimal interestAmount = (loanReconstruct.ReconstuctLoanTotalBalanceAmount / 100) * interest.FirstOrDefault().Rate;
                                        newLoan.InterestAmount = interestAmount;
                                        newLoan.DeductionAmount = 0;
                                        newLoan.NetAmount = loanReconstruct.ReconstuctLoanTotalBalanceAmount + interestAmount;
                                        newLoan.TotalPaidAmount = 0;
                                        newLoan.TotalPenaltyAmount = 0;
                                        newLoan.TotalBalanceAmount = loanReconstruct.ReconstuctLoanTotalBalanceAmount + interestAmount;
                                        newLoan.IsReconstruct = false;
                                        newLoan.IsRenew = false;
                                        newLoan.IsFullyPaid = false;
                                        newLoan.IsLoanApplication = false;
                                        newLoan.IsLoanReconstruct = true;
                                        newLoan.IsLoanRenew = false;
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
                                        newLoanReconstruct.ReconstuctLoanTotalBalanceAmount = loanReconstruct.ReconstuctLoanTotalBalanceAmount;
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
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return 0;
            }
        }
    }
}
