using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiCollectionController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // collection get by id
        [Authorize]
        [HttpGet]
        [Route("api/collection/getById/{id}")]
        public Models.TrnCollection getCollectionById(String id)
        {
            var collection = from d in db.trnCollections
                             where d.Id == Convert.ToInt32(id)
                             select new Models.TrnCollection
                               {
                                   Id = d.Id,
                                   CollectionNumber = d.CollectionNumber,
                                   CollectionDate = d.CollectionDate.ToShortDateString(),
                                   LoanId = d.LoanId,
                                   LoanNumber = d.trnLoanApplication.LoanNumber,
                                   LoanDate = d.trnLoanApplication.LoanDate.ToShortDateString(),
                                   MaturityDate = d.trnLoanApplication.MaturityDate.ToShortDateString(),
                                   PrincipalAmount = d.trnLoanApplication.PrincipalAmount,
                                   Interest = d.trnLoanApplication.mstInterest.Interest,
                                   InterestRate = d.trnLoanApplication.InterestRate,
                                   InterestAmount = d.trnLoanApplication.InterestAmount,
                                   TotalDeductionAmount = d.trnLoanApplication.ProcessingFeeAmountDeduction + d.trnLoanApplication.PassbookAmountDeduction + d.trnLoanApplication.BalanceAmountDeduction + d.trnLoanApplication.PenaltyAmountDeduction + d.trnLoanApplication.LateIntAmountDeduction + d.trnLoanApplication.AdvanceAmountDeduction + d.trnLoanApplication.RequirementsAmountDeduction + d.trnLoanApplication.InsuranceIPIorPPIAmountDeduction,
                                   NetAmount = d.trnLoanApplication.NetAmount,
                                   Applicant = d.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + (d.trnLoanApplication.mstApplicant.ApplicantMiddleName != null ? d.trnLoanApplication.mstApplicant.ApplicantMiddleName : " "),
                                   Area = d.trnLoanApplication.mstApplicant.mstArea.Area,
                                   TermId = d.TermId,
                                   Term = d.mstTerm.Term,
                                   TermNoOfDays = d.TermNoOfDays,
                                   TermNoOfAllowanceDays = d.TermNoOfAllowanceDays,
                                   IsFullyPaid = d.IsFullyPaid,
                                   IsOverdue = d.IsOverdue,
                               };

            return (Models.TrnCollection)collection.FirstOrDefault();
        }
    }
}
