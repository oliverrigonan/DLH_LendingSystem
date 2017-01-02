using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;


namespace Lending.ApiControllers
{
    public class ApiDailyCollectionController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();
        private Business.CollectionStatus collectionStatus = new Business.CollectionStatus();

        // daily collection list by collectionId for advance payment
        [Authorize]
        [HttpGet]
        [Route("api/dailyCollection/list/advancePayment/byCollectionId/{collectionId}")]
        public List<Models.TrnDailyCollection> listDailyCollectionForAdvancePaymentByCollectionId(String collectionId)
        {
            var dailyCollections = from d in db.trnDailyCollections
                                   where d.CollectionId == Convert.ToInt32(collectionId)
                                   && d.IsProcessed == false
                                   select new Models.TrnDailyCollection
                                   {
                                       Id = d.Id,
                                       CollectionId = d.CollectionId,
                                       CollectionNumber = d.trnCollection.CollectionNumber,
                                       Applicant = d.trnCollection.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnCollection.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + (d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName != null ? d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName : " "),
                                       DayNumber = d.DayNumber,
                                       DailyCollectionDate = d.DailyCollectionDate.ToShortDateString(),
                                       NetAmount = d.NetAmount,
                                       CollectibleAmount = d.CollectibleAmount,
                                       PenaltyAmount = d.PenaltyAmount,
                                       PaidAmount = d.PaidAmount,
                                       PreviousBalanceAmount = d.PreviousBalanceAmount,
                                       CurrentBalanceAmount = d.CurrentBalanceAmount,
                                       IsCurrentCollection = d.IsCurrentCollection,
                                       IsCleared = d.IsCleared,
                                       IsAbsent = d.IsAbsent,
                                       IsPartiallyPaid = d.IsPartiallyPaid,
                                       IsPaidInAdvanced = d.IsPaidInAdvanced,
                                       IsFullyPaid = d.IsFullyPaid,
                                       IsProcessed = d.IsProcessed,
                                       CanPerformAction = d.CanPerformAction,
                                       IsDueDate = d.IsDueDate,
                                       IsAllowanceDay = d.IsAllowanceDay,
                                       IsLastDay = d.IsLastDay,
                                       ReconstructId = d.ReconstructId != null ? d.ReconstructId : 0,
                                       IsReconstructed = d.IsReconstructed,
                                       Status = collectionStatus.getStatus(d.IsCleared, d.IsAbsent, d.IsPartiallyPaid, d.IsPaidInAdvanced, d.IsFullyPaid, d.trnCollection.IsOverdue, d.IsReconstructed)
                                   };

            return dailyCollections.ToList();
        }

        // get daily collection current collection from daily collection by collectionId
        [Authorize]
        [HttpGet]
        [Route("api/dailyCollection/get/currentCollection/byCollectionId/{collectionId}")]
        public Models.TrnDailyCollection getDailyCollectionCurrentCollectionForPartialPaymentByCollectionId(String collectionId)
        {
            var dailyCollections = from d in db.trnDailyCollections
                                   where d.CollectionId == Convert.ToInt32(collectionId)
                                   && d.IsCurrentCollection == true
                                   select new Models.TrnDailyCollection
                                   {
                                       Id = d.Id,
                                       CollectionId = d.CollectionId,
                                       CollectionNumber = d.trnCollection.CollectionNumber,
                                       Applicant = d.trnCollection.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnCollection.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + (d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName != null ? d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName : " "),
                                       DayNumber = d.DayNumber,
                                       DailyCollectionDate = d.DailyCollectionDate.ToShortDateString(),
                                       NetAmount = d.NetAmount,
                                       CollectibleAmount = d.CollectibleAmount,
                                       PenaltyAmount = d.PenaltyAmount,
                                       PaidAmount = d.PaidAmount,
                                       PreviousBalanceAmount = d.PreviousBalanceAmount,
                                       CurrentBalanceAmount = d.CurrentBalanceAmount,
                                       IsCurrentCollection = d.IsCurrentCollection,
                                       IsCleared = d.IsCleared,
                                       IsAbsent = d.IsAbsent,
                                       IsPartiallyPaid = d.IsPartiallyPaid,
                                       IsPaidInAdvanced = d.IsPaidInAdvanced,
                                       IsFullyPaid = d.IsFullyPaid,
                                       IsProcessed = d.IsProcessed,
                                       CanPerformAction = d.CanPerformAction,
                                       IsDueDate = d.IsDueDate,
                                       IsAllowanceDay = d.IsAllowanceDay,
                                       IsLastDay = d.IsLastDay,
                                       ReconstructId = d.ReconstructId != null ? d.ReconstructId : 0,
                                       IsReconstructed = d.IsReconstructed,
                                       Status = collectionStatus.getStatus(d.IsCleared, d.IsAbsent, d.IsPartiallyPaid, d.IsPaidInAdvanced, d.IsFullyPaid, d.trnCollection.IsOverdue, d.IsReconstructed)
                                   };

            return (Models.TrnDailyCollection)dailyCollections.FirstOrDefault();
        }

        // daily collection list by collectionId
        [Authorize]
        [HttpGet]
        [Route("api/dailyCollection/list/byCollectionId/{collectionId}")]
        public List<Models.TrnDailyCollection> listDailyCollectionByCollectionId(String collectionId)
        {
            var dailyCollections = from d in db.trnDailyCollections
                                   where d.CollectionId == Convert.ToInt32(collectionId)
                                   select new Models.TrnDailyCollection
                                   {
                                       Id = d.Id,
                                       CollectionId = d.CollectionId,
                                       CollectionNumber = d.trnCollection.CollectionNumber,
                                       Applicant = d.trnCollection.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnCollection.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + (d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName != null ? d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName : " "),
                                       DayNumber = d.DayNumber,
                                       DailyCollectionDate = d.DailyCollectionDate.ToShortDateString(),
                                       NetAmount = d.NetAmount,
                                       CollectibleAmount = d.CollectibleAmount,
                                       PenaltyAmount = d.PenaltyAmount,
                                       PaidAmount = d.PaidAmount,
                                       PreviousBalanceAmount = d.PreviousBalanceAmount,
                                       CurrentBalanceAmount = d.CurrentBalanceAmount,
                                       IsCurrentCollection = d.IsCurrentCollection,
                                       IsCleared = d.IsCleared,
                                       IsAbsent = d.IsAbsent,
                                       IsPartiallyPaid = d.IsPartiallyPaid,
                                       IsPaidInAdvanced = d.IsPaidInAdvanced,
                                       IsFullyPaid = d.IsFullyPaid,
                                       IsProcessed = d.IsProcessed,
                                       CanPerformAction = d.CanPerformAction,
                                       IsDueDate = d.IsDueDate,
                                       IsAllowanceDay = d.IsAllowanceDay,
                                       IsLastDay = d.IsLastDay,
                                       ReconstructId = d.ReconstructId != null ? d.ReconstructId : 0,
                                       IsReconstructed = d.IsReconstructed,
                                       Status = collectionStatus.getStatus(d.IsCleared, d.IsAbsent, d.IsPartiallyPaid, d.IsPaidInAdvanced, d.IsFullyPaid, d.trnCollection.IsOverdue, d.IsReconstructed)
                                   };

            return dailyCollections.ToList();
        }

