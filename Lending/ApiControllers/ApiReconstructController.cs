using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiReconstructController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        //// collection reconstruct list
        //[Authorize]
        //[HttpGet]
        //[Route("api/collectionReconstruct/list/ByCollectionId/{collectionId}")]
        //public List<Models.TrnReconstruct> listCollectionReconstructByCollectionId(String collectionId)
        //{
        //    var collecionReconstruct = from d in db.trnCollectionReconstructs
        //                               where d.CollectionId == Convert.ToInt32(collectionId)
        //                               select new Models.TrnReconstruct
        //                               {
        //                                   Id = d.Id,
        //                                   CollectionId = d.CollectionId,
        //                                   ReconstructNumber = d.ReconstructNumber,
        //                                   StartDate = d.StartDate.ToShortDateString(),
        //                                   EndDate = d.EndDate.ToShortDateString(),
        //                                   TermId = d.TermId,
        //                                   Term = d.mstTerm.Term,
        //                                   TermNoOfDays = d.TermNoOfDays,
        //                                   TermNoOfAllowanceDays = d.TermNoOfAllowanceDays,
        //                                   InterestId = d.InterestId,
        //                                   InterestRate = d.InterestRate,
        //                                   InterestAmount = d.InterestAmount,
        //                                   PenaltyId = d.PenaltyId,
        //                                   Penalty = d.mstPenalty.Penalty,
        //                                   CurrentBalanceAmount = d.CurrentBalanceAmount,
        //                                   BalanceAmount = d.BalanceAmount
        //                               };

        //    return collecionReconstruct.ToList();
        //}

        //// zero fill
        //public String zeroFill(Int32 number, Int32 length)
        //{
        //    var result = number.ToString();
        //    var pad = length - result.Length;
        //    while (pad > 0)
        //    {
        //        result = "0" + result;
        //        pad--;
        //    }

        //    return result;
        //}

        //// add collection reconstruct
        //[Authorize]
        //[HttpPost]
        //[Route("api/collectionReconstruct/add")]
        //public HttpResponseMessage addCollectionReconstruct(Models.TrnReconstruct collectionReconstruct)
        //{
        //    try
        //    {
        //        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
        //        var mstUserForms = from d in db.mstUserForms
        //                           where d.UserId == userId
        //                           select new Models.MstUserForm
        //                           {
        //                               Id = d.Id,
        //                               Form = d.sysForm.Form,
        //                               CanPerformActions = d.CanPerformActions
        //                           };

        //        if (mstUserForms.Any())
        //        {
        //            String matchPageString = "CollectionDetail";
        //            Boolean canPerformActions = false;

        //            foreach (var mstUserForm in mstUserForms)
        //            {
        //                if (mstUserForm.Form.Equals(matchPageString))
        //                {
        //                    if (mstUserForm.CanPerformActions)
        //                    {
        //                        canPerformActions = true;
        //                    }

        //                    break;
        //                }
        //            }

        //            if (canPerformActions)
        //            {
        //                var lastCollectionData = from d in db.trnDailyCollections.OrderByDescending(d => d.Id)
        //                                         where d.CollectionId == collectionReconstruct.CollectionId
        //                                         select d;

        //                if (lastCollectionData.Any())
        //                {
        //                    if (lastCollectionData.FirstOrDefault().IsLastDay)
        //                    {
        //                        if (lastCollectionData.FirstOrDefault().IsProcessed)
        //                        {
        //                            String reconstructNumber = "0000000001";
        //                            var lastCollectionReconstruct = from d in db.trnCollectionReconstructs.OrderByDescending(d => d.Id) select d;
        //                            if (lastCollectionReconstruct.Any())
        //                            {
        //                                var newReconstructNumber = Convert.ToInt32(lastCollectionReconstruct.FirstOrDefault().ReconstructNumber) + 0000000001;
        //                                reconstructNumber = newReconstructNumber.ToString();
        //                            }

        //                            Data.trnCollectionReconstruct newCollectionReconstruct = new Data.trnCollectionReconstruct();
        //                            newCollectionReconstruct.CollectionId = collectionReconstruct.CollectionId;
        //                            newCollectionReconstruct.ReconstructNumber = zeroFill(Convert.ToInt32(reconstructNumber), 10);
        //                            newCollectionReconstruct.StartDate = Convert.ToDateTime(collectionReconstruct.StartDate);
        //                            newCollectionReconstruct.EndDate = Convert.ToDateTime(collectionReconstruct.EndDate);
        //                            newCollectionReconstruct.TermId = collectionReconstruct.TermId;
        //                            newCollectionReconstruct.TermNoOfDays = collectionReconstruct.TermNoOfDays;
        //                            newCollectionReconstruct.TermNoOfAllowanceDays = collectionReconstruct.TermNoOfAllowanceDays;
        //                            newCollectionReconstruct.InterestId = collectionReconstruct.InterestId;
        //                            newCollectionReconstruct.InterestRate = collectionReconstruct.InterestRate;
        //                            newCollectionReconstruct.InterestAmount = (lastCollectionData.FirstOrDefault().CurrentBalanceAmount / 100) * collectionReconstruct.InterestRate;
        //                            newCollectionReconstruct.PenaltyId = collectionReconstruct.PenaltyId;
        //                            newCollectionReconstruct.CurrentBalanceAmount = lastCollectionData.FirstOrDefault().CurrentBalanceAmount;
        //                            newCollectionReconstruct.BalanceAmount = collectionReconstruct.InterestAmount + lastCollectionData.FirstOrDefault().CurrentBalanceAmount;
        //                            db.trnCollectionReconstructs.InsertOnSubmit(newCollectionReconstruct);
        //                            db.SubmitChanges();

        //                            var collection = from d in db.trnCollections where d.Id == collectionReconstruct.CollectionId select d;
        //                            if (collection.Any())
        //                            {
        //                                var updateCollectionIsFullyPaid = collection.FirstOrDefault();
        //                                updateCollectionIsFullyPaid.IsOverdue = true;
        //                                db.SubmitChanges();
        //                            }

        //                            if (newCollectionReconstruct.BalanceAmount > 0)
        //                            {
        //                                var numberOfDays = (Convert.ToDateTime(newCollectionReconstruct.EndDate) - Convert.ToDateTime(newCollectionReconstruct.StartDate)).TotalDays;

        //                                Decimal remainingBalanceAmount = 0;
        //                                Decimal collectibleAmount = Math.Round(newCollectionReconstruct.BalanceAmount / Convert.ToDecimal(numberOfDays), 1);
        //                                Decimal collectibleAmountCeil = Math.Ceiling((collectibleAmount + 1) / 5) * 5;
        //                                Decimal termNoOfAllowanceDay = newCollectionReconstruct.TermNoOfAllowanceDays;

        //                                Decimal dayNumber = lastCollectionData.FirstOrDefault().DayNumber;
        //                                for (var i = 1; i <= numberOfDays + Convert.ToInt32(termNoOfAllowanceDay); i++)
        //                                {
        //                                    Boolean isCurrentCollectionValue = false;
        //                                    Boolean canPerformAction = false;

        //                                    if (i == newCollectionReconstruct.TermNoOfDays)
        //                                    {
        //                                        remainingBalanceAmount = newCollectionReconstruct.BalanceAmount;
        //                                        isCurrentCollectionValue = true;
        //                                        canPerformAction = true;
        //                                    }
        //                                    else
        //                                    {
        //                                        remainingBalanceAmount = 0;
        //                                        canPerformAction = false;
        //                                    }

        //                                    if (i <= numberOfDays)
        //                                    {
        //                                        if (i % newCollectionReconstruct.TermNoOfDays == 0)
        //                                        {
        //                                            Boolean isDueDateValue = false, isLastDay = false;

        //                                            if (i == numberOfDays)
        //                                            {
        //                                                isDueDateValue = true;
        //                                                if (numberOfDays + Convert.ToInt32(termNoOfAllowanceDay) == i)
        //                                                {
        //                                                    isLastDay = true;
        //                                                }
        //                                            }

        //                                            dayNumber += 1;

        //                                            Data.trnDailyCollection newDailyCollection = new Data.trnDailyCollection();
        //                                            newDailyCollection.CollectionId = newCollectionReconstruct.CollectionId;
        //                                            newDailyCollection.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
        //                                            newDailyCollection.DayNumber = dayNumber;
        //                                            newDailyCollection.DailyCollectionDate = Convert.ToDateTime(newCollectionReconstruct.StartDate).Date.AddDays(i);
        //                                            newDailyCollection.NetAmount = newCollectionReconstruct.BalanceAmount;
        //                                            newDailyCollection.CollectibleAmount = 0;
        //                                            newDailyCollection.PenaltyAmount = 0;
        //                                            newDailyCollection.PaidAmount = 0;
        //                                            newDailyCollection.PreviousBalanceAmount = remainingBalanceAmount;
        //                                            newDailyCollection.CurrentBalanceAmount = remainingBalanceAmount;
        //                                            newDailyCollection.IsCurrentCollection = isCurrentCollectionValue;
        //                                            newDailyCollection.IsCleared = false;
        //                                            newDailyCollection.IsAbsent = false;
        //                                            newDailyCollection.IsPartiallyPaid = false;
        //                                            newDailyCollection.IsPaidInAdvanced = false;
        //                                            newDailyCollection.IsFullyPaid = false;
        //                                            newDailyCollection.IsProcessed = false;
        //                                            newDailyCollection.CanPerformAction = canPerformAction;
        //                                            newDailyCollection.IsAllowanceDay = false;
        //                                            newDailyCollection.IsDueDate = isDueDateValue;
        //                                            newDailyCollection.IsLastDay = isLastDay;
        //                                            newDailyCollection.ReconstructId = newCollectionReconstruct.Id;
        //                                            newDailyCollection.IsReconstructed = true;
        //                                            db.trnDailyCollections.InsertOnSubmit(newDailyCollection);
        //                                            db.SubmitChanges();
        //                                        }
        //                                        else
        //                                        {
        //                                            if (i == numberOfDays)
        //                                            {
        //                                                Boolean isDueDateValue = false, isLastDay = false;

        //                                                if (i == numberOfDays)
        //                                                {
        //                                                    isDueDateValue = true;
        //                                                    if (numberOfDays + Convert.ToInt32(termNoOfAllowanceDay) == i)
        //                                                    {
        //                                                        isLastDay = true;
        //                                                    }
        //                                                }

        //                                                dayNumber += 1;

        //                                                Data.trnDailyCollection newDailyCollection = new Data.trnDailyCollection();
        //                                                newDailyCollection.CollectionId = newCollectionReconstruct.CollectionId;
        //                                                newDailyCollection.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
        //                                                newDailyCollection.DayNumber = dayNumber;
        //                                                newDailyCollection.DailyCollectionDate = Convert.ToDateTime(newCollectionReconstruct.StartDate).Date.AddDays(i);
        //                                                newDailyCollection.NetAmount = newCollectionReconstruct.BalanceAmount;
        //                                                newDailyCollection.CollectibleAmount = 0;
        //                                                newDailyCollection.PenaltyAmount = 0;
        //                                                newDailyCollection.PaidAmount = 0;
        //                                                newDailyCollection.PreviousBalanceAmount = 0;
        //                                                newDailyCollection.CurrentBalanceAmount = 0;
        //                                                newDailyCollection.IsCurrentCollection = isCurrentCollectionValue;
        //                                                newDailyCollection.IsCleared = false;
        //                                                newDailyCollection.IsAbsent = false;
        //                                                newDailyCollection.IsPartiallyPaid = false;
        //                                                newDailyCollection.IsPaidInAdvanced = false;
        //                                                newDailyCollection.IsFullyPaid = false;
        //                                                newDailyCollection.IsProcessed = false;
        //                                                newDailyCollection.CanPerformAction = false;
        //                                                newDailyCollection.IsAllowanceDay = false;
        //                                                newDailyCollection.IsDueDate = isDueDateValue;
        //                                                newDailyCollection.IsLastDay = isLastDay;
        //                                                newDailyCollection.ReconstructId = newCollectionReconstruct.Id;
        //                                                newDailyCollection.IsReconstructed = true;
        //                                                db.trnDailyCollections.InsertOnSubmit(newDailyCollection);
        //                                                db.SubmitChanges();
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        Boolean isLastDay = false;

        //                                        if (numberOfDays + Convert.ToInt32(termNoOfAllowanceDay) == i)
        //                                        {
        //                                            isLastDay = true;
        //                                        }

        //                                        dayNumber += 1;

        //                                        Data.trnDailyCollection newDailyCollection = new Data.trnDailyCollection();
        //                                        newDailyCollection.CollectionId = newCollectionReconstruct.CollectionId;
        //                                        newDailyCollection.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
        //                                        newDailyCollection.DayNumber = dayNumber;
        //                                        newDailyCollection.DailyCollectionDate = Convert.ToDateTime(newCollectionReconstruct.StartDate).Date.AddDays(i);
        //                                        newDailyCollection.NetAmount = newCollectionReconstruct.BalanceAmount;
        //                                        newDailyCollection.CollectibleAmount = 0;
        //                                        newDailyCollection.PenaltyAmount = 0;
        //                                        newDailyCollection.PaidAmount = 0;
        //                                        newDailyCollection.PreviousBalanceAmount = 0;
        //                                        newDailyCollection.CurrentBalanceAmount = 0;
        //                                        newDailyCollection.IsCurrentCollection = isCurrentCollectionValue;
        //                                        newDailyCollection.IsCleared = false;
        //                                        newDailyCollection.IsAbsent = false;
        //                                        newDailyCollection.IsPartiallyPaid = false;
        //                                        newDailyCollection.IsPaidInAdvanced = false;
        //                                        newDailyCollection.IsFullyPaid = false;
        //                                        newDailyCollection.IsProcessed = false;
        //                                        newDailyCollection.CanPerformAction = false;
        //                                        newDailyCollection.IsAllowanceDay = true;
        //                                        newDailyCollection.IsDueDate = false;
        //                                        newDailyCollection.IsLastDay = isLastDay;
        //                                        newDailyCollection.ReconstructId = newCollectionReconstruct.Id;
        //                                        newDailyCollection.IsReconstructed = true;
        //                                        db.trnDailyCollections.InsertOnSubmit(newDailyCollection);
        //                                        db.SubmitChanges();
        //                                    }
        //                                }
        //                            }

        //                            return Request.CreateResponse(HttpStatusCode.OK);
        //                        }
        //                        else
        //                        {
        //                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Not yet processed.");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Not yet on last day.");
        //                    }
        //                }
        //                else
        //                {
        //                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No Data found on the server.");
        //                }
        //            }
        //            else
        //            {
        //                return Request.CreateResponse(HttpStatusCode.BadRequest, "No Rights");
        //            }
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest);
        //        }
        //    }
        //    catch
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }
        //}

        //// update collection reconstruct
        //[Authorize]
        //[HttpPut]
        //[Route("api/collectionReconstruct/update/{id}")]
        //public HttpResponseMessage updateCollectionReconstruct(String id, Models.TrnReconstruct collectionReconstruct)
        //{
        //    try
        //    {
        //        var collectionReconstructs = from d in db.trnCollectionReconstructs where d.Id == Convert.ToInt32(id) select d;
        //        if (collectionReconstructs.Any())
        //        {
        //            var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
        //            var mstUserForms = from d in db.mstUserForms
        //                               where d.UserId == userId
        //                               select new Models.MstUserForm
        //                               {
        //                                   Id = d.Id,
        //                                   Form = d.sysForm.Form,
        //                                   CanPerformActions = d.CanPerformActions
        //                               };

        //            if (mstUserForms.Any())
        //            {
        //                String matchPageString = "CollectionDetail";
        //                Boolean canPerformActions = false;

        //                foreach (var mstUserForm in mstUserForms)
        //                {
        //                    if (mstUserForm.Form.Equals(matchPageString))
        //                    {
        //                        if (mstUserForm.CanPerformActions)
        //                        {
        //                            canPerformActions = true;
        //                        }

        //                        break;
        //                    }
        //                }

        //                if (canPerformActions)
        //                {
        //                    var lastCollectionData = from d in db.trnDailyCollections.OrderByDescending(d => d.Id)
        //                                             where d.CollectionId == collectionReconstruct.CollectionId
        //                                             select d;

        //                    if (lastCollectionData.Any())
        //                    {
        //                        if (lastCollectionData.FirstOrDefault().IsLastDay)
        //                        {
        //                            if (lastCollectionData.FirstOrDefault().IsProcessed)
        //                            {
        //                                var lastCollectionReconstruct = from d in db.trnCollectionReconstructs.OrderByDescending(d => d.Id)
        //                                                                where d.CollectionId == collectionReconstructs.FirstOrDefault().CollectionId
        //                                                                select d;

        //                                if (lastCollectionReconstruct.Any())
        //                                {
        //                                    if (lastCollectionReconstruct.FirstOrDefault().Id == Convert.ToInt32(id))
        //                                    {
        //                                        db.trnCollectionReconstructs.DeleteOnSubmit(collectionReconstructs.First());
        //                                        db.SubmitChanges();

        //                                        var updateCollectionReconstruct = collectionReconstructs.FirstOrDefault();
        //                                        updateCollectionReconstruct.StartDate = Convert.ToDateTime(collectionReconstruct.StartDate);
        //                                        updateCollectionReconstruct.EndDate = Convert.ToDateTime(collectionReconstruct.EndDate);
        //                                        updateCollectionReconstruct.TermId = collectionReconstruct.TermId;
        //                                        updateCollectionReconstruct.TermNoOfDays = collectionReconstruct.TermNoOfDays;
        //                                        updateCollectionReconstruct.TermNoOfAllowanceDays = collectionReconstruct.TermNoOfAllowanceDays;
        //                                        updateCollectionReconstruct.InterestId = collectionReconstruct.InterestId;
        //                                        updateCollectionReconstruct.InterestRate = collectionReconstruct.InterestRate;
        //                                        updateCollectionReconstruct.InterestAmount = (lastCollectionData.FirstOrDefault().CurrentBalanceAmount / 100) * collectionReconstruct.InterestRate;
        //                                        updateCollectionReconstruct.PenaltyId = collectionReconstruct.PenaltyId;
        //                                        updateCollectionReconstruct.CurrentBalanceAmount = lastCollectionData.FirstOrDefault().CurrentBalanceAmount;
        //                                        updateCollectionReconstruct.BalanceAmount = collectionReconstruct.InterestAmount + lastCollectionData.FirstOrDefault().CurrentBalanceAmount;
        //                                        db.SubmitChanges();

        //                                        var collection = from d in db.trnCollections where d.Id == collectionReconstruct.CollectionId select d;
        //                                        if (collection.Any())
        //                                        {
        //                                            var updateCollectionIsFullyPaid = collection.FirstOrDefault();
        //                                            updateCollectionIsFullyPaid.IsOverdue = true;
        //                                            db.SubmitChanges();
        //                                        }

        //                                        if (updateCollectionReconstruct.BalanceAmount > 0)
        //                                        {
        //                                            var numberOfDays = (Convert.ToDateTime(updateCollectionReconstruct.EndDate) - Convert.ToDateTime(updateCollectionReconstruct.StartDate)).TotalDays;

        //                                            Decimal remainingBalanceAmount = 0;
        //                                            Decimal collectibleAmount = Math.Round(updateCollectionReconstruct.BalanceAmount / Convert.ToDecimal(numberOfDays), 1);
        //                                            Decimal collectibleAmountCeil = Math.Ceiling((collectibleAmount + 1) / 5) * 5;
        //                                            Decimal termNoOfAllowanceDay = updateCollectionReconstruct.TermNoOfAllowanceDays;

        //                                            Decimal dayNumber = lastCollectionData.FirstOrDefault().DayNumber;
        //                                            for (var i = 1; i <= numberOfDays + Convert.ToInt32(termNoOfAllowanceDay); i++)
        //                                            {
        //                                                Boolean isCurrentCollectionValue = false;
        //                                                Boolean canPerformAction = false;

        //                                                if (i == updateCollectionReconstruct.TermNoOfDays)
        //                                                {
        //                                                    remainingBalanceAmount = updateCollectionReconstruct.BalanceAmount;
        //                                                    isCurrentCollectionValue = true;
        //                                                    canPerformAction = true;
        //                                                }
        //                                                else
        //                                                {
        //                                                    remainingBalanceAmount = 0;
        //                                                    canPerformAction = false;
        //                                                }

        //                                                if (i <= numberOfDays)
        //                                                {
        //                                                    if (i % updateCollectionReconstruct.TermNoOfDays == 0)
        //                                                    {
        //                                                        Boolean isDueDateValue = false, isLastDay = false;

        //                                                        if (i == numberOfDays)
        //                                                        {
        //                                                            isDueDateValue = true;
        //                                                            if (numberOfDays + Convert.ToInt32(termNoOfAllowanceDay) == i)
        //                                                            {
        //                                                                isLastDay = true;
        //                                                            }
        //                                                        }

        //                                                        dayNumber += 1;

        //                                                        Data.trnDailyCollection newDailyCollection = new Data.trnDailyCollection();
        //                                                        newDailyCollection.CollectionId = updateCollectionReconstruct.CollectionId;
        //                                                        newDailyCollection.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
        //                                                        newDailyCollection.DayNumber = dayNumber;
        //                                                        newDailyCollection.DailyCollectionDate = Convert.ToDateTime(updateCollectionReconstruct.StartDate).Date.AddDays(i);
        //                                                        newDailyCollection.NetAmount = updateCollectionReconstruct.BalanceAmount;
        //                                                        newDailyCollection.CollectibleAmount = 0;
        //                                                        newDailyCollection.PenaltyAmount = 0;
        //                                                        newDailyCollection.PaidAmount = 0;
        //                                                        newDailyCollection.PreviousBalanceAmount = remainingBalanceAmount;
        //                                                        newDailyCollection.CurrentBalanceAmount = remainingBalanceAmount;
        //                                                        newDailyCollection.IsCurrentCollection = isCurrentCollectionValue;
        //                                                        newDailyCollection.IsCleared = false;
        //                                                        newDailyCollection.IsAbsent = false;
        //                                                        newDailyCollection.IsPartiallyPaid = false;
        //                                                        newDailyCollection.IsPaidInAdvanced = false;
        //                                                        newDailyCollection.IsFullyPaid = false;
        //                                                        newDailyCollection.IsProcessed = false;
        //                                                        newDailyCollection.CanPerformAction = canPerformAction;
        //                                                        newDailyCollection.IsAllowanceDay = false;
        //                                                        newDailyCollection.IsDueDate = isDueDateValue;
        //                                                        newDailyCollection.IsLastDay = isLastDay;
        //                                                        newDailyCollection.ReconstructId = updateCollectionReconstruct.Id;
        //                                                        newDailyCollection.IsReconstructed = true;
        //                                                        db.trnDailyCollections.InsertOnSubmit(newDailyCollection);
        //                                                        db.SubmitChanges();
        //                                                    }
        //                                                    else
        //                                                    {
        //                                                        if (i == numberOfDays)
        //                                                        {
        //                                                            Boolean isDueDateValue = false, isLastDay = false;

        //                                                            if (i == numberOfDays)
        //                                                            {
        //                                                                isDueDateValue = true;
        //                                                                if (numberOfDays + Convert.ToInt32(termNoOfAllowanceDay) == i)
        //                                                                {
        //                                                                    isLastDay = true;
        //                                                                }
        //                                                            }

        //                                                            dayNumber += 1;

        //                                                            Data.trnDailyCollection newDailyCollection = new Data.trnDailyCollection();
        //                                                            newDailyCollection.CollectionId = updateCollectionReconstruct.CollectionId;
        //                                                            newDailyCollection.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
        //                                                            newDailyCollection.DayNumber = dayNumber;
        //                                                            newDailyCollection.DailyCollectionDate = Convert.ToDateTime(updateCollectionReconstruct.StartDate).Date.AddDays(i);
        //                                                            newDailyCollection.NetAmount = updateCollectionReconstruct.BalanceAmount;
        //                                                            newDailyCollection.CollectibleAmount = 0;
        //                                                            newDailyCollection.PenaltyAmount = 0;
        //                                                            newDailyCollection.PaidAmount = 0;
        //                                                            newDailyCollection.PreviousBalanceAmount = 0;
        //                                                            newDailyCollection.CurrentBalanceAmount = 0;
        //                                                            newDailyCollection.IsCurrentCollection = isCurrentCollectionValue;
        //                                                            newDailyCollection.IsCleared = false;
        //                                                            newDailyCollection.IsAbsent = false;
        //                                                            newDailyCollection.IsPartiallyPaid = false;
        //                                                            newDailyCollection.IsPaidInAdvanced = false;
        //                                                            newDailyCollection.IsFullyPaid = false;
        //                                                            newDailyCollection.IsProcessed = false;
        //                                                            newDailyCollection.CanPerformAction = false;
        //                                                            newDailyCollection.IsAllowanceDay = false;
        //                                                            newDailyCollection.IsDueDate = isDueDateValue;
        //                                                            newDailyCollection.IsLastDay = isLastDay;
        //                                                            newDailyCollection.ReconstructId = updateCollectionReconstruct.Id;
        //                                                            newDailyCollection.IsReconstructed = true;
        //                                                            db.trnDailyCollections.InsertOnSubmit(newDailyCollection);
        //                                                            db.SubmitChanges();
        //                                                        }
        //                                                    }
        //                                                }
        //                                                else
        //                                                {
        //                                                    Boolean isLastDay = false;

        //                                                    if (numberOfDays + Convert.ToInt32(termNoOfAllowanceDay) == i)
        //                                                    {
        //                                                        isLastDay = true;
        //                                                    }

        //                                                    dayNumber += 1;

        //                                                    Data.trnDailyCollection newDailyCollection = new Data.trnDailyCollection();
        //                                                    newDailyCollection.CollectionId = updateCollectionReconstruct.CollectionId;
        //                                                    newDailyCollection.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
        //                                                    newDailyCollection.DayNumber = dayNumber;
        //                                                    newDailyCollection.DailyCollectionDate = Convert.ToDateTime(updateCollectionReconstruct.StartDate).Date.AddDays(i);
        //                                                    newDailyCollection.NetAmount = updateCollectionReconstruct.BalanceAmount;
        //                                                    newDailyCollection.CollectibleAmount = 0;
        //                                                    newDailyCollection.PenaltyAmount = 0;
        //                                                    newDailyCollection.PaidAmount = 0;
        //                                                    newDailyCollection.PreviousBalanceAmount = 0;
        //                                                    newDailyCollection.CurrentBalanceAmount = 0;
        //                                                    newDailyCollection.IsCurrentCollection = isCurrentCollectionValue;
        //                                                    newDailyCollection.IsCleared = false;
        //                                                    newDailyCollection.IsAbsent = false;
        //                                                    newDailyCollection.IsPartiallyPaid = false;
        //                                                    newDailyCollection.IsPaidInAdvanced = false;
        //                                                    newDailyCollection.IsFullyPaid = false;
        //                                                    newDailyCollection.IsProcessed = false;
        //                                                    newDailyCollection.CanPerformAction = false;
        //                                                    newDailyCollection.IsAllowanceDay = true;
        //                                                    newDailyCollection.IsDueDate = false;
        //                                                    newDailyCollection.IsLastDay = isLastDay;
        //                                                    newDailyCollection.ReconstructId = updateCollectionReconstruct.Id;
        //                                                    newDailyCollection.IsReconstructed = true;
        //                                                    db.trnDailyCollections.InsertOnSubmit(newDailyCollection);
        //                                                    db.SubmitChanges();
        //                                                }
        //                                            }
        //                                        }

        //                                        return Request.CreateResponse(HttpStatusCode.OK);
        //                                    }
        //                                    else
        //                                    {
        //                                        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    return Request.CreateResponse(HttpStatusCode.BadRequest);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Not yet processed.");
        //                            }
        //                        }
        //                        else
        //                        {
        //                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Not yet on last day.");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return Request.CreateResponse(HttpStatusCode.BadRequest, "No Data found on the server.");
        //                    }
        //                }
        //                else
        //                {
        //                    return Request.CreateResponse(HttpStatusCode.BadRequest);
        //                }
        //            }
        //            else
        //            {
        //                return Request.CreateResponse(HttpStatusCode.BadRequest);
        //            }
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotFound);
        //        }
        //    }
        //    catch
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }
        //}

        //// delete collection reconstruct
        //[Authorize]
        //[HttpDelete]
        //[Route("api/collectionReconstruct/delete/{id}")]
        //public HttpResponseMessage deleteCollectionReconstruct(String id)
        //{
        //    try
        //    {
        //        var collectionReconstructs = from d in db.trnCollectionReconstructs where d.Id == Convert.ToInt32(id) select d;
        //        if (collectionReconstructs.Any())
        //        {
        //            var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
        //            var mstUserForms = from d in db.mstUserForms
        //                               where d.UserId == userId
        //                               select new Models.MstUserForm
        //                               {
        //                                   Id = d.Id,
        //                                   Form = d.sysForm.Form,
        //                                   CanPerformActions = d.CanPerformActions
        //                               };

        //            if (mstUserForms.Any())
        //            {
        //                String matchPageString = "CollectionDetail";
        //                Boolean canPerformActions = false;

        //                foreach (var mstUserForm in mstUserForms)
        //                {
        //                    if (mstUserForm.Form.Equals(matchPageString))
        //                    {
        //                        if (mstUserForm.CanPerformActions)
        //                        {
        //                            canPerformActions = true;
        //                        }

        //                        break;
        //                    }
        //                }

        //                if (canPerformActions)
        //                {
        //                    var lastCollectionReconstruct = from d in db.trnCollectionReconstructs.OrderByDescending(d => d.Id)
        //                                                    where d.CollectionId == collectionReconstructs.FirstOrDefault().CollectionId
        //                                                    select d;

        //                    if (lastCollectionReconstruct.Any())
        //                    {
        //                        if (lastCollectionReconstruct.FirstOrDefault().Id == Convert.ToInt32(id))
        //                        {
        //                            db.trnCollectionReconstructs.DeleteOnSubmit(collectionReconstructs.First());
        //                            db.SubmitChanges();

        //                            return Request.CreateResponse(HttpStatusCode.OK);
        //                        }
        //                        else
        //                        {
        //                            return Request.CreateResponse(HttpStatusCode.BadRequest);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //                    }
        //                }
        //                else
        //                {
        //                    return Request.CreateResponse(HttpStatusCode.BadRequest);
        //                }
        //            }
        //            else
        //            {
        //                return Request.CreateResponse(HttpStatusCode.BadRequest);
        //            }
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotFound);
        //        }
        //    }
        //    catch
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }
        //}
    }
}
