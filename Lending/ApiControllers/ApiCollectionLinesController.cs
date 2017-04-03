using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Diagnostics;


namespace Lending.ApiControllers
{
    public class ApiCollectionLinesController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // collection lines list by loan id
        [Authorize]
        [HttpGet]
        [Route("api/collectionLines/listByLoanId/{loanId}")]
        public List<Models.TrnCollectionLines> listCollectionLinesByLoanId(String loanId)
        {
            var collectionLines = from d in db.trnCollectionLines.OrderByDescending(d => d.Id)
                                  where d.trnLoanLine.LoanId == Convert.ToInt32(loanId)
                                  && d.trnCollection.IsLocked == true
                                  select new Models.TrnCollectionLines
                                  {
                                      Id = d.Id,
                                      CollectionId = d.CollectionId,
                                      Collection = d.trnCollection.CollectionNumber,
                                      LoanLinesId = d.LoanLinesId,
                                      LoanLinesDayReference = d.trnLoanLine.DayReference,
                                      LoanLinesCollectibleDate = d.trnLoanLine.CollectibleDate.ToShortDateString(),
                                      PenaltyId = d.PenaltyId,
                                      Penalty = d.mstPenalty.Penalty,
                                      PenaltyAmount = d.PenaltyAmount,
                                      PaidAmount = d.PaidAmount,
                                      CollectedDate = d.trnCollection.CollectionDate.ToShortDateString(),
                                      CollectionStatus = d.trnCollection.sysCollectionStatus.Status
                                  };

            return collectionLines.ToList();
        }

        // collection lines list by collection id
        [Authorize]
        [HttpGet]
        [Route("api/collectionLines/listByCollectionId/{collectionId}")]
        public List<Models.TrnCollectionLines> listCollectionLinesByCollectionId(String collectionId)
        {
            var collectionLines = from d in db.trnCollectionLines.OrderByDescending(d => d.Id)
                                  where d.CollectionId == Convert.ToInt32(collectionId)
                                  select new Models.TrnCollectionLines
                                  {
                                      Id = d.Id,
                                      CollectionId = d.CollectionId,
                                      LoanLinesId = d.LoanLinesId,
                                      LoanLinesDayReference = d.trnLoanLine.DayReference,
                                      LoanLinesCollectibleDate = d.trnLoanLine.CollectibleDate.ToShortDateString(),
                                      PenaltyId = d.PenaltyId,
                                      Penalty = d.mstPenalty.Penalty,
                                      PenaltyAmount = d.PenaltyAmount,
                                      PaidAmount = d.PaidAmount,
                                      CollectionStatus = d.trnCollection.sysCollectionStatus.Status
                                  };

            return collectionLines.ToList();
        }

