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
                                       CollectionDate = d.CollectionDate.ToShortDateString(),
                                       NetAmount = d.NetAmount,
                                       CollectibleAmount = d.CollectibleAmount,
                                       PenaltyAmount = d.PenaltyAmount,
                                       PaidAmount = d.PaidAmount,
                                       PreviousBalanceAmount = d.PreviousBalanceAmount,
                                       CurrentBalanceAmount = d.CurrentBalanceAmount,
                                       IsCleared = d.IsCleared,
                                       IsPenalty = d.IsPenalty,
                                       IsOverdue = d.IsOverdue,
                                       IsFullyPaid = d.IsFullyPaid
                                   };

            return loanLogHistories.ToList();
        }

        // loan log history by collectible date and by area
        [Authorize]
        [HttpGet]
        [Route("api/loanLogHistory/listByCollectionDateAndByAreaId/{collectionDate}/{areaId}")]
        public List<Models.TrnLoanLogHistory> listLoanLogHistoryByCollectionDateAndByAreaId(String collectionDate, String areaId)
        {
            var loanLogHistories = from d in db.trnLoanLogHistories
                                   where d.CollectionDate == Convert.ToDateTime(collectionDate)
                                   && d.trnLoanApplication.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                   select new Models.TrnLoanLogHistory
                                   {
                                       Id = d.Id,
                                       LoanId = d.LoanId,
                                       LoanNumber = d.trnLoanApplication.LoanNumber,
                                       Applicant = d.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + d.trnLoanApplication.mstApplicant.ApplicantMiddleName,
                                       Area = d.trnLoanApplication.mstApplicant.mstArea.Area,
                                       CollectionDate = d.CollectionDate.ToShortDateString(),
                                       NetAmount = d.NetAmount,
                                       CollectibleAmount = d.CollectibleAmount,
                                       PenaltyAmount = d.PenaltyAmount,
                                       PaidAmount = d.PaidAmount,
                                       PreviousBalanceAmount = d.PreviousBalanceAmount,
                                       CurrentBalanceAmount = d.CurrentBalanceAmount,
                                       IsCleared = d.IsCleared,
                                       IsPenalty = d.IsPenalty,
                                       IsOverdue = d.IsOverdue,
                                       IsFullyPaid = d.IsFullyPaid
                                   };

            return loanLogHistories.ToList();
        }

        // update branch
        [Authorize]
        [HttpPut]
        [Route("api/loanLogHistory/isCleared/updateByIdAndByLoanId/{id}/{loanId}")]
        public HttpResponseMessage updateIsClearedLoanLogHistory(String id, String loanId)
        {
            try
            {
                var loanApplications = from d in db.trnLoanApplications where d.Id == Convert.ToInt32(loanId) select d;
                if (loanApplications.Any())
                {
                    if (loanApplications.FirstOrDefault().IsLocked)
                    {
                        var loanLogHistories = from d in db.trnLoanLogHistories where d.Id == Convert.ToInt32(id) select d;
                        if (loanLogHistories.Any())
                        {
                            if (loanLogHistories.FirstOrDefault().CurrentBalanceAmount > 0)
                            {
                                Data.trnCollectionLogHistory newCollectionLogHistory = new Data.trnCollectionLogHistory();
                                newCollectionLogHistory.LoanId = Convert.ToInt32(loanId);
                                newCollectionLogHistory.CollectionDate = loanLogHistories.FirstOrDefault().CollectionDate;
                                newCollectionLogHistory.CollectibleAmount = loanLogHistories.FirstOrDefault().CollectibleAmount;
                                newCollectionLogHistory.PaidAmount = loanLogHistories.FirstOrDefault().CurrentBalanceAmount;
                                newCollectionLogHistory.IsCleared = true;
                                newCollectionLogHistory.IsPenalty = loanLogHistories.FirstOrDefault().IsPenalty;
                                newCollectionLogHistory.IsOverdue = loanLogHistories.FirstOrDefault().IsOverdue;
                                newCollectionLogHistory.IsFullyPaid = loanLogHistories.FirstOrDefault().IsFullyPaid;
                                newCollectionLogHistory.CollectorId = loanApplications.FirstOrDefault().CollectorId;
                                db.trnCollectionLogHistories.InsertOnSubmit(newCollectionLogHistory);
                                db.SubmitChanges();

                                var updateLoanLogHistory = loanLogHistories.FirstOrDefault();
                                updateLoanLogHistory.PaidAmount = loanLogHistories.FirstOrDefault().CurrentBalanceAmount;
                                updateLoanLogHistory.PreviousBalanceAmount = loanLogHistories.FirstOrDefault().PreviousBalanceAmount;
                                updateLoanLogHistory.CurrentBalanceAmount = 0;
                                updateLoanLogHistory.IsCleared = true;
                                db.SubmitChanges();

                                var loanLogHistoryByCollectionDate = from d in db.trnLoanLogHistories where d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                if (loanLogHistoryByCollectionDate.Any())
                                {
                                    var updateLoanLogHistoryByCollectionDate = loanLogHistoryByCollectionDate.FirstOrDefault();
                                    updateLoanLogHistoryByCollectionDate.CurrentBalanceAmount = loanLogHistoryByCollectionDate.FirstOrDefault().CollectibleAmount;
                                    db.SubmitChanges();
                                }

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.NotFound);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

    }
}
