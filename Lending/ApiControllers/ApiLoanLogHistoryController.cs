using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiLoanLogHistoryController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan log history list
        [Authorize]
        [HttpGet]
        [Route("api/loanLogHistory/listByApplicantIdAndByLoanId/{applicantId}/{loanId}")]
        public List<Models.TrnLoanLogHistory> listLoanLogHistoryByApplicantIdAndByLoanId(String applicantId, String loanId)
        {
            var loanLogHistories = from d in db.trnLoanLogHistories
                                   where d.trnLoanApplication.ApplicantId == Convert.ToInt32(applicantId)
                                   && d.LoanId == Convert.ToInt32(loanId)
                                   select new Models.TrnLoanLogHistory
                                   {
                                       Id = d.Id,
                                       LoanId = d.LoanId,
                                       CollectibleDate = d.CollectibleDate.ToShortDateString(),
                                       NetAmount = d.NetAmount,
                                       CollectibleAmount = d.CollectibleAmount,
                                       Penalty = d.Penalty,
                                       IsPenalty = d.IsPenalty,
                                       PaidAmount = d.PaidAmount,
                                       PreviousBalance = d.PreviousBalance,
                                       CurrentBalance = d.CurrentBalance,
                                       BalanceNetAmount = d.BalanceNetAmount
                                   };

            return loanLogHistories.ToList();
        }
    }
}
