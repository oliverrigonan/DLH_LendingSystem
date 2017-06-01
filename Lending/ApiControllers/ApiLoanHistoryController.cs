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
    public class ApiLoanHistoryController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan history
        [Authorize]
        [HttpGet]
        [Route("api/loanHistory/list/{applicantId}")]
        public List<Models.TrnLoanHistory> listLoanHistory(String applicantId)
        {
            var loanHistories = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                where d.ApplicantId == Convert.ToInt32(applicantId)
                                && d.IsLocked == true
                                select new Models.TrnLoanHistory
                                {
                                    Id = d.Id,
                                    LoanNumber = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
                                    LoanDate = d.LoanDate.ToShortDateString(),
                                    Particulars = d.Particulars,
                                    PreparedByUserId = d.PreparedByUserId,
                                    PreparedByUser = d.mstUser.FullName,
                                    TermId = d.TermId,
                                    Term = d.mstTerm.Term,
                                    TermNoOfDays = d.TermNoOfDays,
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
                                    CollectibleAmount = d.CollectibleAmount,
                                    TotalPaidAmount = d.TotalPaidAmount,
                                    TotalPenaltyAmount = d.TotalPenaltyAmount,
                                    TotalBalanceAmount = d.TotalBalanceAmount,
                                    IsLoanApplication = d.IsLoanApplication,
                                    IsLoanReconstruct = d.IsLoanReconstruct,
                                    IsLoanRenew = d.IsLoanRenew,
                                    IsLocked = d.IsLocked
                                };

            return loanHistories.ToList();
        }
    }
}
