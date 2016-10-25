using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                                       DayNumber = d.DayNumber,
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
                                       IsFullyPaid = d.trnLoanApplication.IsFullyPaid,
                                       IsAction = d.IsAction,
                                       CollectorId = d.trnLoanApplication.CollectorId,
                                       Collector = d.trnLoanApplication.mstCollector.Collector
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
                                       DayNumber = d.DayNumber,
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
                                       IsFullyPaid = d.trnLoanApplication.IsFullyPaid,
                                       IsAction = d.IsAction,
                                       IsDueDate = d.IsDueDate
                                   };

            return loanLogHistories.ToList();
        }

        // clear loan log history
        [Authorize]
        [HttpPut]
        [Route("api/loanLogHistory/cleared/updateByIdAndByLoanId/{id}/{loanId}")]
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
                                if (!loanLogHistories.FirstOrDefault().IsCleared)
                                {
                                    if (loanLogHistories.FirstOrDefault().IsAction)
                                    {
                                        if (loanLogHistories.FirstOrDefault().CollectionDate <= DateTime.Today)
                                        {
                                            var collectionLogHistories = from d in db.trnCollectionLogHistories
                                                                         where d.LoanLogHistoryId == loanLogHistories.FirstOrDefault().Id
                                                                         select d;

                                            if (collectionLogHistories.Any())
                                            {
                                                var updateCollectionLogHistory = collectionLogHistories.FirstOrDefault();
                                                updateCollectionLogHistory.PaidAmount = loanLogHistories.FirstOrDefault().CurrentBalanceAmount;
                                                db.SubmitChanges();
                                            }
                                            else
                                            {
                                                Data.trnCollectionLogHistory newCollectionLogHistory = new Data.trnCollectionLogHistory();
                                                newCollectionLogHistory.LoanLogHistoryId = loanLogHistories.FirstOrDefault().Id;
                                                newCollectionLogHistory.PaidAmount = loanLogHistories.FirstOrDefault().CurrentBalanceAmount;
                                                newCollectionLogHistory.CollectorId = loanApplications.FirstOrDefault().CollectorId;
                                                newCollectionLogHistory.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
                                                db.trnCollectionLogHistories.InsertOnSubmit(newCollectionLogHistory);
                                                db.SubmitChanges();
                                            }

                                            var updateLoanLogHistory = loanLogHistories.FirstOrDefault();
                                            updateLoanLogHistory.PaidAmount = loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount;
                                            updateLoanLogHistory.CurrentBalanceAmount = 0;
                                            updateLoanLogHistory.PenaltyAmount = 0;
                                            updateLoanLogHistory.IsPenalty = false;
                                            updateLoanLogHistory.IsCleared = true;
                                            db.SubmitChanges();

                                            var loanLogHistoryByCollectionDatePrevious = from d in db.trnLoanLogHistories where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(-1) select d;
                                            if (loanLogHistoryByCollectionDatePrevious.Any())
                                            {
                                                var updateLoanLogHistoryByCollectionDatePrevious = loanLogHistoryByCollectionDatePrevious.FirstOrDefault();
                                                updateLoanLogHistoryByCollectionDatePrevious.IsAction = false;
                                                db.SubmitChanges();

                                                var loanLogHistoryByCollectionDate = from d in db.trnLoanLogHistories where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                                if (loanLogHistoryByCollectionDate.Any())
                                                {
                                                    var updateLoanLogHistoryByCollectionDate = loanLogHistoryByCollectionDate.FirstOrDefault();
                                                    updateLoanLogHistoryByCollectionDate.PreviousBalanceAmount = 0;
                                                    updateLoanLogHistoryByCollectionDate.CurrentBalanceAmount = loanLogHistoryByCollectionDate.FirstOrDefault().CollectibleAmount;
                                                    updateLoanLogHistoryByCollectionDate.IsAction = true;
                                                    db.SubmitChanges();
                                                }
                                                else
                                                {
                                                    var updateLoanApplicationsForFullyPaid = loanApplications.FirstOrDefault();
                                                    updateLoanApplicationsForFullyPaid.IsFullyPaid = true;
                                                    db.SubmitChanges();
                                                }
                                            }
                                            else
                                            {
                                                var loanLogHistoryByCollectionDate = from d in db.trnLoanLogHistories where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                                if (loanLogHistoryByCollectionDate.Any())
                                                {
                                                    var updateLoanLogHistoryByCollectionDate = loanLogHistoryByCollectionDate.FirstOrDefault();
                                                    updateLoanLogHistoryByCollectionDate.PreviousBalanceAmount = 0;
                                                    updateLoanLogHistoryByCollectionDate.CurrentBalanceAmount = loanLogHistoryByCollectionDate.FirstOrDefault().CollectibleAmount;
                                                    updateLoanLogHistoryByCollectionDate.IsAction = true;
                                                    db.SubmitChanges();
                                                }
                                                else
                                                {
                                                    var updateLoanApplicationsForFullyPaid = loanApplications.FirstOrDefault();
                                                    updateLoanApplicationsForFullyPaid.IsFullyPaid = true;
                                                    db.SubmitChanges();
                                                }
                                            }

                                            return Request.CreateResponse(HttpStatusCode.OK);
                                        }
                                        else
                                        {
                                            return Request.CreateResponse(HttpStatusCode.BadRequest, "The actions can be applied when the collection date matches the date today.");
                                        }
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot apply actions by this time.");
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The collection was already cleared. Click the Option button for update changes.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "No current balance to be cleared.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server to apply some actions.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Please lock the loan application first before procceding the collection process.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server to apply some actions.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something went wrong from the server. Please contact the administrator.");
            }
        }

        // absent loan log history
        [Authorize]
        [HttpPut]
        [Route("api/loanLogHistory/absent/updateByIdAndByLoanId/{id}/{loanId}")]
        public HttpResponseMessage updateIsPenaltyLoanLogHistory(String id, String loanId)
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
                            if (!loanLogHistories.FirstOrDefault().IsPenalty)
                            {
                                if (loanLogHistories.FirstOrDefault().IsAction)
                                {
                                    if (loanLogHistories.FirstOrDefault().CollectionDate <= DateTime.Today)
                                    {
                                        var collectionLogHistories = from d in db.trnCollectionLogHistories
                                                                     where d.LoanLogHistoryId == loanLogHistories.FirstOrDefault().Id
                                                                     select d;

                                        if (collectionLogHistories.Any())
                                        {
                                            var updateCollectionLogHistory = collectionLogHistories.FirstOrDefault();
                                            updateCollectionLogHistory.PaidAmount = loanLogHistories.FirstOrDefault().CurrentBalanceAmount;
                                            db.SubmitChanges();
                                        }
                                        else
                                        {
                                            Data.trnCollectionLogHistory newCollectionLogHistory = new Data.trnCollectionLogHistory();
                                            newCollectionLogHistory.LoanLogHistoryId = loanLogHistories.FirstOrDefault().Id;
                                            newCollectionLogHistory.PaidAmount = loanLogHistories.FirstOrDefault().CurrentBalanceAmount;
                                            newCollectionLogHistory.CollectorId = loanApplications.FirstOrDefault().CollectorId;
                                            newCollectionLogHistory.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
                                            db.trnCollectionLogHistories.InsertOnSubmit(newCollectionLogHistory);
                                            db.SubmitChanges();
                                        }

                                        var penaltyValue = 10;
                                        var previousDay1 = from d in db.trnLoanLogHistories
                                                           where d.LoanId == loanLogHistories.FirstOrDefault().LoanId
                                                           && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(-1)
                                                           select d;

                                        if (previousDay1.Any())
                                        {
                                            if (previousDay1.FirstOrDefault().IsPenalty)
                                            {
                                                if (previousDay1.FirstOrDefault().PenaltyAmount == 10)
                                                {
                                                    var previousDay2 = from d in db.trnLoanLogHistories
                                                                       where d.LoanId == loanLogHistories.FirstOrDefault().LoanId
                                                                       && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(-2)
                                                                       select d;

                                                    if (previousDay2.Any())
                                                    {
                                                        if (previousDay2.FirstOrDefault().IsPenalty)
                                                        {
                                                            if (previousDay2.FirstOrDefault().PenaltyAmount == 10)
                                                            {
                                                                penaltyValue = 20;
                                                            }
                                                            else
                                                            {
                                                                penaltyValue = 10;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        var updateLoanLogHistory = loanLogHistories.FirstOrDefault();
                                        updateLoanLogHistory.PenaltyAmount = penaltyValue;
                                        updateLoanLogHistory.PaidAmount = 0;
                                        updateLoanLogHistory.CurrentBalanceAmount = loanLogHistories.FirstOrDefault().CollectibleAmount + penaltyValue + loanLogHistories.FirstOrDefault().PreviousBalanceAmount;
                                        updateLoanLogHistory.IsPenalty = true;
                                        updateLoanLogHistory.IsCleared = false;
                                        db.SubmitChanges();

                                        var loanLogHistoryByCollectionDatePrevious = from d in db.trnLoanLogHistories where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(-1) select d;
                                        if (loanLogHistoryByCollectionDatePrevious.Any())
                                        {
                                            var updateLoanLogHistoryByCollectionDatePrevious = loanLogHistoryByCollectionDatePrevious.FirstOrDefault();
                                            updateLoanLogHistoryByCollectionDatePrevious.IsAction = false;
                                            db.SubmitChanges();

                                            var loanLogHistoryByCollectionDate = from d in db.trnLoanLogHistories where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                            if (loanLogHistoryByCollectionDate.Any())
                                            {
                                                var updateLoanLogHistoryByCollectionDate = loanLogHistoryByCollectionDate.FirstOrDefault();
                                                updateLoanLogHistoryByCollectionDate.PreviousBalanceAmount = loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PenaltyAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount;
                                                updateLoanLogHistoryByCollectionDate.CurrentBalanceAmount = loanLogHistoryByCollectionDate.FirstOrDefault().CollectibleAmount + (loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PenaltyAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount);
                                                updateLoanLogHistoryByCollectionDate.IsAction = true;
                                                db.SubmitChanges();
                                            }
                                            else
                                            {
                                                var updateLoanApplicationsForFullyPaid = loanApplications.FirstOrDefault();
                                                updateLoanApplicationsForFullyPaid.IsFullyPaid = false;
                                                db.SubmitChanges();
                                            }
                                        }
                                        else
                                        {
                                            var loanLogHistoryByCollectionDate = from d in db.trnLoanLogHistories where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                            if (loanLogHistoryByCollectionDate.Any())
                                            {
                                                var updateLoanLogHistoryByCollectionDate = loanLogHistoryByCollectionDate.FirstOrDefault();
                                                updateLoanLogHistoryByCollectionDate.PreviousBalanceAmount = loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PenaltyAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount;
                                                updateLoanLogHistoryByCollectionDate.CurrentBalanceAmount = loanLogHistoryByCollectionDate.FirstOrDefault().CollectibleAmount + (loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PenaltyAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount);
                                                updateLoanLogHistoryByCollectionDate.IsAction = true;
                                                db.SubmitChanges();
                                            }
                                            else
                                            {
                                                var updateLoanApplicationsForFullyPaid = loanApplications.FirstOrDefault();
                                                updateLoanApplicationsForFullyPaid.IsFullyPaid = false;
                                                db.SubmitChanges();
                                            }
                                        }

                                        return Request.CreateResponse(HttpStatusCode.OK);
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest, "The actions can be applied when the collection date matches the date today.");
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot apply actions by this time.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "The collection was already absent. Click the Option button for update changes.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server to apply some actions.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Please lock the loan application first before procceding the collection process.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server to apply some actions.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something went wrong from the server. Please contact the administrator.");
            }
        }

        // undo changes loan log history
        [Authorize]
        [HttpPut]
        [Route("api/loanLogHistory/undoChanges/updateByIdAndByLoanId/{id}/{loanId}")]
        public HttpResponseMessage undoChangesLoanLogHistory(String id, String loanId)
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
                            if (loanLogHistories.FirstOrDefault().IsAction)
                            {
                                if (loanLogHistories.FirstOrDefault().CollectionDate <= DateTime.Today)
                                {
                                    var collectionLogHistories = from d in db.trnCollectionLogHistories
                                                                 where d.LoanLogHistoryId == loanLogHistories.FirstOrDefault().Id
                                                                 select d;

                                    if (collectionLogHistories.Any())
                                    {
                                        db.trnCollectionLogHistories.DeleteOnSubmit(collectionLogHistories.First());
                                        db.SubmitChanges();
                                    }

                                    var loanLogHistoryByCollectionDatePrevious = from d in db.trnLoanLogHistories where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(-1) select d;
                                    if (loanLogHistoryByCollectionDatePrevious.Any())
                                    {
                                        var updateLoanLogHistory = loanLogHistories.FirstOrDefault();
                                        updateLoanLogHistory.PenaltyAmount = 0;
                                        updateLoanLogHistory.PaidAmount = 0;
                                        updateLoanLogHistory.PreviousBalanceAmount = loanLogHistoryByCollectionDatePrevious.FirstOrDefault().CurrentBalanceAmount;
                                        updateLoanLogHistory.CurrentBalanceAmount = loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistoryByCollectionDatePrevious.FirstOrDefault().CurrentBalanceAmount;
                                        updateLoanLogHistory.PenaltyAmount = 0;
                                        updateLoanLogHistory.IsCleared = false;
                                        updateLoanLogHistory.IsPenalty = false;
                                        updateLoanLogHistory.IsAction = true;
                                        db.SubmitChanges();

                                        var updateLoanLogHistoryByCollectionDatePrevious = loanLogHistoryByCollectionDatePrevious.FirstOrDefault();
                                        updateLoanLogHistoryByCollectionDatePrevious.IsAction = true;
                                        db.SubmitChanges();

                                        var loanLogHistoryByCollectionDate = from d in db.trnLoanLogHistories where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                        if (loanLogHistoryByCollectionDate.Any())
                                        {
                                            var updateLoanLogHistoryByCollectionDate = loanLogHistoryByCollectionDate.FirstOrDefault();
                                            updateLoanLogHistoryByCollectionDate.PreviousBalanceAmount = 0;
                                            updateLoanLogHistoryByCollectionDate.CurrentBalanceAmount = 0;
                                            updateLoanLogHistoryByCollectionDate.IsAction = false;
                                            db.SubmitChanges();
                                        }
                                        else
                                        {
                                            var updateLoanApplicationsForFullyPaid = loanApplications.FirstOrDefault();
                                            updateLoanApplicationsForFullyPaid.IsFullyPaid = false;
                                            db.SubmitChanges();
                                        }
                                    }
                                    else
                                    {
                                        var updateLoanLogHistory = loanLogHistories.FirstOrDefault();
                                        updateLoanLogHistory.PenaltyAmount = 0;
                                        updateLoanLogHistory.PaidAmount = 0;
                                        updateLoanLogHistory.PreviousBalanceAmount = 0;
                                        updateLoanLogHistory.CurrentBalanceAmount = loanLogHistories.FirstOrDefault().CollectibleAmount;
                                        updateLoanLogHistory.PenaltyAmount = 0;
                                        updateLoanLogHistory.IsCleared = false;
                                        updateLoanLogHistory.IsPenalty = false;
                                        updateLoanLogHistory.IsAction = true;
                                        db.SubmitChanges();

                                        var loanLogHistoryByCollectionDate = from d in db.trnLoanLogHistories where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                        if (loanLogHistoryByCollectionDate.Any())
                                        {
                                            var updateLoanLogHistoryByCollectionDate = loanLogHistoryByCollectionDate.FirstOrDefault();
                                            updateLoanLogHistoryByCollectionDate.PreviousBalanceAmount = 0;
                                            updateLoanLogHistoryByCollectionDate.CurrentBalanceAmount = 0;
                                            updateLoanLogHistoryByCollectionDate.IsAction = false;
                                            db.SubmitChanges();
                                        }
                                        else
                                        {
                                            var updateLoanApplicationsForFullyPaid = loanApplications.FirstOrDefault();
                                            updateLoanApplicationsForFullyPaid.IsFullyPaid = false;
                                            db.SubmitChanges();
                                        }
                                    }

                                    return Request.CreateResponse(HttpStatusCode.OK);
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The actions can be applied when the collection date matches the date today.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot apply actions by this time.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server to apply some actions.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Please lock the loan application first before procceding the collection process.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server to apply some actions.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something went wrong from the server. Please contact the administrator.");
            }
        }

        // loan log history get penalty value
        [Authorize]
        [HttpGet]
        [Route("api/loanLogHistory/getPenaltyValue/{loanId}/{collectionDate}")]
        public Decimal getLoanLogHistoryPenaltyValue(String loanId, String collectionDate)
        {

            var penaltyValue = 10;
            var previousDay1 = from d in db.trnLoanLogHistories
                               where d.LoanId == Convert.ToInt32(loanId)
                               && d.CollectionDate == Convert.ToDateTime(collectionDate).Date.AddDays(-1)
                               select d;

            if (previousDay1.Any())
            {
                if (previousDay1.FirstOrDefault().IsPenalty)
                {
                    if (previousDay1.FirstOrDefault().PenaltyAmount == 10)
                    {
                        var previousDay2 = from d in db.trnLoanLogHistories
                                           where d.LoanId == Convert.ToInt32(loanId)
                                           && d.CollectionDate == Convert.ToDateTime(collectionDate).Date.AddDays(-2)
                                           select d;

                        if (previousDay2.Any())
                        {
                            if (previousDay2.FirstOrDefault().IsPenalty)
                            {
                                if (previousDay2.FirstOrDefault().PenaltyAmount == 10)
                                {
                                    penaltyValue = 20;
                                }
                                else
                                {
                                    penaltyValue = 10;
                                }
                            }
                        }
                    }
                }
            }

            return penaltyValue;
        }

        // loan log history get previous balance
        [Authorize]
        [HttpGet]
        [Route("api/loanLogHistory/getPreviousBalance/{loanId}/{collectionDate}")]
        public Decimal getLoanLogHistoryPreviousBalance(String loanId, String collectionDate)
        {
            var prevoiusDay = from d in db.trnLoanLogHistories where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == Convert.ToDateTime(collectionDate).Date.AddDays(-1) select d;

            Decimal previousBalance = 0;
            if (prevoiusDay.Any())
            {
                previousBalance = prevoiusDay.FirstOrDefault().CurrentBalanceAmount;
            }

            return previousBalance;
        }

        // loan log history get fully payment
        [Authorize]
        [HttpGet]
        [Route("api/loanLogHistory/getIsFullyPaid/{loanId}")]
        public Boolean getLoanLogHistoryIsFullPaid(String loanId)
        {
            var isFullyPaid = from d in db.trnLoanApplications where d.Id == Convert.ToInt32(loanId) select d;

            Boolean isFullyPaidValue = false;
            if (isFullyPaid.Any())
            {
                isFullyPaidValue = isFullyPaid.FirstOrDefault().IsFullyPaid;
            }

            return isFullyPaidValue;
        }

        // partial payment loan log history
        [Authorize]
        [HttpPut]
        [Route("api/loanLogHistory/partialPayment/updateByIdAndByLoanId/{id}/{loanId}")]
        public HttpResponseMessage updatePartialPaymentLoanLogHistory(String id, String loanId, Models.TrnLoanLogHistory loanLogHistory)
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
                                if (!loanLogHistories.FirstOrDefault().IsCleared)
                                {
                                    if (loanLogHistories.FirstOrDefault().IsAction)
                                    {
                                        if (loanLogHistories.FirstOrDefault().CollectionDate <= DateTime.Today)
                                        {
                                            if (loanLogHistories.FirstOrDefault().CurrentBalanceAmount >= loanLogHistory.PaidAmount)
                                            {
                                                var collectionLogHistories = from d in db.trnCollectionLogHistories
                                                                             where d.LoanLogHistoryId == loanLogHistories.FirstOrDefault().Id
                                                                             select d;

                                                if (collectionLogHistories.Any())
                                                {
                                                    var updateCollectionLogHistory = collectionLogHistories.FirstOrDefault();
                                                    updateCollectionLogHistory.PaidAmount = loanLogHistory.PaidAmount;
                                                    db.SubmitChanges();
                                                }
                                                else
                                                {
                                                    Data.trnCollectionLogHistory newCollectionLogHistory = new Data.trnCollectionLogHistory();
                                                    newCollectionLogHistory.LoanLogHistoryId = loanLogHistories.FirstOrDefault().Id;
                                                    newCollectionLogHistory.PaidAmount = loanLogHistory.PaidAmount;
                                                    newCollectionLogHistory.CollectorId = loanApplications.FirstOrDefault().CollectorId;
                                                    newCollectionLogHistory.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
                                                    db.trnCollectionLogHistories.InsertOnSubmit(newCollectionLogHistory);
                                                    db.SubmitChanges();
                                                }

                                                var isClearedValue = false;
                                                if (loanLogHistories.FirstOrDefault().CurrentBalanceAmount - loanLogHistory.PaidAmount == 0)
                                                {
                                                    isClearedValue = true;
                                                }

                                                var updateLoanLogHistory = loanLogHistories.FirstOrDefault();
                                                updateLoanLogHistory.PaidAmount = loanLogHistory.PaidAmount;
                                                updateLoanLogHistory.CurrentBalanceAmount = (loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount) - loanLogHistory.PaidAmount;
                                                updateLoanLogHistory.IsCleared = isClearedValue;
                                                db.SubmitChanges();

                                                var loanLogHistoryByCollectionDatePrevious = from d in db.trnLoanLogHistories where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(-1) select d;
                                                if (loanLogHistoryByCollectionDatePrevious.Any())
                                                {
                                                    var updateLoanLogHistoryByCollectionDatePrevious = loanLogHistoryByCollectionDatePrevious.FirstOrDefault();
                                                    updateLoanLogHistoryByCollectionDatePrevious.IsAction = false;
                                                    db.SubmitChanges();

                                                    var loanLogHistoryByCollectionDate = from d in db.trnLoanLogHistories where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                                    if (loanLogHistoryByCollectionDate.Any())
                                                    {
                                                        var updateLoanLogHistoryByCollectionDate = loanLogHistoryByCollectionDate.FirstOrDefault();
                                                        updateLoanLogHistoryByCollectionDate.PreviousBalanceAmount = (loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount) - loanLogHistory.PaidAmount;
                                                        updateLoanLogHistoryByCollectionDate.CurrentBalanceAmount = loanLogHistoryByCollectionDate.FirstOrDefault().CollectibleAmount + ((loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount) - loanLogHistory.PaidAmount);
                                                        updateLoanLogHistoryByCollectionDate.IsAction = true;
                                                        db.SubmitChanges();
                                                    }
                                                    else
                                                    {
                                                        if (loanLogHistories.FirstOrDefault().CurrentBalanceAmount == 0)
                                                        {
                                                            var updateLoanApplicationsForFullyPaid = loanApplications.FirstOrDefault();
                                                            updateLoanApplicationsForFullyPaid.IsFullyPaid = true;
                                                            db.SubmitChanges();
                                                        }
                                                        else
                                                        {
                                                            Data.trnLoanLogHistory newLoanLogHistory = new Data.trnLoanLogHistory();
                                                            newLoanLogHistory.LoanId = Convert.ToInt32(loanId);
                                                            newLoanLogHistory.DayNumber = loanLogHistories.FirstOrDefault().DayNumber + 1;
                                                            newLoanLogHistory.CollectionDate = Convert.ToDateTime(loanLogHistories.FirstOrDefault().CollectionDate).Date.AddDays(1);
                                                            newLoanLogHistory.NetAmount = loanLogHistories.FirstOrDefault().NetAmount;
                                                            newLoanLogHistory.CollectibleAmount = 0;
                                                            newLoanLogHistory.PenaltyAmount = 0;
                                                            newLoanLogHistory.PaidAmount = 0;
                                                            newLoanLogHistory.PreviousBalanceAmount = (loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount) - loanLogHistory.PaidAmount;
                                                            newLoanLogHistory.CurrentBalanceAmount = (loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount) - loanLogHistory.PaidAmount;
                                                            newLoanLogHistory.IsCleared = false;
                                                            newLoanLogHistory.IsPenalty = false;
                                                            newLoanLogHistory.IsAction = true;
                                                            newLoanLogHistory.IsOverdue = false;
                                                            newLoanLogHistory.IsDueDate = false;
                                                            db.trnLoanLogHistories.InsertOnSubmit(newLoanLogHistory);
                                                            db.SubmitChanges();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var loanLogHistoryByCollectionDate = from d in db.trnLoanLogHistories where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                                    if (loanLogHistoryByCollectionDate.Any())
                                                    {
                                                        var updateLoanLogHistoryByCollectionDate = loanLogHistoryByCollectionDate.FirstOrDefault();
                                                        updateLoanLogHistoryByCollectionDate.PreviousBalanceAmount = (loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount) - loanLogHistory.PaidAmount;
                                                        updateLoanLogHistoryByCollectionDate.CurrentBalanceAmount = loanLogHistoryByCollectionDate.FirstOrDefault().CollectibleAmount + ((loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount) - loanLogHistory.PaidAmount);
                                                        updateLoanLogHistoryByCollectionDate.IsAction = true;
                                                        db.SubmitChanges();
                                                    }
                                                    else
                                                    {
                                                        if (loanLogHistories.FirstOrDefault().CurrentBalanceAmount - loanLogHistory.PaidAmount == 0)
                                                        {
                                                            var updateLoanApplicationsForFullyPaid = loanApplications.FirstOrDefault();
                                                            updateLoanApplicationsForFullyPaid.IsFullyPaid = true;
                                                            db.SubmitChanges();
                                                        }
                                                        else
                                                        {
                                                            Data.trnLoanLogHistory newLoanLogHistory = new Data.trnLoanLogHistory();
                                                            newLoanLogHistory.LoanId = Convert.ToInt32(loanId);
                                                            newLoanLogHistory.DayNumber = loanLogHistories.FirstOrDefault().DayNumber + 1;
                                                            newLoanLogHistory.CollectionDate = Convert.ToDateTime(loanLogHistories.FirstOrDefault().CollectionDate).Date.AddDays(1);
                                                            newLoanLogHistory.NetAmount = loanLogHistories.FirstOrDefault().NetAmount;
                                                            newLoanLogHistory.CollectibleAmount = 0;
                                                            newLoanLogHistory.PenaltyAmount = 0;
                                                            newLoanLogHistory.PaidAmount = 0;
                                                            newLoanLogHistory.PreviousBalanceAmount = (loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount) - loanLogHistory.PaidAmount;
                                                            newLoanLogHistory.CurrentBalanceAmount = (loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().PreviousBalanceAmount) - loanLogHistory.PaidAmount;
                                                            newLoanLogHistory.IsCleared = false;
                                                            newLoanLogHistory.IsPenalty = false;
                                                            newLoanLogHistory.IsAction = true;
                                                            newLoanLogHistory.IsOverdue = false;
                                                            newLoanLogHistory.IsDueDate = true;
                                                            db.trnLoanLogHistories.InsertOnSubmit(newLoanLogHistory);
                                                            db.SubmitChanges();
                                                        }
                                                    }
                                                }

                                                return Request.CreateResponse(HttpStatusCode.OK);
                                            }
                                            else
                                            {
                                                return Request.CreateResponse(HttpStatusCode.BadRequest, "The amount to be paid must not be greater than the current amount which is to be collected (Paid).");
                                            }
                                        }
                                        else
                                        {
                                            return Request.CreateResponse(HttpStatusCode.BadRequest, "The actions can be applied when the collection date matches the date today.");
                                        }
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot apply actions by this time.");
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The collection was already cleared. Click the Option button for update changes.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "No current balance to be cleared.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server to apply some actions.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Please lock the loan application first before procceding the collection process.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server to apply some actions.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something went wrong from the server. Please contact the administrator.");
            }
        }
    }
}