        // add collection lines
        [Authorize]
        [HttpPost]
        [Route("api/collectionLines/add")]
        public HttpResponseMessage addCollectionLines(Models.TrnCollectionLines collectionLines)
        {
            try
            {
                var collection = from d in db.trnCollections where d.Id == collectionLines.CollectionId select d;
                if (collection.Any())
                {
                    if (!collection.FirstOrDefault().IsLocked)
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
                                var collectionLine = from d in db.trnCollectionLines
                                                     where d.CollectionId == collection.FirstOrDefault().Id
                                                     && d.LoanLinesId == Convert.ToInt32(collectionLines.LoanLinesId)
                                                     select d;

                                if (!collectionLine.Any())
                                {
                                    var loanLines = from d in db.trnLoanLines
                                                    where d.Id == Convert.ToInt32(collectionLines.LoanLinesId)
                                                    select d;

                                    if (loanLines.Any())
                                    {
                                        if (collectionLines.PaidAmount <= loanLines.FirstOrDefault().CollectibleAmount)
                                        {
                                            Data.trnCollectionLine newCollectionLine = new Data.trnCollectionLine();
                                            newCollectionLine.CollectionId = collectionLines.CollectionId;
                                            newCollectionLine.LoanLinesId = collectionLines.LoanLinesId;
                                            newCollectionLine.PenaltyId = collectionLines.PenaltyId;
                                            newCollectionLine.PenaltyAmount = collectionLines.PenaltyAmount;
                                            newCollectionLine.PaidAmount = collectionLines.PaidAmount;
                                            db.trnCollectionLines.InsertOnSubmit(newCollectionLine);
                                            db.SubmitChanges();

                                            var collectionLineAmount = from d in db.trnCollectionLines
                                                                       where d.CollectionId == Convert.ToInt32(collectionLines.CollectionId)
                                                                       select d;

                                            Decimal totalPaidAmount = 0;
                                            if (collectionLineAmount.Any())
                                            {
                                                totalPaidAmount = collectionLineAmount.Sum(d => d.PaidAmount);
                                            }

                                            var updateCollection = collection.FirstOrDefault();
                                            updateCollection.TotalPaidAmount = totalPaidAmount;
                                            db.SubmitChanges();

                                            return Request.CreateResponse(HttpStatusCode.OK);
                                        }
                                        else
                                        {
                                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                                        }
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
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

        // update collection lines
        [Authorize]
        [HttpPut]
        [Route("api/collectionLines/update/{id}")]
        public HttpResponseMessage updateCollectionLines(String id, Models.TrnCollectionLines collectionLines)
        {
            try
            {
                var collection = from d in db.trnCollections where d.Id == collectionLines.CollectionId select d;
                if (collection.Any())
                {
                    if (!collection.FirstOrDefault().IsLocked)
                    {
                        var collectionLine = from d in db.trnCollectionLines where d.Id == Convert.ToInt32(id) select d;
                        if (collectionLine.Any())
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
                                    if (collectionLines.PaidAmount <= collectionLine.FirstOrDefault().trnLoanLine.CollectibleAmount)
                                    {
                                        var updateCollectionLines = collectionLine.FirstOrDefault();
                                        updateCollectionLines.CollectionId = collectionLines.CollectionId;
                                        updateCollectionLines.LoanLinesId = collectionLines.LoanLinesId;
                                        updateCollectionLines.PenaltyId = collectionLines.PenaltyId;
                                        updateCollectionLines.PenaltyAmount = collectionLines.PenaltyAmount;
                                        updateCollectionLines.PaidAmount = collectionLines.PaidAmount;
                                        db.SubmitChanges();

                                        var collectionLineAmount = from d in db.trnCollectionLines
                                                                   where d.CollectionId == Convert.ToInt32(collectionLines.CollectionId)
                                                                   select d;

                                        Decimal totalPaidAmount = 0;
                                        if (collectionLineAmount.Any())
                                        {
                                            totalPaidAmount = collectionLineAmount.Sum(d => d.PaidAmount);
                                        }

                                        var updateCollection = collection.FirstOrDefault();
                                        updateCollection.TotalPaidAmount = totalPaidAmount;
                                        db.SubmitChanges();

                                        return Request.CreateResponse(HttpStatusCode.OK);
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest);
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

        // delete collection lines
        [Authorize]
        [HttpDelete]
        [Route("api/collectionLines/delete/{id}/{collectionId}")]
        public HttpResponseMessage deleteCollectionLine(String id, String collectionId)
        {
            try
            {
                var collectionLines = from d in db.trnCollectionLines where d.Id == Convert.ToInt32(id) select d;
                if (collectionLines.Any())
                {
                    var collection = from d in db.trnCollections where d.Id == collectionLines.FirstOrDefault().CollectionId select d;
                    if (collection.Any())
                    {
                        if (!collection.FirstOrDefault().IsLocked)
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
                                    db.trnCollectionLines.DeleteOnSubmit(collectionLines.First());
                                    db.SubmitChanges();

                                    var collectionLineAmount = from d in db.trnCollectionLines
                                                               where d.CollectionId == Convert.ToInt32(collectionId)
                                                               select d;

                                    Decimal totalPaidAmount = 0;
                                    if (collectionLineAmount.Any())
                                    {
                                        totalPaidAmount = collectionLineAmount.Sum(d => d.PaidAmount);
                                    }

                                    var updateDeleteCollection = from d in db.trnCollections where d.Id == Convert.ToInt32(collectionId) select d;
                                    var updateCollection = updateDeleteCollection.FirstOrDefault();
                                    updateCollection.TotalPaidAmount = totalPaidAmount;
                                    db.SubmitChanges();

                                    return Request.CreateResponse(HttpStatusCode.OK);
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
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

        // full payment
        [Authorize]
        [HttpPost]
        [Route("api/collectionLines/fullPayment")]
        public HttpResponseMessage fullPaymentCollectionLines(Models.TrnCollectionLines collectionLines)
        {
            try
            {
                var collection = from d in db.trnCollections where d.Id == collectionLines.CollectionId select d;
                if (collection.Any())
                {
                    if (!collection.FirstOrDefault().IsLocked)
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
                                var collectionLine = from d in db.trnCollectionLines
                                                     where d.CollectionId == collectionLines.CollectionId
                                                     select d;

                                if (collectionLine.Any())
                                {
                                    db.trnCollectionLines.DeleteAllOnSubmit(collectionLine);
                                    db.SubmitChanges();
                                }

                                var loanLines = from d in db.trnLoanLines
                                                where d.LoanId == collectionLines.LoanId
                                                && d.trnLoan.IsLocked == true
                                                && d.PaidAmount == 0
                                                select new Models.TrnLoanLines
                                                {
                                                    Id = d.Id,
                                                    DayReference = d.DayReference,
                                                    CollectibleDate = d.CollectibleDate.ToShortDateString(),
                                                    CollectibleAmount = d.CollectibleAmount,
                                                    PaidAmount = d.PaidAmount,
                                                    PenaltyAmount = d.PenaltyAmount
                                                };

                                if (loanLines.Any())
                                {
                                    var penalty = from d in db.mstPenalties
                                                  select d;

                                    if (penalty.Any())
                                    {
                                        foreach (var loanLine in loanLines)
                                        {
                                            Data.trnCollectionLine newCollectionLine = new Data.trnCollectionLine();
                                            newCollectionLine.CollectionId = collectionLines.CollectionId;
                                            newCollectionLine.LoanLinesId = loanLine.Id;
                                            newCollectionLine.PenaltyId = penalty.FirstOrDefault().Id;
                                            newCollectionLine.PenaltyAmount = 0;
                                            newCollectionLine.PaidAmount = loanLine.CollectibleAmount;
                                            db.trnCollectionLines.InsertOnSubmit(newCollectionLine);
                                            db.SubmitChanges();
                                        }

                                        return Request.CreateResponse(HttpStatusCode.OK);
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
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
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

    }
}