        // daily collection list by collectionDate and by areaId
        [Authorize]
        [HttpGet]
        [Route("api/dailyCollection/list/byCollectionDate/byAreaId/{dailyCollectionDate}/{areaId}")]
        public List<Models.TrnDailyCollection> listDailyCollectionByCollectionDateByAreaId(String dailyCollectionDate, String areaId)
        {
            var dailyCollections = from d in db.trnDailyCollections
                                   where d.DailyCollectionDate == Convert.ToDateTime(dailyCollectionDate)
                                   && d.trnCollection.trnLoanApplication.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                   select new Models.TrnDailyCollection
                                   {
                                       Id = d.Id,
                                       CollectionId = d.CollectionId,
                                       CollectionNumber = d.trnCollection.CollectionNumber,
                                       Applicant = d.trnCollection.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnCollection.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + (d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName != null ? d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName : " "),
                                       DayNumber = d.DayNumber,
                                       DailyCollectionDate = d.DailyCollectionDate.ToShortDateString(),
                                       NetAmount = d.NetAmount,
                                       CollectibleAmount = d.CollectibleAmount,
                                       PenaltyAmount = d.PenaltyAmount,
                                       PaidAmount = d.PaidAmount,
                                       PreviousBalanceAmount = d.PreviousBalanceAmount,
                                       CurrentBalanceAmount = d.CurrentBalanceAmount,
                                       IsCurrentCollection = d.IsCurrentCollection,
                                       IsCleared = d.IsCleared,
                                       IsAbsent = d.IsAbsent,
                                       IsPartiallyPaid = d.IsPartiallyPaid,
                                       IsPaidInAdvanced = d.IsPaidInAdvanced,
                                       IsFullyPaid = d.IsFullyPaid,
                                       IsProcessed = d.IsProcessed,
                                       CanPerformAction = d.CanPerformAction,
                                       IsDueDate = d.IsDueDate,
                                       IsAllowanceDay = d.IsAllowanceDay,
                                       IsLastDay = d.IsLastDay,
                                       ReconstructId = d.ReconstructId != null ? d.ReconstructId : 0,
                                       IsReconstructed = d.IsReconstructed,
                                       Status = collectionStatus.getStatus(d.IsCleared, d.IsAbsent, d.IsPartiallyPaid, d.IsPaidInAdvanced, d.IsFullyPaid, d.trnCollection.IsOverdue, d.IsReconstructed)
                                   };

            return dailyCollections.ToList();
        }

        // get daily collection penalty amount
        [Authorize]
        [HttpGet]
        [Route("api/dailyCollection/get/penaltyAmount/byCollectionId/byDayNumber/{collectionId}/{dayNumber}")]
        public Decimal getDailyCollectionPenaltyAmount(String collectionId, String dayNumber)
        {
            var collection = from d in db.trnCollections where d.Id == Convert.ToInt32(collectionId) select d;

            Decimal defaultPenaltyAmount = collection.FirstOrDefault().trnLoanApplication.mstPenalty.DefaultPenaltyAmount;
            Decimal numberOfLimitAbsent = collection.FirstOrDefault().trnLoanApplication.mstPenalty.NoOfLimitAbsent;

            Int32 absentCount = 0;
            for (Int32 i = Convert.ToInt32(numberOfLimitAbsent); i >= 1; i--)
            {
                var dailyColletion = from d in db.trnDailyCollections
                                     where d.CollectionId == Convert.ToInt32(collectionId)
                                     && d.DayNumber == Convert.ToDecimal(Convert.ToDecimal(dayNumber) - i)
                                     select d;

                if (dailyColletion.Any())
                {
                    if (dailyColletion.FirstOrDefault().IsAbsent)
                    {
                        if (dailyColletion.FirstOrDefault().PenaltyAmount == defaultPenaltyAmount)
                        {
                            absentCount += 1;
                            if (absentCount == Convert.ToInt32(numberOfLimitAbsent))
                            {
                                defaultPenaltyAmount = collection.FirstOrDefault().trnLoanApplication.mstPenalty.PenaltyAmountOverNoOfLimitAbsent;
                            }
                        }
                    }
                    else
                    {
                        absentCount -= 1;
                    }
                }
            }

            return defaultPenaltyAmount;
        }

        // get daily collection last current collection
        [Authorize]
        [HttpGet]
        [Route("api/dailyCollection/get/lastCurrentCollection/byCollectionId/{collectionId}")]
        public Models.TrnDailyCollection getDailyCollectionLastCurrentCollectionByCollectionId(String collectionId)
        {
            var dailyCollection = from d in db.trnDailyCollections.OrderByDescending(d => d.Id)
                                  where d.CollectionId == Convert.ToInt32(collectionId)
                                  && d.IsLastDay == true
                                  && d.IsProcessed == true
                                  select new Models.TrnDailyCollection
                                  {
                                      Id = d.Id,
                                      CollectionId = d.CollectionId,
                                      CollectionNumber = d.trnCollection.CollectionNumber,
                                      Applicant = d.trnCollection.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnCollection.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + (d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName != null ? d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName : " "),
                                      DayNumber = d.DayNumber,
                                      DailyCollectionDate = d.DailyCollectionDate.ToShortDateString(),
                                      NetAmount = d.NetAmount,
                                      CollectibleAmount = d.CollectibleAmount,
                                      PenaltyAmount = d.PenaltyAmount,
                                      PaidAmount = d.PaidAmount,
                                      PreviousBalanceAmount = d.PreviousBalanceAmount,
                                      CurrentBalanceAmount = d.CurrentBalanceAmount,
                                      IsCurrentCollection = d.IsCurrentCollection,
                                      IsCleared = d.IsCleared,
                                      IsAbsent = d.IsAbsent,
                                      IsPartiallyPaid = d.IsPartiallyPaid,
                                      IsPaidInAdvanced = d.IsPaidInAdvanced,
                                      IsFullyPaid = d.IsFullyPaid,
                                      IsProcessed = d.IsProcessed,
                                      CanPerformAction = d.CanPerformAction,
                                      IsDueDate = d.IsDueDate,
                                      IsAllowanceDay = d.IsAllowanceDay,
                                      IsLastDay = d.IsLastDay,
                                      ReconstructId = d.ReconstructId != null ? d.ReconstructId : 0,
                                      IsReconstructed = d.IsReconstructed,
                                      Status = collectionStatus.getStatus(d.IsCleared, d.IsAbsent, d.IsPartiallyPaid, d.IsPaidInAdvanced, d.IsFullyPaid, d.trnCollection.IsOverdue, d.IsReconstructed)
                                  };

            return (Models.TrnDailyCollection)dailyCollection.FirstOrDefault();
        }

