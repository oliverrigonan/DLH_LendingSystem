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
                                PenaltyAmount = d.PenaltyAmount
                            };

            return loanLines.ToList();
        }

        // loan list by loan date
        [Authorize]
        [HttpGet]
        [Route("api/loanLines/listByLoanId/byNotRenew/byNotReconstruct/{loanId}")]
        public List<Models.TrnLoanLines> listLoanLinesByLoanIdByNotRenewByNotReconstruct(String loanId)
        {
            var loanLines = from d in db.trnLoanLines
                            where d.LoanId == Convert.ToInt32(loanId)
                            && d.trnLoan.IsReconstruct == false
                            && d.trnLoan.IsRenew == false
                            select new Models.TrnLoanLines
                            {
                                Id = d.Id,
                                DayReference = d.DayReference,
                                CollectibleDate = d.CollectibleDate.ToShortDateString(),
                                CollectibleAmount = d.CollectibleAmount,
                                PaidAmount = d.PaidAmount,
                                PenaltyAmount = d.PenaltyAmount
                            };

            return loanLines.ToList();
        }

        // loan list by loan date
        [Authorize]
        [HttpGet]
        [Route("api/loanLines/listByCollectibleDateDate/byAreaId/{collectibleDate}/{areaId}")]
        public List<Models.TrnLoanLines> listLoanLinesByLoanIdByNotRenewByNotReconstruct(String collectibleDate, String areaId)
        {
            var loanLines = from d in db.trnLoanLines.OrderBy(d => d.trnLoan.mstApplicant.ApplicantLastName)
                            where d.CollectibleDate == Convert.ToDateTime(collectibleDate)
                            && d.trnLoan.mstApplicant.AreaId == Convert.ToInt32(areaId)
                            && d.trnLoan.IsReconstruct == false
                            && d.trnLoan.IsRenew == false
                            && d.trnLoan.IsLocked == true
                            select new Models.TrnLoanLines
                            {
                                Id = d.Id,
                                LoanId = d.LoanId,
                                DayReference = d.DayReference,
                                CollectibleDate = d.CollectibleDate.ToShortDateString(),
                                CollectibleAmount = d.CollectibleAmount,
                                PaidAmount = d.PaidAmount,
                                PenaltyAmount = d.PenaltyAmount,
                                Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                IsReconstruct = d.trnLoan.IsReconstruct,
                                IsRenew = d.trnLoan.IsRenew,
                                IsLoanApplication = d.trnLoan.IsLoanApplication,
                                IsLoanReconstruct = d.trnLoan.IsLoanReconstruct,
                                IsLoanRenew = d.trnLoan.IsLoanRenew,
                                DueDate = d.trnLoan.MaturityDate.ToShortDateString()
                            };

            return loanLines.ToList();
        }
    }
}
