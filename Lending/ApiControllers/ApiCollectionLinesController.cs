using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;


namespace Lending.ApiControllers
{
    public class ApiCollectionLinesController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // collection lines list by collection id
        [Authorize]
        [HttpGet]
        [Route("api/collectionLines/listByCollectionId/{collectionId}")]
        public List<Models.TrnCollectionLines> listCollectionLinesByCollectionId(String collectionId)
        {
            var collectionLines = from d in db.trnCollectionLines
                                  where d.CollectionId == Convert.ToInt32(collectionId)
                                  select new Models.TrnCollectionLines
                                  {
                                      Id = d.Id,
                                      CollectionId = d.CollectionId,
                                      LoanId = d.LoanId,
                                      LoanNumber = d.trnLoan.LoanNumber,
                                      LoanLinesId = d.LoanLinesId,
                                      LoanLinesDayReference = d.trnLoanLine.DayReference,
                                      LoanLinesCollectibleDate = d.trnLoanLine.CollectibleDate.ToShortDateString(),
                                      PenaltyId = d.PenaltyId,
                                      Penalty = d.mstPenalty.Penalty,
                                      PenaltyAmount = d.PenaltyAmount,
                                      PaidAmount = d.PaidAmount
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
                                                     where d.LoanLinesId == Convert.ToInt32(collectionLines.LoanLinesId) 
                                                     select d;

                                if (!collectionLine.Any())
                                {
                                    var loanLines = from d in db.trnLoanLines
                                                    where d.Id == Convert.ToInt32(collectionLines.LoanLinesId) 
                                                    select d;

                                    if (loanLines.Any())
                                    {
                                        if (!loanLines.FirstOrDefault().IsCleared)
                                        {
                                            if (loanLines.FirstOrDefault().BalanceAmount > 0)
                                            {
                                                if (collectionLines.PaidAmount < loanLines.FirstOrDefault().BalanceAmount)
                                                {
                                                    Data.trnCollectionLine newCollectionLine = new Data.trnCollectionLine();
                                                    newCollectionLine.CollectionId = collectionLines.CollectionId;
                                                    newCollectionLine.LoanId = collectionLines.LoanId;
                                                    newCollectionLine.LoanLinesId = collectionLines.LoanLinesId;
                                                    newCollectionLine.PenaltyId = collectionLines.PenaltyId;
                                                    newCollectionLine.PenaltyAmount = collectionLines.PenaltyAmount;
                                                    newCollectionLine.PaidAmount = collectionLines.PaidAmount;
                                                    db.trnCollectionLines.InsertOnSubmit(newCollectionLine);
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
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Processed Already");
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
                                    var updateCollectionLines = collectionLine.FirstOrDefault();
                                    updateCollectionLines.CollectionId = collectionLines.CollectionId;
                                    updateCollectionLines.LoanId = collectionLines.LoanId;
                                    updateCollectionLines.LoanLinesId = collectionLines.LoanLinesId;
                                    updateCollectionLines.PenaltyId = collectionLines.PenaltyId;
                                    updateCollectionLines.PenaltyAmount = collectionLines.PenaltyAmount;
                                    updateCollectionLines.PaidAmount = collectionLines.PaidAmount;
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
        [Route("api/collectionLines/delete/{id}")]
        public HttpResponseMessage deleteCollectionLine(String id)
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
    }
}