        // get daily collection isFullyPaid
        [Authorize]
        [HttpGet]
        [Route("api/dailyCollection/get/isFullyPaid/byId/{id}")]
        public Boolean getDailyCollectionIsFullyPaid(String id)
        {
            var dailyCollection = from d in db.trnDailyCollections
                                  where d.Id == Convert.ToInt32(id)
                                  select d;

            Boolean isFullyPaidValue = false;
            if (dailyCollection.Any())
            {
                isFullyPaidValue = dailyCollection.FirstOrDefault().trnCollection.IsFullyPaid;
            }

            return isFullyPaidValue;
        }

        // get daily collection previous balance paid by collection Id and daynumber
        [Authorize]
        [HttpGet]
        [Route("api/dailyCollection/get/previousBalance/byCollectionId/byDayNumber/{collectionId}/{dayNumber}")]
        public Decimal getDailyCollectionPreviousBalance(String collectionId, String dayNumber)
        {
            var collection = from d in db.trnCollections where d.Id == Convert.ToInt32(collectionId) select d;
            var dailyCollectionNextDate = from d in db.trnDailyCollections where d.CollectionId == Convert.ToInt32(collectionId) && d.DayNumber == Convert.ToDecimal(dayNumber + 1) select d;

            Decimal previousBalanceValue = 0;
            if (dailyCollectionNextDate.Any())
            {
                previousBalanceValue = dailyCollectionNextDate.FirstOrDefault().CurrentBalanceAmount;
            }

            return previousBalanceValue;
        }

