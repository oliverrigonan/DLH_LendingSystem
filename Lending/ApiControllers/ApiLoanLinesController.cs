using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiLoanLinesController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan list by loan date
        [Authorize]
        [HttpGet]
        [Route("api/loanLines/listByLoanId/{loanId}")]
        public List<Models.TrnLoanLines> listLoanLinesByLoanId(String loanId)
        {
            var loanLines = from d in db.trnLoanLines
                                   where d.LoanId == Convert.ToInt32(loanId)
                                   select new Models.TrnLoanLines
                                   {
                                       Id = d.Id,
                                       DayReference = d.DayReference,
                                       CollectibleDate = d.CollectibleDate.ToShortDateString(),
                                       CollectibleAmount = d.CollectibleAmount,
                                       PaidAmount = d.PaidAmount,
                                       PenaltyAmount = d.PenaltyAmount,
                                       BalanceAmount = d.BalanceAmount,
                                       IsCleared = d.IsCleared
                                   };

            return loanLines.ToList();
        }
    }
}
