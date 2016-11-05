﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiCollectionController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // collection list by applicantId and by loanId for advance payment
        [Authorize]
        [HttpGet]
        [Route("api/collection/list/advancePayment/byApplicantId/byLoanId/{applicantId}/{loanId}")]
        public List<Models.TrnCollection> listCollectionForAdvancePayment(String applicantId, String loanId)
        {
            var collections = from d in db.trnCollections
                              where d.trnLoanApplication.ApplicantId == Convert.ToInt32(applicantId)
                              && d.LoanId == Convert.ToInt32(loanId)
                              && d.IsProcessed == false
                              select new Models.TrnCollection
                              {
                                  Id = d.Id,
                                  LoanId = d.LoanId,
                                  LoanNumber = d.trnLoanApplication.LoanNumber,
                                  ApplicantId = d.trnLoanApplication.ApplicantId,
                                  Applicant = d.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + d.trnLoanApplication.mstApplicant.ApplicantMiddleName,
                                  Area = d.trnLoanApplication.mstApplicant.mstArea.Area,
                                  IsFullyPaid = d.trnLoanApplication.IsFullyPaid,
                                  AccountId = d.AccountId,
                                  Account = d.mstAccount.Account,
                                  CollectionDate = d.CollectionDate.ToShortDateString(),
                                  NetAmount = d.NetAmount,
                                  CollectibleAmount = d.CollectibleAmount,
                                  PenaltyAmount = d.PenaltyAmount,
                                  PaidAmount = d.PaidAmount,
                                  PreviousBalanceAmount = d.PreviousBalanceAmount,
                                  CurrentBalanceAmount = d.CurrentBalanceAmount,
                                  IsCleared = d.IsCleared,
                                  IsAbsent = d.IsAbsent,
                                  IsPartiallyPaid = d.IsPartiallyPaid,
                                  IsAdvancedPaid = d.IsAdvancedPaid,
                                  IsDueDate = d.IsDueDate,
                                  IsOverdue = d.IsOverdue,
                                  IsExtendCollection = d.IsExtendCollection,
                                  IsCurrentCollection = d.IsCurrentCollection,
                                  IsProcessed = d.IsProcessed,
                                  IsAction = d.IsAction,
                                  AssignedCollectorId = d.trnLoanApplication.AssignedCollectorId,
                                  AssignedCollector = d.trnLoanApplication.mstCollector.Collector,
                                  AssignedCollectorArea = d.trnLoanApplication.mstCollector.Collector + " (" + d.trnLoanApplication.mstCollector.mstArea.Area + ")",
                                  CurrentCollectorId = d.trnLoanApplication.CurrentCollectorId,
                                  CurrentCollector = d.trnLoanApplication.mstCollector1.Collector,
                                  CurrentCollectorArea = d.trnLoanApplication.mstCollector1.Collector + " (" + d.trnLoanApplication.mstCollector1.mstArea.Area + ")",
                                  Status = getStatus(d.IsCleared, d.IsAbsent, d.IsOverdue, d.IsExtendCollection, d.IsPartiallyPaid, d.IsAdvancedPaid)
                              };

            return collections.ToList();
        }

        // get status
        public String getStatus(Boolean IsCleared, Boolean IsAbsent, Boolean IsOverdue, Boolean IsExtendCollection, Boolean IsPartiallyPaid, Boolean IsAdvancedPaid)
        {
            String status = "";

            if (IsCleared)
            {
                if (IsAdvancedPaid)
                {
                    status = "Advanced";
                }
                else
                {
                    if (IsOverdue)
                    {
                        status = "Overdue";
                    }
                    else
                    {
                        if (IsExtendCollection)
                        {
                            status = "Extended";
                        }
                        else
                        {
                            status = "Paid";
                        }
                    }
                }
            }
            else
            {
                if (IsAdvancedPaid)
                {
                    status = "Advanced";
                }
                else
                {
                    if (IsAbsent)
                    {
                        if (IsOverdue)
                        {
                            status = "Overdue";
                        }
                        else
                        {
                            if (IsExtendCollection)
                            {
                                status = "Extended";
                            }
                            else
                            {
                                status = "Absent";
                            }
                        }
                    }
                    else
                    {
                        if (IsPartiallyPaid)
                        {
                            if (IsOverdue)
                            {
                                status = "Overdue";
                            }
                            else
                            {
                                if (IsExtendCollection)
                                {
                                    status = "Extended";
                                }
                                else
                                {
                                    status = "Partially Paid";
                                }
                            }
                        }
                        else
                        {
                            status = "--";
                        }
                    }
                }
            }

            return status;
        }

        // get current collection by applicantId and by loanId
        [Authorize]
        [HttpGet]
        [Route("api/collection/get/currentCollection/byApplicantId/byLoanId/{applicantId}/{loanId}")]
        public Models.TrnCollection getCurrentCollectionForPartialPayment(String applicantId, String loanId)
        {
            var collections = from d in db.trnCollections
                              where d.trnLoanApplication.ApplicantId == Convert.ToInt32(applicantId)
                              && d.LoanId == Convert.ToInt32(loanId)
                              && d.IsCurrentCollection == true
                              select new Models.TrnCollection
                              {
                                  Id = d.Id,
                                  LoanId = d.LoanId,
                                  LoanNumber = d.trnLoanApplication.LoanNumber,
                                  ApplicantId = d.trnLoanApplication.ApplicantId,
                                  Applicant = d.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + d.trnLoanApplication.mstApplicant.ApplicantMiddleName,
                                  Area = d.trnLoanApplication.mstApplicant.mstArea.Area,
                                  IsFullyPaid = d.trnLoanApplication.IsFullyPaid,
                                  AccountId = d.AccountId,
                                  Account = d.mstAccount.Account,
                                  CollectionDate = d.CollectionDate.ToShortDateString(),
                                  NetAmount = d.NetAmount,
                                  CollectibleAmount = d.CollectibleAmount,
                                  PenaltyAmount = d.PenaltyAmount,
                                  PaidAmount = d.PaidAmount,
                                  PreviousBalanceAmount = d.PreviousBalanceAmount,
                                  CurrentBalanceAmount = d.CurrentBalanceAmount,
                                  IsCleared = d.IsCleared,
                                  IsAbsent = d.IsAbsent,
                                  IsPartiallyPaid = d.IsPartiallyPaid,
                                  IsAdvancedPaid = d.IsAdvancedPaid,
                                  IsDueDate = d.IsDueDate,
                                  IsOverdue = d.IsOverdue,
                                  IsExtendCollection = d.IsExtendCollection,
                                  IsCurrentCollection = d.IsCurrentCollection,
                                  IsProcessed = d.IsProcessed,
                                  IsAction = d.IsAction,
                                  AssignedCollectorId = d.trnLoanApplication.AssignedCollectorId,
                                  AssignedCollector = d.trnLoanApplication.mstCollector.Collector,
                                  AssignedCollectorArea = d.trnLoanApplication.mstCollector.Collector + " (" + d.trnLoanApplication.mstCollector.mstArea.Area + ")",
                                  CurrentCollectorId = d.trnLoanApplication.CurrentCollectorId,
                                  CurrentCollector = d.trnLoanApplication.mstCollector1.Collector,
                                  CurrentCollectorArea = d.trnLoanApplication.mstCollector1.Collector + " (" + d.trnLoanApplication.mstCollector1.mstArea.Area + ")",
                                  Status = getStatus(d.IsCleared, d.IsAbsent, d.IsOverdue, d.IsExtendCollection, d.IsPartiallyPaid, d.IsAdvancedPaid)
                              };

            return (Models.TrnCollection)collections.FirstOrDefault();
        }

        // collection list by applicantId and by loanId
        [Authorize]
        [HttpGet]
        [Route("api/collection/list/byApplicantId/byLoanId/{applicantId}/{loanId}")]
        public List<Models.TrnCollection> listCollectionByApplicantIdByLoanId(String applicantId, String loanId)
        {
            var collections = from d in db.trnCollections
                              where d.trnLoanApplication.ApplicantId == Convert.ToInt32(applicantId)
                              && d.LoanId == Convert.ToInt32(loanId)
                              select new Models.TrnCollection
                              {
                                  Id = d.Id,
                                  LoanId = d.LoanId,
                                  LoanNumber = d.trnLoanApplication.LoanNumber,
                                  ApplicantId = d.trnLoanApplication.ApplicantId,
                                  Applicant = d.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + d.trnLoanApplication.mstApplicant.ApplicantMiddleName,
                                  Area = d.trnLoanApplication.mstApplicant.mstArea.Area,
                                  IsFullyPaid = d.trnLoanApplication.IsFullyPaid,
                                  AccountId = d.AccountId,
                                  Account = d.mstAccount.Account,
                                  CollectionDate = d.CollectionDate.ToShortDateString(),
                                  NetAmount = d.NetAmount,
                                  CollectibleAmount = d.CollectibleAmount,
                                  PenaltyAmount = d.PenaltyAmount,
                                  PaidAmount = d.PaidAmount,
                                  PreviousBalanceAmount = d.PreviousBalanceAmount,
                                  CurrentBalanceAmount = d.CurrentBalanceAmount,
                                  IsCleared = d.IsCleared,
                                  IsAbsent = d.IsAbsent,
                                  IsPartiallyPaid = d.IsPartiallyPaid,
                                  IsAdvancedPaid = d.IsAdvancedPaid,
                                  IsDueDate = d.IsDueDate,
                                  IsOverdue = d.IsOverdue,
                                  IsExtendCollection = d.IsExtendCollection,
                                  IsCurrentCollection = d.IsCurrentCollection,
                                  IsProcessed = d.IsProcessed,
                                  IsAction = d.IsAction,
                                  AssignedCollectorId = d.trnLoanApplication.AssignedCollectorId,
                                  AssignedCollector = d.trnLoanApplication.mstCollector.Collector,
                                  AssignedCollectorArea = d.trnLoanApplication.mstCollector.Collector + " (" + d.trnLoanApplication.mstCollector.mstArea.Area + ")",
                                  CurrentCollectorId = d.trnLoanApplication.CurrentCollectorId,
                                  CurrentCollector = d.trnLoanApplication.mstCollector1.Collector,
                                  CurrentCollectorArea = d.trnLoanApplication.mstCollector1.Collector + " (" + d.trnLoanApplication.mstCollector1.mstArea.Area + ")",
                                  Status = getStatus(d.IsCleared, d.IsAbsent, d.IsOverdue, d.IsExtendCollection, d.IsPartiallyPaid, d.IsAdvancedPaid)
                              };

            return collections.ToList();
        }

        // collection list by collectionDate and by areaId
        [Authorize]
        [HttpGet]
        [Route("api/collection/list/byCollectionDate/byAreaId/{collectionDate}/{areaId}")]
        public List<Models.TrnCollection> listCollectionByCollectionDateByAreaId(String collectionDate, String areaId)
        {
            var collections = from d in db.trnCollections
                              where d.CollectionDate == Convert.ToDateTime(collectionDate)
                              && d.trnLoanApplication.mstApplicant.AreaId == Convert.ToInt32(areaId)
                              select new Models.TrnCollection
                                   {
                                       Id = d.Id,
                                       LoanId = d.LoanId,
                                       LoanNumber = d.trnLoanApplication.LoanNumber,
                                       ApplicantId = d.trnLoanApplication.ApplicantId,
                                       Applicant = d.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + d.trnLoanApplication.mstApplicant.ApplicantMiddleName,
                                       Area = d.trnLoanApplication.mstApplicant.mstArea.Area,
                                       IsFullyPaid = d.trnLoanApplication.IsFullyPaid,
                                       AccountId = d.AccountId,
                                       Account = d.mstAccount.Account,
                                       CollectionDate = d.CollectionDate.ToShortDateString(),
                                       NetAmount = d.NetAmount,
                                       CollectibleAmount = d.CollectibleAmount,
                                       PenaltyAmount = d.PenaltyAmount,
                                       PaidAmount = d.PaidAmount,
                                       PreviousBalanceAmount = d.PreviousBalanceAmount,
                                       CurrentBalanceAmount = d.CurrentBalanceAmount,
                                       IsCleared = d.IsCleared,
                                       IsAbsent = d.IsAbsent,
                                       IsPartiallyPaid = d.IsPartiallyPaid,
                                       IsAdvancedPaid = d.IsAdvancedPaid,
                                       IsDueDate = d.IsDueDate,
                                       IsOverdue = d.IsOverdue,
                                       IsExtendCollection = d.IsExtendCollection,
                                       IsCurrentCollection = d.IsCurrentCollection,
                                       IsProcessed = d.IsProcessed,
                                       IsAction = d.IsAction,
                                       AssignedCollectorId = d.trnLoanApplication.AssignedCollectorId,
                                       AssignedCollector = d.trnLoanApplication.mstCollector.Collector,
                                       AssignedCollectorArea = d.trnLoanApplication.mstCollector.Collector + " (" + d.trnLoanApplication.mstCollector.mstArea.Area + ")",
                                       CurrentCollectorId = d.trnLoanApplication.CurrentCollectorId,
                                       CurrentCollector = d.trnLoanApplication.mstCollector1.Collector,
                                       CurrentCollectorArea = d.trnLoanApplication.mstCollector1.Collector + " (" + d.trnLoanApplication.mstCollector1.mstArea.Area + ")",
                                       Status = getStatus(d.IsCleared, d.IsAbsent, d.IsOverdue, d.IsExtendCollection, d.IsPartiallyPaid, d.IsAdvancedPaid)
                                   };

            return collections.ToList();
        }

        // clear collection
        [Authorize]
        [HttpPut]
        [Route("api/collection/cleared/update/byId/byLoanId/{id}/{loanId}")]
        public HttpResponseMessage clearedCollection(String id, String loanId)
        {
            try
            {
                var loanApplication = from d in db.trnLoanApplications where d.Id == Convert.ToInt32(loanId) select d;
                if (loanApplication.Any())
                {
                    if (loanApplication.FirstOrDefault().IsLocked)
                    {
                        var collection = from d in db.trnCollections where d.Id == Convert.ToInt32(id) select d;
                        if (collection.Any())
                        {
                            if (collection.FirstOrDefault().CurrentBalanceAmount > 0)
                            {
                                if (!collection.FirstOrDefault().IsCleared)
                                {
                                    if (collection.FirstOrDefault().IsAction)
                                    {
                                        var updateCollection = collection.FirstOrDefault();
                                        updateCollection.PaidAmount = collection.FirstOrDefault().CollectibleAmount + collection.FirstOrDefault().PreviousBalanceAmount;
                                        updateCollection.CurrentBalanceAmount = 0;
                                        updateCollection.PenaltyAmount = 0;
                                        updateCollection.IsCleared = true;
                                        updateCollection.IsAbsent = false;
                                        updateCollection.IsProcessed = true;
                                        updateCollection.IsCurrentCollection = false;
                                        updateCollection.IsPartiallyPaid = false;
                                        updateCollection.IsAdvancedPaid = false;
                                        db.SubmitChanges();

                                        var collectionPrevoiusDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == collection.FirstOrDefault().CollectionDate.Date.AddDays(-1) select d;
                                        if (collectionPrevoiusDate.Any())
                                        {
                                            var updateCollectionPrevoiusDate = collectionPrevoiusDate.FirstOrDefault();
                                            updateCollectionPrevoiusDate.IsAction = false;
                                            updateCollectionPrevoiusDate.IsCurrentCollection = false;
                                            db.SubmitChanges();

                                            var collectionNextDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == collection.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                            if (collectionNextDate.Any())
                                            {
                                                var updateCollectionNextDate = collectionNextDate.FirstOrDefault();
                                                updateCollectionNextDate.PreviousBalanceAmount = 0;
                                                updateCollectionNextDate.CurrentBalanceAmount = collectionNextDate.FirstOrDefault().CollectibleAmount;
                                                updateCollectionNextDate.IsAction = true;
                                                updateCollectionNextDate.IsCurrentCollection = true;
                                                db.SubmitChanges();
                                            }
                                            else
                                            {
                                                var updateLoanApplicationFullPayment = loanApplication.FirstOrDefault();
                                                updateLoanApplicationFullPayment.IsFullyPaid = true;
                                                db.SubmitChanges();
                                            }
                                        }
                                        else
                                        {
                                            var collectionNextDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == collection.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                            if (collectionNextDate.Any())
                                            {
                                                var updateCollectionNextDate = collectionNextDate.FirstOrDefault();
                                                updateCollectionNextDate.PreviousBalanceAmount = 0;
                                                updateCollectionNextDate.CurrentBalanceAmount = collectionNextDate.FirstOrDefault().CollectibleAmount;
                                                updateCollectionNextDate.IsAction = true;
                                                updateCollectionNextDate.IsCurrentCollection = true;
                                                db.SubmitChanges();
                                            }
                                            else
                                            {
                                                var updateLoanApplicationFullPayment = loanApplication.FirstOrDefault();
                                                updateLoanApplicationFullPayment.IsFullyPaid = true;
                                                db.SubmitChanges();
                                            }
                                        }

                                        return Request.CreateResponse(HttpStatusCode.OK);
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot apply actions by this time.");
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Collection was already cleared.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "No current balance to be cleared.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Please lock the loan application first before procceding the collection process.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something went wrong from the server.");
            }
        }

        // absent collection
        [Authorize]
        [HttpPut]
        [Route("api/collection/absent/update/byId/byLoanId/{id}/{loanId}")]
        public HttpResponseMessage absentCollection(String id, String loanId)
        {
            try
            {
                var loanApplication = from d in db.trnLoanApplications where d.Id == Convert.ToInt32(loanId) select d;
                if (loanApplication.Any())
                {
                    if (loanApplication.FirstOrDefault().IsLocked)
                    {
                        var collection = from d in db.trnCollections where d.Id == Convert.ToInt32(id) select d;
                        if (collection.Any())
                        {
                            if (!collection.FirstOrDefault().IsAbsent)
                            {
                                if (collection.FirstOrDefault().IsAction)
                                {
                                    if (!collection.FirstOrDefault().IsDueDate)
                                    {
                                        Decimal penaltyAmount = getPenaltyAmount(collection.FirstOrDefault().LoanId, collection.FirstOrDefault().CollectionDate.ToShortDateString());

                                        var updateCollection = collection.FirstOrDefault();
                                        updateCollection.PaidAmount = 0;
                                        updateCollection.CurrentBalanceAmount = collection.FirstOrDefault().CollectibleAmount + penaltyAmount + collection.FirstOrDefault().PreviousBalanceAmount;
                                        updateCollection.PenaltyAmount = penaltyAmount;
                                        updateCollection.IsCleared = false;
                                        updateCollection.IsAbsent = true;
                                        updateCollection.IsProcessed = true;
                                        updateCollection.IsCurrentCollection = false;
                                        updateCollection.IsPartiallyPaid = false;
                                        updateCollection.IsAdvancedPaid = false;
                                        db.SubmitChanges();

                                        var collectionPrevoiusDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == collection.FirstOrDefault().CollectionDate.Date.AddDays(-1) select d;
                                        if (collectionPrevoiusDate.Any())
                                        {
                                            var updateCollectionPrevoiusDate = collectionPrevoiusDate.FirstOrDefault();
                                            updateCollectionPrevoiusDate.IsAction = false;
                                            updateCollectionPrevoiusDate.IsCurrentCollection = false;
                                            db.SubmitChanges();

                                            var collectionNextDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == collection.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                            if (collectionNextDate.Any())
                                            {
                                                var updateCollectionNextDate = collectionNextDate.FirstOrDefault();
                                                updateCollectionNextDate.PreviousBalanceAmount = collection.FirstOrDefault().CollectibleAmount + penaltyAmount + collection.FirstOrDefault().PreviousBalanceAmount;
                                                updateCollectionNextDate.CurrentBalanceAmount = collectionNextDate.FirstOrDefault().CollectibleAmount + (collection.FirstOrDefault().CollectibleAmount + penaltyAmount + collection.FirstOrDefault().PreviousBalanceAmount);
                                                updateCollectionNextDate.IsAction = true;
                                                updateCollectionNextDate.IsCurrentCollection = true;
                                                db.SubmitChanges();
                                            }
                                            else
                                            {
                                                var updateLoanApplicationFullPayment = loanApplication.FirstOrDefault();
                                                updateLoanApplicationFullPayment.IsFullyPaid = true;
                                                db.SubmitChanges();
                                            }
                                        }
                                        else
                                        {
                                            var collectionNextDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == collection.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                            if (collectionNextDate.Any())
                                            {
                                                var updateCollectionNextDate = collectionNextDate.FirstOrDefault();
                                                updateCollectionNextDate.PreviousBalanceAmount = collection.FirstOrDefault().CollectibleAmount + penaltyAmount + collection.FirstOrDefault().PreviousBalanceAmount;
                                                updateCollectionNextDate.CurrentBalanceAmount = collectionNextDate.FirstOrDefault().CollectibleAmount + (collection.FirstOrDefault().CollectibleAmount + penaltyAmount + collection.FirstOrDefault().PreviousBalanceAmount);
                                                updateCollectionNextDate.IsAction = true;
                                                updateCollectionNextDate.IsCurrentCollection = true;
                                                db.SubmitChanges();
                                            }
                                            else
                                            {
                                                var updateLoanApplicationFullPayment = loanApplication.FirstOrDefault();
                                                updateLoanApplicationFullPayment.IsFullyPaid = true;
                                                db.SubmitChanges();
                                            }
                                        }

                                        return Request.CreateResponse(HttpStatusCode.OK);
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest, "The Collection is in due date. Extend or Overdue");
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot apply actions by this time.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Collection was already absent.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Please lock the loan application first before procceding the collection process.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something went wrong from the server.");
            }
        }

        // undo changes collection
        [Authorize]
        [HttpPut]
        [Route("api/collection/undoChanges/update/byId/byLoanId/{id}/{loanId}")]
        public HttpResponseMessage undoChangesCollection(String id, String loanId)
        {
            try
            {
                var loanApplication = from d in db.trnLoanApplications where d.Id == Convert.ToInt32(loanId) select d;
                if (loanApplication.Any())
                {
                    if (loanApplication.FirstOrDefault().IsLocked)
                    {
                        var collection = from d in db.trnCollections where d.Id == Convert.ToInt32(id) select d;
                        if (collection.Any())
                        {
                            if (collection.FirstOrDefault().IsAction)
                            {
                                var collectionPrevoiusDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == collection.FirstOrDefault().CollectionDate.Date.AddDays(-1) select d;
                                if (collectionPrevoiusDate.Any())
                                {
                                    var updateCollection = collection.FirstOrDefault();
                                    updateCollection.PaidAmount = 0;
                                    updateCollection.PreviousBalanceAmount = collectionPrevoiusDate.FirstOrDefault().CurrentBalanceAmount;
                                    updateCollection.CurrentBalanceAmount = collection.FirstOrDefault().CollectibleAmount + collectionPrevoiusDate.FirstOrDefault().CurrentBalanceAmount;
                                    updateCollection.PenaltyAmount = 0;
                                    updateCollection.IsCleared = false;
                                    updateCollection.IsAbsent = false;
                                    updateCollection.IsCurrentCollection = true;
                                    updateCollection.IsProcessed = false;
                                    updateCollection.IsAction = true;
                                    updateCollection.IsPartiallyPaid = false;
                                    updateCollection.IsAdvancedPaid = false;
                                    db.SubmitChanges();

                                    var updateCollectionPrevoiusDate = collectionPrevoiusDate.FirstOrDefault();
                                    updateCollectionPrevoiusDate.IsAction = true;
                                    updateCollectionPrevoiusDate.IsCurrentCollection = false;
                                    db.SubmitChanges();

                                    var collectionNextDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == collection.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                    if (collectionNextDate.Any())
                                    {
                                        var updateCollectionNextDate = collectionNextDate.FirstOrDefault();
                                        updateCollectionNextDate.PreviousBalanceAmount = 0;
                                        updateCollectionNextDate.CurrentBalanceAmount = collectionNextDate.FirstOrDefault().CollectibleAmount;
                                        updateCollectionNextDate.IsAction = false;
                                        updateCollectionNextDate.IsCurrentCollection = false;
                                        db.SubmitChanges();
                                    }
                                    else
                                    {
                                        var updateLoanApplicationFullPayment = loanApplication.FirstOrDefault();
                                        updateLoanApplicationFullPayment.IsFullyPaid = false;
                                        db.SubmitChanges();
                                    }
                                }
                                else
                                {
                                    var updateCollection = collection.FirstOrDefault();
                                    updateCollection.PaidAmount = 0;
                                    updateCollection.PreviousBalanceAmount = 0;
                                    updateCollection.CurrentBalanceAmount = collection.FirstOrDefault().CollectibleAmount;
                                    updateCollection.PenaltyAmount = 0;
                                    updateCollection.IsCleared = false;
                                    updateCollection.IsAbsent = false;
                                    updateCollection.IsCurrentCollection = true;
                                    updateCollection.IsProcessed = false;
                                    updateCollection.IsAction = true;
                                    updateCollection.IsPartiallyPaid = false;
                                    updateCollection.IsAdvancedPaid = false;
                                    db.SubmitChanges();

                                    var collectionNextDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == collection.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                    if (collectionNextDate.Any())
                                    {
                                        var updateCollectionNextDate = collectionNextDate.FirstOrDefault();
                                        updateCollectionNextDate.PreviousBalanceAmount = 0;
                                        updateCollectionNextDate.CurrentBalanceAmount = collectionNextDate.FirstOrDefault().CollectibleAmount;
                                        updateCollectionNextDate.IsAction = false;
                                        updateCollectionNextDate.IsCurrentCollection = false;
                                        db.SubmitChanges();
                                    }
                                    else
                                    {
                                        var updateLoanApplicationFullPayment = loanApplication.FirstOrDefault();
                                        updateLoanApplicationFullPayment.IsFullyPaid = false;
                                        db.SubmitChanges();
                                    }
                                }

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot apply actions by this time.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Please lock the loan application first before procceding the collection process.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something went wrong from the server.");
            }
        }

        // partial payment collection
        [Authorize]
        [HttpPut]
        [Route("api/collection/partialPayment/update/byId/byLoanId/{id}/{loanId}")]
        public HttpResponseMessage partialPaymentCollection(String id, String loanId, Models.TrnCollection collection)
        {
            try
            {
                var loanApplication = from d in db.trnLoanApplications where d.Id == Convert.ToInt32(loanId) select d;
                if (loanApplication.Any())
                {
                    if (loanApplication.FirstOrDefault().IsLocked)
                    {
                        var collections = from d in db.trnCollections
                                          where d.Id == Convert.ToInt32(id)
                                          select d;

                        if (collections.Any())
                        {
                            if (collections.FirstOrDefault().IsCurrentCollection)
                            {
                                if (!collections.FirstOrDefault().IsDueDate)
                                {
                                    if (collections.FirstOrDefault().CurrentBalanceAmount > 0)
                                    {
                                        if (!collections.FirstOrDefault().IsCleared && !collections.FirstOrDefault().IsAbsent)
                                        {
                                            if (collections.FirstOrDefault().IsAction)
                                            {
                                                if (collection.PaidAmount > 0)
                                                {
                                                    if (collections.FirstOrDefault().CurrentBalanceAmount >= collection.PaidAmount)
                                                    {
                                                        var isClearedValue = false;
                                                        var isPartiallyPaidValue = true;
                                                        if (collections.FirstOrDefault().CurrentBalanceAmount - collection.PaidAmount == 0)
                                                        {
                                                            isClearedValue = true;
                                                            isPartiallyPaidValue = false;
                                                        }

                                                        var updateCollection = collections.FirstOrDefault();
                                                        updateCollection.PaidAmount = collection.PaidAmount;
                                                        updateCollection.CurrentBalanceAmount = (collections.FirstOrDefault().CollectibleAmount + collections.FirstOrDefault().PreviousBalanceAmount) - collection.PaidAmount;
                                                        updateCollection.IsCleared = isClearedValue;
                                                        updateCollection.IsAbsent = false;
                                                        updateCollection.IsPartiallyPaid = isPartiallyPaidValue;
                                                        updateCollection.IsCurrentCollection = false;
                                                        updateCollection.IsProcessed = true;
                                                        updateCollection.IsAdvancedPaid = false;
                                                        db.SubmitChanges();

                                                        var collectionPrevoiusDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == collections.FirstOrDefault().CollectionDate.Date.AddDays(-1) select d;
                                                        if (collectionPrevoiusDate.Any())
                                                        {
                                                            var updateCollectionPrevoiusDate = collectionPrevoiusDate.FirstOrDefault();
                                                            updateCollectionPrevoiusDate.IsAction = false;
                                                            updateCollectionPrevoiusDate.IsCurrentCollection = false;
                                                            db.SubmitChanges();

                                                            var collectionNextDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == collections.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                                            if (collectionNextDate.Any())
                                                            {
                                                                var updateCollectionNextDate = collectionNextDate.FirstOrDefault();
                                                                updateCollectionNextDate.PreviousBalanceAmount = (collections.FirstOrDefault().CollectibleAmount + collections.FirstOrDefault().PreviousBalanceAmount) - collection.PaidAmount;
                                                                updateCollectionNextDate.CurrentBalanceAmount = collectionNextDate.FirstOrDefault().CollectibleAmount + ((collections.FirstOrDefault().CollectibleAmount + collections.FirstOrDefault().PreviousBalanceAmount) - collection.PaidAmount);
                                                                updateCollectionNextDate.IsAction = true;
                                                                updateCollectionNextDate.IsCurrentCollection = true;
                                                                db.SubmitChanges();
                                                            }
                                                            else
                                                            {
                                                                if (collections.FirstOrDefault().CurrentBalanceAmount == 0)
                                                                {
                                                                    var updateLoanApplicationFullPayment = loanApplication.FirstOrDefault();
                                                                    updateLoanApplicationFullPayment.IsFullyPaid = true;
                                                                    db.SubmitChanges();
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var collectionNextDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == collections.FirstOrDefault().CollectionDate.Date.AddDays(1) select d;
                                                            if (collectionNextDate.Any())
                                                            {
                                                                var updateCollectionNextDate = collectionNextDate.FirstOrDefault();
                                                                updateCollectionNextDate.PreviousBalanceAmount = (collections.FirstOrDefault().CollectibleAmount + collections.FirstOrDefault().PreviousBalanceAmount) - collection.PaidAmount;
                                                                updateCollectionNextDate.CurrentBalanceAmount = collectionNextDate.FirstOrDefault().CollectibleAmount + ((collections.FirstOrDefault().CollectibleAmount + collections.FirstOrDefault().PreviousBalanceAmount) - collection.PaidAmount);
                                                                updateCollectionNextDate.IsAction = true;
                                                                updateCollectionNextDate.IsCurrentCollection = true;
                                                                db.SubmitChanges();
                                                            }
                                                            else
                                                            {
                                                                if (collections.FirstOrDefault().CurrentBalanceAmount == 0)
                                                                {
                                                                    var updateLoanApplicationFullPayment = loanApplication.FirstOrDefault();
                                                                    updateLoanApplicationFullPayment.IsFullyPaid = true;
                                                                    db.SubmitChanges();
                                                                }
                                                            }
                                                        }

                                                        return Request.CreateResponse(HttpStatusCode.OK);
                                                    }
                                                    else
                                                    {
                                                        return Request.CreateResponse(HttpStatusCode.BadRequest, "The amount to be paid must not be greater than the current balance amount.");
                                                    }
                                                }
                                                else
                                                {
                                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Zero(0) amount. Please Enter an amount for partial payment.");
                                                }
                                            }
                                            else
                                            {
                                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot apply actions by this time.");
                                            }
                                        }
                                        else
                                        {
                                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Collection actions has already been applied");
                                        }
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest, "No current balance.");
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry but the collection is in due date.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry, the collection date is not the current collection. Please close the modal and reopen it to update the details. Then try again");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Please lock the loan application first before procceding the collection process.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something went wrong from the server.");
            }
        }

        // advance payment collection
        [Authorize]
        [HttpPut]
        [Route("api/collection/advancePayment/update/byId/byLoanId/{id}/{loanId}")]
        public HttpResponseMessage advancePaymentCollection(String id, String loanId)
        {
            try
            {
                var loanApplication = from d in db.trnLoanApplications where d.Id == Convert.ToInt32(loanId) select d;
                if (loanApplication.Any())
                {
                    if (loanApplication.FirstOrDefault().IsLocked)
                    {
                        var currentCollection = from d in db.trnCollections where d.IsCurrentCollection == true select d;
                        if (currentCollection.Any())
                        {
                            var advancePaymentCollections = from d in db.trnCollections
                                                            where d.IsProcessed == false
                                                            && d.Id >= currentCollection.FirstOrDefault().Id
                                                            && d.Id <= Convert.ToInt32(id)
                                                            select new Models.TrnCollection
                                                            {
                                                                Id = d.Id,
                                                                CollectionDate = d.CollectionDate.ToShortDateString(),
                                                                CollectibleAmount = d.CollectibleAmount,
                                                                PreviousBalanceAmount = d.PreviousBalanceAmount,
                                                                CurrentBalanceAmount = d.CurrentBalanceAmount
                                                            };

                            if (advancePaymentCollections.Any())
                            {
                                foreach (var advancePaymentCollection in advancePaymentCollections)
                                {
                                    var collectionForUpdate = from d in db.trnCollections where d.Id == advancePaymentCollection.Id select d;
                                    if (collectionForUpdate.Any())
                                    {
                                        var updateCollection = collectionForUpdate.FirstOrDefault();
                                        updateCollection.PaidAmount = advancePaymentCollection.CollectibleAmount + advancePaymentCollection.PreviousBalanceAmount;
                                        updateCollection.PreviousBalanceAmount = 0;
                                        updateCollection.CurrentBalanceAmount = 0;
                                        updateCollection.PenaltyAmount = 0;
                                        updateCollection.IsCleared = true;
                                        updateCollection.IsAbsent = false;
                                        updateCollection.IsProcessed = true;
                                        updateCollection.IsCurrentCollection = false;
                                        updateCollection.IsPartiallyPaid = false;
                                        updateCollection.IsAdvancedPaid = true;
                                        updateCollection.IsAction = false;
                                        db.SubmitChanges();

                                        var collectionPrevoiusDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == Convert.ToDateTime(advancePaymentCollection.CollectionDate).Date.AddDays(-1) select d;
                                        if (collectionPrevoiusDate.Any())
                                        {
                                            var updateCollectionPrevoiusDate = collectionPrevoiusDate.FirstOrDefault();
                                            updateCollectionPrevoiusDate.IsAction = false;
                                            updateCollectionPrevoiusDate.IsCurrentCollection = false;
                                            db.SubmitChanges();

                                            var collectionNextDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == Convert.ToDateTime(advancePaymentCollection.CollectionDate).Date.AddDays(1) select d;
                                            if (collectionNextDate.Any())
                                            {
                                                var updateCollectionNextDate = collectionNextDate.FirstOrDefault();
                                                updateCollectionNextDate.IsAction = true;
                                                updateCollectionNextDate.IsCurrentCollection = true;
                                                db.SubmitChanges();
                                            }
                                            else
                                            {
                                                if (advancePaymentCollection.CurrentBalanceAmount == 0)
                                                {
                                                    var updateLoanApplicationFullPayment = loanApplication.FirstOrDefault();
                                                    updateLoanApplicationFullPayment.IsFullyPaid = true;
                                                    db.SubmitChanges();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var collectionNextDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == Convert.ToDateTime(advancePaymentCollection.CollectionDate).Date.AddDays(1) select d;
                                            if (collectionNextDate.Any())
                                            {
                                                var updateCollectionNextDate = collectionNextDate.FirstOrDefault();
                                                updateCollectionNextDate.IsAction = true;
                                                updateCollectionNextDate.IsCurrentCollection = true;
                                                db.SubmitChanges();
                                            }
                                            else
                                            {
                                                if (advancePaymentCollection.CurrentBalanceAmount == 0)
                                                {
                                                    var updateLoanApplicationFullPayment = loanApplication.FirstOrDefault();
                                                    updateLoanApplicationFullPayment.IsFullyPaid = true;
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
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "No Data found from the server.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "No current collection.");
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
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something went wrong from the server. Please contact the administrator.");
            }
        }

        // full payment collection
        [Authorize]
        [HttpPut]
        [Route("api/collection/fullPayment/update/byId/byLoanId/{id}/{loanId}")]
        public HttpResponseMessage fullPaymentCollection(String id, String loanId)
        {
            try
            {
                var loanApplication = from d in db.trnLoanApplications where d.Id == Convert.ToInt32(loanId) select d;
                if (loanApplication.Any())
                {
                    if (loanApplication.FirstOrDefault().IsLocked)
                    {
                        var collections = from d in db.trnCollections where d.Id == Convert.ToInt32(id) select d;
                        if (collections.Any())
                        {
                            if (collections.FirstOrDefault().IsAction)
                            {
                                var collectionDateSequences = from d in db.trnCollections
                                                              where d.LoanId == Convert.ToInt32(loanId)
                                                              && d.CollectionDate >= collections.FirstOrDefault().CollectionDate
                                                              select new Models.TrnCollection
                                                              {
                                                                  Id = d.Id,
                                                                  CollectionDate = d.CollectionDate.ToShortDateString(),
                                                                  CollectibleAmount = d.CollectibleAmount,
                                                                  PaidAmount = d.PaidAmount,
                                                                  PreviousBalanceAmount = d.PreviousBalanceAmount,
                                                                  CurrentBalanceAmount = d.CurrentBalanceAmount,
                                                                  CurrentCollectorId = d.trnLoanApplication.CurrentCollectorId,
                                                                  IsDueDate = d.IsDueDate,
                                                              };

                                if (collectionDateSequences.Any())
                                {
                                    foreach (var collectionDateSequence in collectionDateSequences)
                                    {
                                        var collectionDividedDate = from d in db.trnCollections
                                                                    where d.Id == collectionDateSequence.Id
                                                                    select d;

                                        if (collectionDividedDate.Any())
                                        {
                                            var updateCollectionDividedDate = collectionDividedDate.FirstOrDefault();
                                            updateCollectionDividedDate.PaidAmount = collectionDateSequence.CollectibleAmount + collectionDateSequence.PenaltyAmount + collectionDateSequence.PreviousBalanceAmount;
                                            updateCollectionDividedDate.PreviousBalanceAmount = collectionDateSequence.PreviousBalanceAmount;
                                            updateCollectionDividedDate.CurrentBalanceAmount = 0;
                                            updateCollectionDividedDate.IsCleared = true;
                                            updateCollectionDividedDate.IsAbsent = false;
                                            updateCollectionDividedDate.IsAdvancedPaid = false;
                                            updateCollectionDividedDate.IsCurrentCollection = false;
                                            updateCollectionDividedDate.IsProcessed = true;
                                            updateCollectionDividedDate.IsAction = false;
                                            db.SubmitChanges();
                                        }
                                    }

                                    var updateLoanApplicationFullPayment = loanApplication.FirstOrDefault();
                                    updateLoanApplicationFullPayment.IsFullyPaid = true;
                                    db.SubmitChanges();
                                }

                                return Request.CreateResponse(HttpStatusCode.OK);
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

        // get penalty amount
        [Authorize]
        [HttpGet]
        [Route("api/collection/penaltyAmount/get/byLoanId/byCollectionDate/{loanId}/{collectionDate}")]
        public Decimal getPenaltyAmount(Int32 loanId, String collectionDate)
        {
            Decimal penaltyAmount = 10;
            var previousCollectionDay = from d in db.trnCollections
                                        where d.LoanId == loanId
                                        && d.CollectionDate == Convert.ToDateTime(collectionDate).Date.AddDays(-1)
                                        select d;

            if (previousCollectionDay.Any())
            {
                if (previousCollectionDay.FirstOrDefault().IsAbsent)
                {
                    if (previousCollectionDay.FirstOrDefault().PenaltyAmount == 10)
                    {
                        var previousCollectionDay2 = from d in db.trnCollections
                                                     where d.LoanId == loanId
                                                     && d.CollectionDate == Convert.ToDateTime(collectionDate).Date.AddDays(-2)
                                                     select d;

                        if (previousCollectionDay2.Any())
                        {
                            if (previousCollectionDay2.FirstOrDefault().IsAbsent)
                            {
                                if (previousCollectionDay2.FirstOrDefault().PenaltyAmount == 10)
                                {
                                    penaltyAmount = 20;
                                }
                                else
                                {
                                    penaltyAmount = 10;
                                }
                            }
                        }
                    }
                }
            }

            return penaltyAmount;
        }

        // get advance payment total balance to be paid.
        public Decimal getAdvancePaymentTotalCurrentBalance(Int32 loanId, String collectionStarDate, String collectionEndDate)
        {
            var collectionDateSequences = from d in db.trnCollections
                                          where d.LoanId == Convert.ToInt32(loanId)
                                          && d.CollectionDate >= Convert.ToDateTime(collectionStarDate)
                                          && d.CollectionDate <= Convert.ToDateTime(collectionEndDate)
                                          select d;

            Decimal currentBalanceAmount = 0;
            if (collectionDateSequences.Any())
            {
                currentBalanceAmount = collectionDateSequences.Sum(d => d.CurrentBalanceAmount);
            }

            return currentBalanceAmount;
        }

        // get colllection from loan application fully paid
        [Authorize]
        [HttpGet]
        [Route("api/collection/isFullyPaid/get/byId/{id}")]
        public Boolean getIsFullyPaidCollection(String id)
        {
            var collection = from d in db.trnCollections
                             where d.Id == Convert.ToInt32(id)
                             select d;

            Boolean isFullyPaidValue = false;
            if (collection.Any())
            {
                isFullyPaidValue = collection.FirstOrDefault().trnLoanApplication.IsFullyPaid;
            }

            return isFullyPaidValue;
        }


        // get collection previous balance paid
        [Authorize]
        [HttpGet]
        [Route("api/collection/previousBalance/get/byLoanId/byCollectionDate/{loanId}/{collectionDate}")]
        public Decimal getPreviousBalanceCollection(String loanId, String collectionDate)
        {
            var collectionNextDate = from d in db.trnCollections where d.LoanId == Convert.ToInt32(loanId) && d.CollectionDate == Convert.ToDateTime(collectionDate).Date.AddDays(-1) select d;

            Decimal previousBalanceValue = 0;
            if (collectionNextDate.Any())
            {
                previousBalanceValue = collectionNextDate.FirstOrDefault().CurrentBalanceAmount;
            }

            return previousBalanceValue;
        }
    }
}
