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
                                       IsFullyPaid = d.IsFullyPaid,
                                       IsAction = d.IsAction
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
                                       IsFullyPaid = d.IsFullyPaid,
                                       IsAction = d.IsAction
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
                                                                         where d.LoanId == Convert.ToInt32(loanId)
                                                                         && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate
                                                                         select d;

                                            if (collectionLogHistories.Any())
                                            {
                                                var updateCollectionLogHistory = collectionLogHistories.FirstOrDefault();
                                                updateCollectionLogHistory.LoanId = Convert.ToInt32(loanId);
                                                updateCollectionLogHistory.CollectionDate = loanLogHistories.FirstOrDefault().CollectionDate;
                                                updateCollectionLogHistory.CollectibleAmount = loanLogHistories.FirstOrDefault().CollectibleAmount;
                                                updateCollectionLogHistory.PaidAmount = loanLogHistories.FirstOrDefault().CurrentBalanceAmount;
                                                updateCollectionLogHistory.IsCleared = true;
                                                updateCollectionLogHistory.IsPenalty = loanLogHistories.FirstOrDefault().IsPenalty;
                                                updateCollectionLogHistory.IsOverdue = loanLogHistories.FirstOrDefault().IsOverdue;
                                                updateCollectionLogHistory.IsFullyPaid = loanLogHistories.FirstOrDefault().IsFullyPaid;
                                                updateCollectionLogHistory.CollectorId = loanApplications.FirstOrDefault().CollectorId;
                                                updateCollectionLogHistory.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
                                                db.SubmitChanges();
                                            }
                                            else
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
                                                    var loanLogHistoriesForFullPayments = from d in db.trnLoanLogHistories
                                                                                          where d.LoanId == Convert.ToInt32(loanId)
                                                                                          && d.CollectionDate >= loanApplications.FirstOrDefault().LoanDate
                                                                                          && d.CollectionDate <= loanApplications.FirstOrDefault().MaturityDate
                                                                                          select new Models.TrnLoanLogHistory
                                                                                          {
                                                                                              Id = d.Id
                                                                                          };

                                                    Debug.WriteLine("0");

                                                    if (loanLogHistoriesForFullPayments.Any())
                                                    {
                                                        Debug.WriteLine("1");

                                                        foreach (var loanLogHistoriesForFullPayment in loanLogHistoriesForFullPayments)
                                                        {
                                                            Debug.WriteLine("2");
                                                            var eachLoanLogHistoriesForFullPayment = from d in db.trnLoanLogHistories where d.Id == loanLogHistoriesForFullPayment.Id select d;

                                                            if (eachLoanLogHistoriesForFullPayment.Any())
                                                            {
                                                                Debug.WriteLine("3");
                                                                var updateFullPaymentLoanLogHistoriesForFullPayment = eachLoanLogHistoriesForFullPayment.FirstOrDefault();
                                                                updateFullPaymentLoanLogHistoriesForFullPayment.IsFullyPaid = true;
                                                                db.SubmitChanges();
                                                            }
                                                        }
                                                    }
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
                                                    var loanLogHistoriesForFullPayments = from d in db.trnLoanLogHistories
                                                                                          where d.LoanId == Convert.ToInt32(loanId)
                                                                                          && d.CollectionDate >= loanApplications.FirstOrDefault().LoanDate
                                                                                          && d.CollectionDate <= loanApplications.FirstOrDefault().MaturityDate
                                                                                          select new Models.TrnLoanLogHistory
                                                                                          {
                                                                                              Id = d.Id
                                                                                          };

                                                    Debug.WriteLine("0");

                                                    if (loanLogHistoriesForFullPayments.Any())
                                                    {
                                                        Debug.WriteLine("1");

                                                        foreach (var loanLogHistoriesForFullPayment in loanLogHistoriesForFullPayments)
                                                        {
                                                            Debug.WriteLine("2");
                                                            var eachLoanLogHistoriesForFullPayment = from d in db.trnLoanLogHistories where d.Id == loanLogHistoriesForFullPayment.Id select d;

                                                            if (eachLoanLogHistoriesForFullPayment.Any())
                                                            {
                                                                Debug.WriteLine("3");
                                                                var updateFullPaymentLoanLogHistoriesForFullPayment = eachLoanLogHistoriesForFullPayment.FirstOrDefault();
                                                                updateFullPaymentLoanLogHistoriesForFullPayment.IsFullyPaid = true;
                                                                db.SubmitChanges();
                                                            }
                                                        }
                                                    }
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
                                                                     where d.LoanId == Convert.ToInt32(loanId)
                                                                     && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate
                                                                     select d;

                                        if (collectionLogHistories.Any())
                                        {
                                            var updateCollectionLogHistory = collectionLogHistories.FirstOrDefault();
                                            updateCollectionLogHistory.LoanId = Convert.ToInt32(loanId);
                                            updateCollectionLogHistory.CollectionDate = loanLogHistories.FirstOrDefault().CollectionDate;
                                            updateCollectionLogHistory.CollectibleAmount = loanLogHistories.FirstOrDefault().CollectibleAmount;
                                            updateCollectionLogHistory.PaidAmount = loanLogHistories.FirstOrDefault().CurrentBalanceAmount;
                                            updateCollectionLogHistory.IsCleared = true;
                                            updateCollectionLogHistory.IsPenalty = loanLogHistories.FirstOrDefault().IsPenalty;
                                            updateCollectionLogHistory.IsOverdue = loanLogHistories.FirstOrDefault().IsOverdue;
                                            updateCollectionLogHistory.IsFullyPaid = loanLogHistories.FirstOrDefault().IsFullyPaid;
                                            updateCollectionLogHistory.CollectorId = loanApplications.FirstOrDefault().CollectorId;
                                            updateCollectionLogHistory.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
                                            db.SubmitChanges();
                                        }
                                        else
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
                                                var loanLogHistoriesForFullPayments = from d in db.trnLoanLogHistories
                                                                                      where d.Id == Convert.ToInt32(id)
                                                                                      && d.CollectionDate >= loanApplications.FirstOrDefault().LoanDate
                                                                                      && d.CollectionDate <= loanApplications.FirstOrDefault().MaturityDate
                                                                                      select new Models.TrnLoanLogHistory
                                                                                      {
                                                                                          Id = d.Id
                                                                                      };

                                                if (loanLogHistoriesForFullPayments.Any())
                                                {
                                                    foreach (var loanLogHistoriesForFullPayment in loanLogHistoriesForFullPayments)
                                                    {
                                                        var eachLoanLogHistoriesForFullPayment = from d in db.trnLoanLogHistories where d.Id == loanLogHistoriesForFullPayment.Id select d;

                                                        if (eachLoanLogHistoriesForFullPayment.Any())
                                                        {
                                                            var updateFullPaymentLoanLogHistoriesForFullPayment = eachLoanLogHistoriesForFullPayment.FirstOrDefault();
                                                            updateFullPaymentLoanLogHistoriesForFullPayment.IsFullyPaid = false;
                                                            db.SubmitChanges();
                                                        }
                                                    }
                                                }

                                                Data.trnLoanLogHistory newLoanLogHistory = new Data.trnLoanLogHistory();
                                                newLoanLogHistory.LoanId = Convert.ToInt32(loanId);
                                                newLoanLogHistory.DayNumber = loanLogHistories.FirstOrDefault().DayNumber + 1;
                                                newLoanLogHistory.CollectionDate = loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(1);
                                                newLoanLogHistory.NetAmount = loanLogHistories.FirstOrDefault().NetAmount;
                                                newLoanLogHistory.CollectibleAmount = loanLogHistories.FirstOrDefault().CollectibleAmount;
                                                newLoanLogHistory.PenaltyAmount = 0;
                                                newLoanLogHistory.PaidAmount = 0;
                                                newLoanLogHistory.PreviousBalanceAmount = loanLogHistories.FirstOrDefault().CurrentBalanceAmount;
                                                newLoanLogHistory.CurrentBalanceAmount = loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().CurrentBalanceAmount;
                                                newLoanLogHistory.IsCleared = false;
                                                newLoanLogHistory.IsPenalty = false;
                                                newLoanLogHistory.IsOverdue = false;
                                                newLoanLogHistory.IsFullyPaid = false;
                                                newLoanLogHistory.IsAction = true;
                                                db.trnLoanLogHistories.InsertOnSubmit(newLoanLogHistory);
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
                                                var loanLogHistoriesForFullPayments = from d in db.trnLoanLogHistories
                                                                                      where d.Id == Convert.ToInt32(id)
                                                                                      && d.CollectionDate >= loanApplications.FirstOrDefault().LoanDate
                                                                                      && d.CollectionDate <= loanApplications.FirstOrDefault().MaturityDate
                                                                                      select new Models.TrnLoanLogHistory
                                                                                      {
                                                                                          Id = d.Id
                                                                                      };

                                                if (loanLogHistoriesForFullPayments.Any())
                                                {
                                                    foreach (var loanLogHistoriesForFullPayment in loanLogHistoriesForFullPayments)
                                                    {
                                                        var eachLoanLogHistoriesForFullPayment = from d in db.trnLoanLogHistories where d.Id == loanLogHistoriesForFullPayment.Id select d;

                                                        if (eachLoanLogHistoriesForFullPayment.Any())
                                                        {
                                                            var updateFullPaymentLoanLogHistoriesForFullPayment = eachLoanLogHistoriesForFullPayment.FirstOrDefault();
                                                            updateFullPaymentLoanLogHistoriesForFullPayment.IsFullyPaid = false;
                                                            db.SubmitChanges();
                                                        }
                                                    }
                                                }

                                                Data.trnLoanLogHistory newLoanLogHistory = new Data.trnLoanLogHistory();
                                                newLoanLogHistory.LoanId = Convert.ToInt32(loanId);
                                                newLoanLogHistory.DayNumber = loanLogHistories.FirstOrDefault().DayNumber + 1;
                                                newLoanLogHistory.CollectionDate = loanLogHistories.FirstOrDefault().CollectionDate.Date.AddDays(1);
                                                newLoanLogHistory.NetAmount = loanLogHistories.FirstOrDefault().NetAmount;
                                                newLoanLogHistory.CollectibleAmount = loanLogHistories.FirstOrDefault().CollectibleAmount;
                                                newLoanLogHistory.PenaltyAmount = 0;
                                                newLoanLogHistory.PaidAmount = 0;
                                                newLoanLogHistory.PreviousBalanceAmount = loanLogHistories.FirstOrDefault().CurrentBalanceAmount;
                                                newLoanLogHistory.CurrentBalanceAmount = loanLogHistories.FirstOrDefault().CollectibleAmount + loanLogHistories.FirstOrDefault().CurrentBalanceAmount;
                                                newLoanLogHistory.IsCleared = false;
                                                newLoanLogHistory.IsPenalty = false;
                                                newLoanLogHistory.IsOverdue = false;
                                                newLoanLogHistory.IsFullyPaid = false;
                                                newLoanLogHistory.IsAction = true;
                                                db.trnLoanLogHistories.InsertOnSubmit(newLoanLogHistory);
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
                                                                 where d.LoanId == Convert.ToInt32(loanId)
                                                                 && d.CollectionDate == loanLogHistories.FirstOrDefault().CollectionDate
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
                                        updateLoanLogHistory.IsOverdue = false;
                                        updateLoanLogHistory.IsFullyPaid = false;
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
                                        updateLoanLogHistory.IsOverdue = false;
                                        updateLoanLogHistory.IsFullyPaid = false;
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
    }
}