        // clear daily collection
        [Authorize]
        [HttpPut]
        [Route("api/dailyCollection/cleared/byId/{id}")]
        public HttpResponseMessage clearedDailyCollection(String id)
        {
            try
            {
                var dailyCollection = from d in db.trnDailyCollections where d.Id == Convert.ToInt32(id) select d;
                if (dailyCollection.Any())
                {
                    if (dailyCollection.FirstOrDefault().trnCollection.trnLoanApplication.IsLocked)
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
                            String matchPageString = "CollectionList";
                            String matchPageString2 = "CollectionDetail";
                            Boolean canPerformActions = false;

                            foreach (var mstUserForm in mstUserForms)
                            {
                                if (mstUserForm.Form.Equals(matchPageString) || mstUserForm.Form.Equals(matchPageString2))
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
                                if (dailyCollection.FirstOrDefault().CurrentBalanceAmount > 0)
                                {
                                    if (!dailyCollection.FirstOrDefault().IsCleared)
                                    {
                                        if (dailyCollection.FirstOrDefault().CanPerformAction)
                                        {
                                            var updateDailyCollection = dailyCollection.FirstOrDefault();
                                            updateDailyCollection.PaidAmount = dailyCollection.FirstOrDefault().CollectibleAmount + dailyCollection.FirstOrDefault().PreviousBalanceAmount;
                                            updateDailyCollection.CurrentBalanceAmount = 0;
                                            updateDailyCollection.PenaltyAmount = 0;
                                            updateDailyCollection.IsCurrentCollection = false;
                                            updateDailyCollection.IsCleared = true;
                                            updateDailyCollection.IsAbsent = false;
                                            updateDailyCollection.IsPartiallyPaid = false;
                                            updateDailyCollection.IsPaidInAdvanced = false;
                                            updateDailyCollection.IsFullyPaid = false;
                                            updateDailyCollection.IsProcessed = true;
                                            db.SubmitChanges();

                                            var dailyCollectionPrevoiusDate = from d in db.trnDailyCollections where d.CollectionId == Convert.ToInt32(dailyCollection.FirstOrDefault().CollectionId) && d.DayNumber == (dailyCollection.FirstOrDefault().DayNumber - 1) select d;
                                            if (dailyCollectionPrevoiusDate.Any())
                                            {
                                                var updateDailyCollectionPrevoiusDate = dailyCollectionPrevoiusDate.FirstOrDefault();
                                                updateDailyCollectionPrevoiusDate.IsCurrentCollection = false;
                                                updateDailyCollectionPrevoiusDate.CanPerformAction = false;
                                                db.SubmitChanges();

                                                var dailyCollectionNextDate = from d in db.trnDailyCollections where d.CollectionId == Convert.ToInt32(dailyCollection.FirstOrDefault().CollectionId) && d.DayNumber == (dailyCollection.FirstOrDefault().DayNumber + 1) select d;
                                                if (dailyCollectionNextDate.Any())
                                                {
                                                    var updateDailyCollectionNextDate = dailyCollectionNextDate.FirstOrDefault();
                                                    updateDailyCollectionNextDate.PreviousBalanceAmount = 0;
                                                    updateDailyCollectionNextDate.CurrentBalanceAmount = dailyCollectionNextDate.FirstOrDefault().CollectibleAmount;
                                                    updateDailyCollectionNextDate.CanPerformAction = true;
                                                    updateDailyCollectionNextDate.IsCurrentCollection = true;
                                                    db.SubmitChanges();

                                                    if (updateDailyCollectionNextDate.CurrentBalanceAmount == 0)
                                                    {
                                                        var collection = from d in db.trnCollections where d.Id == dailyCollection.FirstOrDefault().CollectionId select d;
                                                        if (collection.Any())
                                                        {
                                                            var updateCollectionIsFullyPaid = collection.FirstOrDefault();
                                                            updateCollectionIsFullyPaid.IsFullyPaid = true;
                                                            db.SubmitChanges();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var collection = from d in db.trnCollections where d.Id == dailyCollection.FirstOrDefault().CollectionId select d;
                                                    if (collection.Any())
                                                    {
                                                        var updateCollectionIsFullyPaid = collection.FirstOrDefault();
                                                        updateCollectionIsFullyPaid.IsFullyPaid = true;
                                                        db.SubmitChanges();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var dailyCollectionNextDate = from d in db.trnDailyCollections where d.CollectionId == Convert.ToInt32(dailyCollection.FirstOrDefault().CollectionId) && d.DayNumber == (dailyCollection.FirstOrDefault().DayNumber + 1) select d;
                                                if (dailyCollectionNextDate.Any())
                                                {
                                                    var updateDailyCollectionNextDate = dailyCollectionNextDate.FirstOrDefault();
                                                    updateDailyCollectionNextDate.PreviousBalanceAmount = 0;
                                                    updateDailyCollectionNextDate.CurrentBalanceAmount = dailyCollectionNextDate.FirstOrDefault().CollectibleAmount;
                                                    updateDailyCollectionNextDate.CanPerformAction = true;
                                                    updateDailyCollectionNextDate.IsCurrentCollection = true;
                                                    db.SubmitChanges();

                                                    if (updateDailyCollectionNextDate.CurrentBalanceAmount == 0)
                                                    {
                                                        var collection = from d in db.trnCollections where d.Id == dailyCollection.FirstOrDefault().CollectionId select d;
                                                        if (collection.Any())
                                                        {
                                                            var updateCollectionIsFullyPaid = collection.FirstOrDefault();
                                                            updateCollectionIsFullyPaid.IsFullyPaid = true;
                                                            db.SubmitChanges();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var collection = from d in db.trnCollections where d.Id == dailyCollection.FirstOrDefault().CollectionId select d;
                                                    if (collection.Any())
                                                    {
                                                        var updateCollectionIsFullyPaid = collection.FirstOrDefault();
                                                        updateCollectionIsFullyPaid.IsFullyPaid = true;
                                                        db.SubmitChanges();
                                                    }
                                                }
                                            }

                                            Business.Journal journal = new Business.Journal();
                                            journal.postCollectionJournal(Convert.ToInt32(id));

                                            return Request.CreateResponse(HttpStatusCode.OK);
                                        }
                                        else
                                        {
                                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Can't process.");
                                        }
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Already cleared.");
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No current balance.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "No rights to perform some actions.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "No rights to access this page.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "The loan appliction must be locked before proceeding the collection.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No data found in the server.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something's went wrong from the server.");
            }
        }

        // absent daily collection
        [Authorize]
        [HttpPut]
        [Route("api/dailyCollection/absent/byId/{id}")]
        public HttpResponseMessage absentDailyCollection(String id)
        {
            try
            {
                var dailyCollection = from d in db.trnDailyCollections where d.Id == Convert.ToInt32(id) select d;
                if (dailyCollection.Any())
                {
                    if (dailyCollection.FirstOrDefault().trnCollection.trnLoanApplication.IsLocked)
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
                            String matchPageString = "CollectionList";
                            String matchPageString2 = "CollectionDetail";
                            Boolean canPerformActions = false;

                            foreach (var mstUserForm in mstUserForms)
                            {
                                if (mstUserForm.Form.Equals(matchPageString) || mstUserForm.Form.Equals(matchPageString2))
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
                                if (dailyCollection.FirstOrDefault().CurrentBalanceAmount > 0)
                                {
                                    if (!dailyCollection.FirstOrDefault().IsAbsent)
                                    {
                                        if (dailyCollection.FirstOrDefault().CanPerformAction)
                                        {
                                            if (!dailyCollection.FirstOrDefault().IsLastDay)
                                            {
                                                Decimal penaltyAmount = getDailyCollectionPenaltyAmount(dailyCollection.FirstOrDefault().CollectionId.ToString(), dailyCollection.FirstOrDefault().DayNumber.ToString());

                                                var updateDailyCollection = dailyCollection.FirstOrDefault();
                                                updateDailyCollection.PaidAmount = 0;
                                                updateDailyCollection.CurrentBalanceAmount = dailyCollection.FirstOrDefault().CollectibleAmount + penaltyAmount + dailyCollection.FirstOrDefault().PreviousBalanceAmount;
                                                updateDailyCollection.PenaltyAmount = penaltyAmount;
                                                updateDailyCollection.IsCurrentCollection = false;
                                                updateDailyCollection.IsCleared = false;
                                                updateDailyCollection.IsAbsent = true;
                                                updateDailyCollection.IsPartiallyPaid = false;
                                                updateDailyCollection.IsPaidInAdvanced = false;
                                                updateDailyCollection.IsFullyPaid = false;
                                                updateDailyCollection.IsProcessed = true;
                                                db.SubmitChanges();

                                                var dailyCollectionPrevoiusDate = from d in db.trnDailyCollections where d.CollectionId == Convert.ToInt32(dailyCollection.FirstOrDefault().CollectionId) && d.DayNumber == (dailyCollection.FirstOrDefault().DayNumber - 1) select d;
                                                if (dailyCollectionPrevoiusDate.Any())
                                                {
                                                    var updateDailyCollectionPrevoiusDate = dailyCollectionPrevoiusDate.FirstOrDefault();
                                                    updateDailyCollectionPrevoiusDate.IsCurrentCollection = false;
                                                    updateDailyCollectionPrevoiusDate.CanPerformAction = false;
                                                    db.SubmitChanges();

                                                    var dailyCollectionNextDate = from d in db.trnDailyCollections where d.CollectionId == Convert.ToInt32(dailyCollection.FirstOrDefault().CollectionId) && d.DayNumber == (dailyCollection.FirstOrDefault().DayNumber + 1) select d;
                                                    if (dailyCollectionNextDate.Any())
                                                    {
                                                        var updateDailyCollectionNextDate = dailyCollectionNextDate.FirstOrDefault();
                                                        updateDailyCollectionNextDate.PreviousBalanceAmount = dailyCollection.FirstOrDefault().CollectibleAmount + penaltyAmount + dailyCollection.FirstOrDefault().PreviousBalanceAmount;
                                                        updateDailyCollectionNextDate.CurrentBalanceAmount = dailyCollectionNextDate.FirstOrDefault().CollectibleAmount + (dailyCollection.FirstOrDefault().CollectibleAmount + penaltyAmount + dailyCollection.FirstOrDefault().PreviousBalanceAmount);
                                                        updateDailyCollectionNextDate.CanPerformAction = true;
                                                        updateDailyCollectionNextDate.IsCurrentCollection = true;
                                                        db.SubmitChanges();
                                                    }
                                                }
                                                else
                                                {
                                                    var dailyCollectionNextDate = from d in db.trnDailyCollections where d.CollectionId == Convert.ToInt32(dailyCollection.FirstOrDefault().CollectionId) && d.DayNumber == (dailyCollection.FirstOrDefault().DayNumber + 1) select d;
                                                    if (dailyCollectionNextDate.Any())
                                                    {
                                                        var updateDailyCollectionNextDate = dailyCollectionNextDate.FirstOrDefault();
                                                        updateDailyCollectionNextDate.PreviousBalanceAmount = dailyCollection.FirstOrDefault().CollectibleAmount + penaltyAmount + dailyCollection.FirstOrDefault().PreviousBalanceAmount;
                                                        updateDailyCollectionNextDate.CurrentBalanceAmount = dailyCollectionNextDate.FirstOrDefault().CollectibleAmount + (dailyCollection.FirstOrDefault().CollectibleAmount + penaltyAmount + dailyCollection.FirstOrDefault().PreviousBalanceAmount);
                                                        updateDailyCollectionNextDate.CanPerformAction = true;
                                                        updateDailyCollectionNextDate.IsCurrentCollection = true;
                                                        db.SubmitChanges();
                                                    }
                                                }

                                                var collection = from d in db.trnCollections where d.Id == dailyCollection.FirstOrDefault().CollectionId select d;
                                                if (collection.Any())
                                                {
                                                    var updateCollectionIsFullyPaid = collection.FirstOrDefault();
                                                    updateCollectionIsFullyPaid.IsFullyPaid = false;
                                                    db.SubmitChanges();
                                                }

                                                Business.Journal journal = new Business.Journal();
                                                journal.deleteCollectionJournal(Convert.ToInt32(id));
                                            }
                                            else
                                            {
                                                Decimal penaltyAmount = getDailyCollectionPenaltyAmount(dailyCollection.FirstOrDefault().CollectionId.ToString(), dailyCollection.FirstOrDefault().DayNumber.ToString());

                                                var updateDailyCollection = dailyCollection.FirstOrDefault();
                                                updateDailyCollection.PaidAmount = 0;
                                                updateDailyCollection.CurrentBalanceAmount = dailyCollection.FirstOrDefault().CollectibleAmount + penaltyAmount + dailyCollection.FirstOrDefault().PreviousBalanceAmount;
                                                updateDailyCollection.PenaltyAmount = penaltyAmount;
                                                updateDailyCollection.IsCurrentCollection = false;
                                                updateDailyCollection.IsCleared = false;
                                                updateDailyCollection.IsAbsent = true;
                                                updateDailyCollection.IsPartiallyPaid = false;
                                                updateDailyCollection.IsPaidInAdvanced = false;
                                                updateDailyCollection.IsFullyPaid = false;
                                                updateDailyCollection.IsProcessed = true;
                                                db.SubmitChanges();
                                            }

                                            return Request.CreateResponse(HttpStatusCode.OK);
                                        }
                                        else
                                        {
                                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Can't process");
                                        }
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Already absent.");
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No Current Balance.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "No rights to perform some actions.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "No rights to access this page.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "The loan appliction must be locked before proceeding the collection.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No data found in the server.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something's went wrong from the server.");
            }
        }

        // undo changes daily collection
        [Authorize]
        [HttpPut]
        [Route("api/dailyCollection/undoChanges/byId/{id}")]
        public HttpResponseMessage undoChangesDailyCollection(String id)
        {
            try
            {
                var dailyCollection = from d in db.trnDailyCollections where d.Id == Convert.ToInt32(id) select d;
                if (dailyCollection.Any())
                {
                    if (dailyCollection.FirstOrDefault().trnCollection.trnLoanApplication.IsLocked)
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
                            String matchPageString = "CollectionList";
                            String matchPageString2 = "CollectionDetail";
                            Boolean canPerformActions = false;

                            foreach (var mstUserForm in mstUserForms)
                            {
                                if (mstUserForm.Form.Equals(matchPageString) || mstUserForm.Form.Equals(matchPageString2))
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
                                if (dailyCollection.FirstOrDefault().CanPerformAction)
                                {
                                    var dailCollectionPrevoiusDate = from d in db.trnDailyCollections where d.CollectionId == dailyCollection.FirstOrDefault().CollectionId && d.DayNumber == (dailyCollection.FirstOrDefault().DayNumber - 1) select d;
                                    if (dailCollectionPrevoiusDate.Any())
                                    {
                                        var updateDailyCollection = dailyCollection.FirstOrDefault();
                                        updateDailyCollection.PaidAmount = 0;
                                        updateDailyCollection.PreviousBalanceAmount = dailCollectionPrevoiusDate.FirstOrDefault().CurrentBalanceAmount;
                                        updateDailyCollection.CurrentBalanceAmount = dailyCollection.FirstOrDefault().CollectibleAmount + dailCollectionPrevoiusDate.FirstOrDefault().CurrentBalanceAmount;
                                        updateDailyCollection.PenaltyAmount = 0;
                                        updateDailyCollection.IsCurrentCollection = true;
                                        updateDailyCollection.IsCleared = false;
                                        updateDailyCollection.IsAbsent = false;
                                        updateDailyCollection.IsPartiallyPaid = false;
                                        updateDailyCollection.IsPaidInAdvanced = false;
                                        updateDailyCollection.IsFullyPaid = false;
                                        updateDailyCollection.IsProcessed = false;
                                        updateDailyCollection.CanPerformAction = true;
                                        db.SubmitChanges();

                                        var updateDailyCollectionPrevoiusDate = dailCollectionPrevoiusDate.FirstOrDefault();
                                        updateDailyCollectionPrevoiusDate.CanPerformAction = true;
                                        updateDailyCollectionPrevoiusDate.IsCurrentCollection = false;
                                        db.SubmitChanges();

                                        var dailyCollectionNextDate = from d in db.trnDailyCollections where d.CollectionId == dailyCollection.FirstOrDefault().CollectionId && d.DayNumber == (dailyCollection.FirstOrDefault().DayNumber + 1) select d;
                                        if (dailyCollectionNextDate.Any())
                                        {
                                            var updateDailyCollectionNextDate = dailyCollectionNextDate.FirstOrDefault();
                                            updateDailyCollectionNextDate.PreviousBalanceAmount = 0;
                                            updateDailyCollectionNextDate.CurrentBalanceAmount = dailyCollectionNextDate.FirstOrDefault().CollectibleAmount;
                                            updateDailyCollectionNextDate.CanPerformAction = false;
                                            updateDailyCollectionNextDate.IsCurrentCollection = false;
                                            db.SubmitChanges();
                                        }
                                    }
                                    else
                                    {
                                        var updateDailyCollection = dailyCollection.FirstOrDefault();
                                        updateDailyCollection.PaidAmount = 0;
                                        updateDailyCollection.PreviousBalanceAmount = 0;
                                        updateDailyCollection.CurrentBalanceAmount = dailyCollection.FirstOrDefault().CollectibleAmount;
                                        updateDailyCollection.PenaltyAmount = 0;
                                        updateDailyCollection.IsCurrentCollection = true;
                                        updateDailyCollection.IsCleared = false;
                                        updateDailyCollection.IsAbsent = false;
                                        updateDailyCollection.IsPartiallyPaid = false;
                                        updateDailyCollection.IsPaidInAdvanced = false;
                                        updateDailyCollection.IsFullyPaid = false;
                                        updateDailyCollection.IsProcessed = false;
                                        updateDailyCollection.CanPerformAction = true;
                                        db.SubmitChanges();

                                        var dailyCollectionNextDate = from d in db.trnDailyCollections where d.CollectionId == dailyCollection.FirstOrDefault().CollectionId && d.DayNumber == (dailyCollection.FirstOrDefault().DayNumber + 1) select d;
                                        if (dailyCollectionNextDate.Any())
                                        {
                                            var updateDailyCollectionNextDate = dailyCollectionNextDate.FirstOrDefault();
                                            updateDailyCollectionNextDate.PreviousBalanceAmount = 0;
                                            updateDailyCollectionNextDate.CurrentBalanceAmount = dailyCollectionNextDate.FirstOrDefault().CollectibleAmount;
                                            updateDailyCollectionNextDate.CanPerformAction = false;
                                            updateDailyCollectionNextDate.IsCurrentCollection = false;
                                            db.SubmitChanges();
                                        }
                                    }

                                    var collection = from d in db.trnCollections where d.Id == dailyCollection.FirstOrDefault().CollectionId select d;
                                    if (collection.Any())
                                    {
                                        var updateCollectionIsFullyPaid = collection.FirstOrDefault();
                                        updateCollectionIsFullyPaid.IsFullyPaid = false;
                                        db.SubmitChanges();
                                    }

                                    Business.Journal journal = new Business.Journal();
                                    journal.deleteCollectionJournal(Convert.ToInt32(id));

                                    return Request.CreateResponse(HttpStatusCode.OK);
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Can't process.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "No rights to perform some actions.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "No rights to access this page.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "The loan appliction must be locked before proceeding the collection.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No data found in the server.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something's went wrong from the server.");
            }
        }

        // partial payment collection
        [Authorize]
        [HttpPut]
        [Route("api/dailyCollection/partialPayment/byId/{id}")]
        public HttpResponseMessage partialPaymentDailyCollection(String id, Models.TrnDailyCollection dailyCollection)
        {
            try
            {
                var dailyCollections = from d in db.trnDailyCollections where d.Id == Convert.ToInt32(id) select d;
                if (dailyCollections.Any())
                {
                    if (dailyCollections.FirstOrDefault().trnCollection.trnLoanApplication.IsLocked)
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
                            String matchPageString = "CollectionDetail";
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
                                var partialPaidAmount = dailyCollection.PaidAmount;
                                if (dailyCollections.FirstOrDefault().IsCurrentCollection)
                                {
                                    if (!dailyCollections.FirstOrDefault().IsLastDay)
                                    {
                                        if (dailyCollections.FirstOrDefault().CurrentBalanceAmount > 0)
                                        {
                                            if (!dailyCollections.FirstOrDefault().IsCleared && !dailyCollections.FirstOrDefault().IsAbsent)
                                            {
                                                if (dailyCollections.FirstOrDefault().CanPerformAction)
                                                {
                                                    if (partialPaidAmount > 0)
                                                    {
                                                        if (dailyCollections.FirstOrDefault().CurrentBalanceAmount >= partialPaidAmount)
                                                        {
                                                            var isClearedValue = false;
                                                            var IsPartiallyPaidValue = true;
                                                            if (dailyCollections.FirstOrDefault().CurrentBalanceAmount - partialPaidAmount == 0)
                                                            {
                                                                isClearedValue = true;
                                                                IsPartiallyPaidValue = false;
                                                            }

                                                            var updateDailyCollection = dailyCollections.FirstOrDefault();
                                                            updateDailyCollection.PaidAmount = partialPaidAmount;
                                                            updateDailyCollection.CurrentBalanceAmount = (dailyCollections.FirstOrDefault().CollectibleAmount + dailyCollections.FirstOrDefault().PreviousBalanceAmount) - partialPaidAmount;
                                                            updateDailyCollection.IsCurrentCollection = false;
                                                            updateDailyCollection.IsCleared = isClearedValue;
                                                            updateDailyCollection.IsAbsent = false;
                                                            updateDailyCollection.IsPartiallyPaid = IsPartiallyPaidValue;
                                                            updateDailyCollection.IsPaidInAdvanced = false;
                                                            updateDailyCollection.IsFullyPaid = false;
                                                            updateDailyCollection.IsProcessed = true;
                                                            db.SubmitChanges();

                                                            var dailyCollectionPrevoiusDate = from d in db.trnDailyCollections where d.CollectionId == Convert.ToInt32(dailyCollections.FirstOrDefault().CollectionId) && d.DayNumber == (dailyCollections.FirstOrDefault().DayNumber - 1) select d;
                                                            if (dailyCollectionPrevoiusDate.Any())
                                                            {
                                                                var updateDailyCollectionPrevoiusDate = dailyCollectionPrevoiusDate.FirstOrDefault();
                                                                updateDailyCollectionPrevoiusDate.CanPerformAction = false;
                                                                updateDailyCollectionPrevoiusDate.IsCurrentCollection = false;
                                                                db.SubmitChanges();

                                                                var dailyCollectionNextDate = from d in db.trnDailyCollections where d.CollectionId == Convert.ToInt32(dailyCollections.FirstOrDefault().CollectionId) && d.DayNumber == (dailyCollections.FirstOrDefault().DayNumber + 1) select d;
                                                                if (dailyCollectionNextDate.Any())
                                                                {
                                                                    var updateDailyCollectionNextDate = dailyCollectionNextDate.FirstOrDefault();
                                                                    updateDailyCollectionNextDate.PreviousBalanceAmount = (dailyCollections.FirstOrDefault().CollectibleAmount + dailyCollections.FirstOrDefault().PreviousBalanceAmount) - partialPaidAmount;
                                                                    updateDailyCollectionNextDate.CurrentBalanceAmount = dailyCollectionNextDate.FirstOrDefault().CollectibleAmount + ((dailyCollections.FirstOrDefault().CollectibleAmount + dailyCollections.FirstOrDefault().PreviousBalanceAmount) - partialPaidAmount);
                                                                    updateDailyCollectionNextDate.CanPerformAction = true;
                                                                    updateDailyCollectionNextDate.IsCurrentCollection = true;
                                                                    db.SubmitChanges();

                                                                    if (updateDailyCollectionNextDate.CurrentBalanceAmount == 0)
                                                                    {
                                                                        var collection = from d in db.trnCollections where d.Id == dailyCollections.FirstOrDefault().CollectionId select d;
                                                                        if (collection.Any())
                                                                        {
                                                                            var updateCollectionIsFullyPaid = collection.FirstOrDefault();
                                                                            updateCollectionIsFullyPaid.IsFullyPaid = true;
                                                                            db.SubmitChanges();
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var dailyCollectionNextDate = from d in db.trnDailyCollections where d.CollectionId == Convert.ToInt32(dailyCollections.FirstOrDefault().CollectionId) && d.DayNumber == (dailyCollections.FirstOrDefault().DayNumber + 1) select d;
                                                                if (dailyCollectionNextDate.Any())
                                                                {
                                                                    var updateDailyCollectionNextDate = dailyCollectionNextDate.FirstOrDefault();
                                                                    updateDailyCollectionNextDate.PreviousBalanceAmount = (dailyCollections.FirstOrDefault().CollectibleAmount + dailyCollections.FirstOrDefault().PreviousBalanceAmount) - partialPaidAmount;
                                                                    updateDailyCollectionNextDate.CurrentBalanceAmount = dailyCollectionNextDate.FirstOrDefault().CollectibleAmount + ((dailyCollections.FirstOrDefault().CollectibleAmount + dailyCollections.FirstOrDefault().PreviousBalanceAmount) - partialPaidAmount);
                                                                    updateDailyCollectionNextDate.CanPerformAction = true;
                                                                    updateDailyCollectionNextDate.IsCurrentCollection = true;
                                                                    db.SubmitChanges();

                                                                    if (updateDailyCollectionNextDate.CurrentBalanceAmount == 0)
                                                                    {
                                                                        var collection = from d in db.trnCollections where d.Id == dailyCollections.FirstOrDefault().CollectionId select d;
                                                                        if (collection.Any())
                                                                        {
                                                                            var updateCollectionIsFullyPaid = collection.FirstOrDefault();
                                                                            updateCollectionIsFullyPaid.IsFullyPaid = true;
                                                                            db.SubmitChanges();
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            Business.Journal journal = new Business.Journal();
                                                            journal.postCollectionJournal(dailyCollections.FirstOrDefault().Id);

                                                            return Request.CreateResponse(HttpStatusCode.OK);
                                                        }
                                                        else
                                                        {
                                                            return Request.CreateResponse(HttpStatusCode.BadRequest, "The amount must not be greater than the current balance amount.");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Zero(0) amount. Please enter an amount.");
                                                    }
                                                }
                                                else
                                                {
                                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Can't process.");
                                                }
                                            }
                                            else
                                            {
                                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Already proceesed.");
                                            }
                                        }
                                        else
                                        {
                                            return Request.CreateResponse(HttpStatusCode.BadRequest, "No current balance.");
                                        }
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Last Day. please reconstruct.");
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Not a current collection.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "No rights to perform some actions.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "No rights to access this page.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "The loan appliction must be locked before proceeding the collection.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No data found in the server.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something's went wrong from the server.");
            }
        }

        // advance payment collection
        [Authorize]
        [HttpPut]
        [Route("api/dailyCollection/advancePayment/byId/{id}")]
        public HttpResponseMessage advancePaymentDailyCollection(String id)
        {
            try
            {
                var dailyCollection = from d in db.trnDailyCollections where d.Id == Convert.ToInt32(id) select d;
                if (dailyCollection.Any())
                {
                    if (dailyCollection.FirstOrDefault().trnCollection.trnLoanApplication.IsLocked)
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
                            String matchPageString = "CollectionDetail";
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
                                var advancePaymentDailyCollections = from d in db.trnDailyCollections
                                                                     where d.IsProcessed == false
                                                                     && d.CollectionId == dailyCollection.FirstOrDefault().CollectionId
                                                                     && d.DayNumber <= dailyCollection.FirstOrDefault().DayNumber
                                                                     select new Models.TrnDailyCollection
                                                                     {
                                                                         Id = d.Id,
                                                                         DayNumber = d.DayNumber,
                                                                         DailyCollectionDate = d.DailyCollectionDate.ToShortDateString(),
                                                                         CollectibleAmount = d.CollectibleAmount,
                                                                         PreviousBalanceAmount = d.PreviousBalanceAmount,
                                                                         CurrentBalanceAmount = d.CurrentBalanceAmount
                                                                     };

                                if (advancePaymentDailyCollections.Any())
                                {
                                    foreach (var advancePaymentDailyCollection in advancePaymentDailyCollections)
                                    {
                                        var dailyCollectionForUpdate = from d in db.trnDailyCollections where d.Id == advancePaymentDailyCollection.Id select d;
                                        if (dailyCollectionForUpdate.Any())
                                        {
                                            var updateDailyCollection = dailyCollectionForUpdate.FirstOrDefault();
                                            updateDailyCollection.PaidAmount = advancePaymentDailyCollection.CollectibleAmount + advancePaymentDailyCollection.PreviousBalanceAmount;
                                            updateDailyCollection.PreviousBalanceAmount = 0;
                                            updateDailyCollection.CurrentBalanceAmount = 0;
                                            updateDailyCollection.PenaltyAmount = 0;
                                            updateDailyCollection.IsCurrentCollection = false;
                                            updateDailyCollection.IsCleared = true;
                                            updateDailyCollection.IsAbsent = false;
                                            updateDailyCollection.IsPartiallyPaid = false;
                                            updateDailyCollection.IsPaidInAdvanced = true;
                                            updateDailyCollection.IsFullyPaid = false;
                                            updateDailyCollection.IsProcessed = true;
                                            db.SubmitChanges();

                                            var dailyCollectionPrevoiusDate = from d in db.trnDailyCollections where d.CollectionId == dailyCollection.FirstOrDefault().CollectionId && d.DayNumber == Convert.ToDecimal(advancePaymentDailyCollection.DayNumber - 1) select d;
                                            if (dailyCollectionPrevoiusDate.Any())
                                            {
                                                var updateDailyCollectionPrevoiusDate = dailyCollectionPrevoiusDate.FirstOrDefault();
                                                updateDailyCollectionPrevoiusDate.CanPerformAction = false;
                                                updateDailyCollectionPrevoiusDate.IsCurrentCollection = false;
                                                db.SubmitChanges();

                                                var dailyCollectionNextDate = from d in db.trnDailyCollections where d.CollectionId == dailyCollection.FirstOrDefault().CollectionId && d.DayNumber == Convert.ToDecimal(advancePaymentDailyCollection.DayNumber + 1) select d;
                                                if (dailyCollectionNextDate.Any())
                                                {
                                                    var updateDailyCollectionNextDate = dailyCollectionNextDate.FirstOrDefault();
                                                    updateDailyCollectionNextDate.CanPerformAction = true;
                                                    updateDailyCollectionNextDate.IsCurrentCollection = true;
                                                    db.SubmitChanges();

                                                    if (updateDailyCollectionNextDate.CurrentBalanceAmount == 0)
                                                    {
                                                        var collection = from d in db.trnCollections where d.Id == dailyCollection.FirstOrDefault().CollectionId select d;
                                                        if (collection.Any())
                                                        {
                                                            var updateCollectionIsFullyPaid = collection.FirstOrDefault();
                                                            updateCollectionIsFullyPaid.IsFullyPaid = true;
                                                            db.SubmitChanges();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var collection = from d in db.trnCollections where d.Id == dailyCollection.FirstOrDefault().CollectionId select d;
                                                    if (collection.Any())
                                                    {
                                                        var updateCollectionIsFullyPaid = collection.FirstOrDefault();
                                                        updateCollectionIsFullyPaid.IsFullyPaid = true;
                                                        db.SubmitChanges();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var dailyCollectionNextDate = from d in db.trnDailyCollections where d.CollectionId == dailyCollection.FirstOrDefault().CollectionId && d.DayNumber == Convert.ToDecimal(advancePaymentDailyCollection.DayNumber + 1) select d;
                                                if (dailyCollectionNextDate.Any())
                                                {
                                                    var updateDailyCollectionNextDate = dailyCollectionNextDate.FirstOrDefault();
                                                    updateDailyCollectionNextDate.CanPerformAction = true;
                                                    updateDailyCollectionNextDate.IsCurrentCollection = true;
                                                    db.SubmitChanges();

                                                    if (updateDailyCollectionNextDate.CurrentBalanceAmount == 0)
                                                    {
                                                        var collection = from d in db.trnCollections where d.Id == dailyCollection.FirstOrDefault().CollectionId select d;
                                                        if (collection.Any())
                                                        {
                                                            var updateCollectionIsFullyPaid = collection.FirstOrDefault();
                                                            updateCollectionIsFullyPaid.IsFullyPaid = true;
                                                            db.SubmitChanges();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var collection = from d in db.trnCollections where d.Id == dailyCollection.FirstOrDefault().CollectionId select d;
                                                    if (collection.Any())
                                                    {
                                                        var updateCollectionIsFullyPaid = collection.FirstOrDefault();
                                                        updateCollectionIsFullyPaid.IsFullyPaid = true;
                                                        db.SubmitChanges();
                                                    }
                                                }
                                            }
                                        }

                                        Business.Journal journal = new Business.Journal();
                                        journal.postCollectionJournal(advancePaymentDailyCollection.Id);
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
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "No rights to perform some actions.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "No rights to access this page.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "The loan appliction must be locked before proceeding the collection.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No data found in the server.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something's went wrong from the server.");
            }
        }

        // full payment collection
        [Authorize]
        [HttpPut]
        [Route("api/collection/fullPayment/byCollectionId/{collectionId}")]
        public HttpResponseMessage fullPaymentDailyCollection(String collectionId)
        {
            try
            {
                var dailyCollections = from d in db.trnDailyCollections
                                       where d.CollectionId == Convert.ToInt32(collectionId)
                                       select d;

                if (dailyCollections.Any())
                {
                    if (dailyCollections.FirstOrDefault().trnCollection.trnLoanApplication.IsLocked)
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
                            String matchPageString = "CollectionDetail";
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
                                var fullPaymentCollections = from d in db.trnDailyCollections
                                                             where d.CollectionId == Convert.ToInt32(collectionId)
                                                             && d.IsProcessed == false
                                                             && d.CurrentBalanceAmount > 0
                                                             select new Models.TrnDailyCollection
                                                             {
                                                                 Id = d.Id,
                                                                 DayNumber = d.DayNumber,
                                                                 DailyCollectionDate = d.DailyCollectionDate.ToShortDateString(),
                                                                 CollectibleAmount = d.CollectibleAmount,
                                                                 PreviousBalanceAmount = d.PreviousBalanceAmount,
                                                                 CurrentBalanceAmount = d.CurrentBalanceAmount
                                                             };

                                if (fullPaymentCollections.Any())
                                {
                                    foreach (var fullPaymentCollection in fullPaymentCollections)
                                    {
                                        var dailyCollectionForUpdate = from d in db.trnDailyCollections where d.Id == fullPaymentCollection.Id select d;
                                        if (dailyCollectionForUpdate.Any())
                                        {
                                            var updateDailyCollection = dailyCollectionForUpdate.FirstOrDefault();
                                            updateDailyCollection.PaidAmount = fullPaymentCollection.CollectibleAmount + fullPaymentCollection.PreviousBalanceAmount;
                                            updateDailyCollection.PreviousBalanceAmount = 0;
                                            updateDailyCollection.CurrentBalanceAmount = 0;
                                            updateDailyCollection.PenaltyAmount = 0;
                                            updateDailyCollection.IsCurrentCollection = false;
                                            updateDailyCollection.IsCleared = true;
                                            updateDailyCollection.IsAbsent = false;
                                            updateDailyCollection.IsPartiallyPaid = false;
                                            updateDailyCollection.IsPaidInAdvanced = false;
                                            updateDailyCollection.IsFullyPaid = true;
                                            updateDailyCollection.IsProcessed = true;
                                            db.SubmitChanges();

                                            var dailyCollectionPrevoiusDate = from d in db.trnDailyCollections where d.CollectionId == Convert.ToInt32(collectionId) && d.DayNumber == Convert.ToDecimal(fullPaymentCollection.DayNumber - 1) select d;
                                            if (dailyCollectionPrevoiusDate.Any())
                                            {
                                                var updateDailyCollectionPrevoiusDate = dailyCollectionPrevoiusDate.FirstOrDefault();
                                                updateDailyCollectionPrevoiusDate.CanPerformAction = false;
                                                updateDailyCollectionPrevoiusDate.IsCurrentCollection = false;
                                                db.SubmitChanges();

                                                var dailyCollectionNextDate = from d in db.trnDailyCollections where d.CollectionId == Convert.ToInt32(collectionId) && d.DayNumber == Convert.ToDecimal(fullPaymentCollection.DayNumber + 1) select d;
                                                if (dailyCollectionNextDate.Any())
                                                {
                                                    var updateDailyCollectionNextDate = dailyCollectionNextDate.FirstOrDefault();
                                                    updateDailyCollectionNextDate.CanPerformAction = true;
                                                    updateDailyCollectionNextDate.IsCurrentCollection = true;
                                                    db.SubmitChanges();
                                                }
                                            }
                                            else
                                            {
                                                var dailyCollectionNextDate = from d in db.trnDailyCollections where d.CollectionId == Convert.ToInt32(collectionId) && d.DayNumber == Convert.ToDecimal(fullPaymentCollection.DayNumber + 1) select d;
                                                if (dailyCollectionNextDate.Any())
                                                {
                                                    var updateDailyCollectionNextDate = dailyCollectionNextDate.FirstOrDefault();
                                                    updateDailyCollectionNextDate.CanPerformAction = true;
                                                    updateDailyCollectionNextDate.IsCurrentCollection = true;
                                                    db.SubmitChanges();
                                                }
                                            }
                                        }

                                        Business.Journal journal = new Business.Journal();
                                        journal.postCollectionJournal(fullPaymentCollection.Id);
                                    }

                                    var collection = from d in db.trnCollections where d.Id == dailyCollections.FirstOrDefault().CollectionId select d;
                                    if (collection.Any())
                                    {
                                        var updateCollectionIsFullyPaid = collection.FirstOrDefault();
                                        updateCollectionIsFullyPaid.IsFullyPaid = true;
                                        db.SubmitChanges();
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
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "No rights to perform some actions.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "No rights to access this page.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "The loan appliction must be locked before proceeding the collection.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No data found in the server to apply some actions.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something's went wrong from the server.");
            }
        }
    }
}
