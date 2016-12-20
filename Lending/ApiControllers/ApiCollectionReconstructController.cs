using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiCollectionReconstructController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // collection reconstruct list
        [Authorize]
        [HttpGet]
        [Route("api/collectionReconstruct/list/ByCollectionId/{collectionId}")]
        public List<Models.TrnCollectionReconstruct> listCollectionReconstructByCollectionId(String collectionId)
        {
            var collecionReconstruct = from d in db.trnCollectionReconstructs
                                       where d.CollectionId == Convert.ToInt32(collectionId)
                                       select new Models.TrnCollectionReconstruct
                                       {
                                           Id = d.Id,
                                           CollectionId = d.CollectionId,
                                           ReconstructNumber = d.ReconstructNumber,
                                           StartDate = d.StartDate.ToShortDateString(),
                                           EndDate = d.EndDate.ToShortDateString(),
                                           TermId = d.TermId,
                                           Term = d.mstTerm.Term,
                                           TermNoOfDays = d.TermNoOfDays,
                                           TermNoOfAllowanceDays = d.TermNoOfAllowanceDays,
                                           InterestId = d.InterestId,
                                           InterestRate = d.InterestRate,
                                           PenaltyId = d.PenaltyId,
                                           Penalty = d.mstPenalty.Penalty
                                       };

            return collecionReconstruct.ToList();
        }

        // add collection reconstruct
        [Authorize]
        [HttpPost]
        [Route("api/collectionReconstruct/add")]
        public HttpResponseMessage addCollectionReconstruct(Models.TrnCollectionReconstruct collectionReconstruct)
        {
            try
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
                        Data.trnCollectionReconstruct newCollectionReconstruct = new Data.trnCollectionReconstruct();
                        newCollectionReconstruct.CollectionId = collectionReconstruct.CollectionId;
                        newCollectionReconstruct.ReconstructNumber = collectionReconstruct.ReconstructNumber;
                        newCollectionReconstruct.StartDate = Convert.ToDateTime(collectionReconstruct.StartDate);
                        newCollectionReconstruct.EndDate = Convert.ToDateTime(collectionReconstruct.EndDate);
                        newCollectionReconstruct.TermId = collectionReconstruct.TermId;
                        newCollectionReconstruct.TermNoOfDays = collectionReconstruct.TermNoOfDays;
                        newCollectionReconstruct.TermNoOfAllowanceDays = collectionReconstruct.TermNoOfAllowanceDays;
                        newCollectionReconstruct.InterestId = collectionReconstruct.InterestId;
                        newCollectionReconstruct.InterestRate = collectionReconstruct.InterestRate;
                        newCollectionReconstruct.PenaltyId = collectionReconstruct.PenaltyId;
                        db.trnCollectionReconstructs.InsertOnSubmit(newCollectionReconstruct);
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
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // update collection reconstruct
        [Authorize]
        [HttpPut]
        [Route("api/collectionReconstruct/update/{id}")]
        public HttpResponseMessage updateCollectionReconstruct(String id, Models.TrnCollectionReconstruct collectionReconstruct)
        {
            try
            {
                var collectionReconstructs = from d in db.trnCollectionReconstructs where d.Id == Convert.ToInt32(id) select d;
                if (collectionReconstructs.Any())
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
                            var updateCollectionReconstruct = collectionReconstructs.FirstOrDefault();
                            updateCollectionReconstruct.CollectionId = collectionReconstruct.CollectionId;
                            updateCollectionReconstruct.ReconstructNumber = collectionReconstruct.ReconstructNumber;
                            updateCollectionReconstruct.StartDate = Convert.ToDateTime(collectionReconstruct.StartDate);
                            updateCollectionReconstruct.EndDate = Convert.ToDateTime(collectionReconstruct.EndDate);
                            updateCollectionReconstruct.TermId = collectionReconstruct.TermId;
                            updateCollectionReconstruct.TermNoOfDays = collectionReconstruct.TermNoOfDays;
                            updateCollectionReconstruct.TermNoOfAllowanceDays = collectionReconstruct.TermNoOfAllowanceDays;
                            updateCollectionReconstruct.InterestId = collectionReconstruct.InterestId;
                            updateCollectionReconstruct.InterestRate = collectionReconstruct.InterestRate;
                            updateCollectionReconstruct.PenaltyId = collectionReconstruct.PenaltyId;
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
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // delete collection reconstruct
        [Authorize]
        [HttpDelete]
        [Route("api/collectionReconstruct/delete/{id}")]
        public HttpResponseMessage deleteCollectionReconstruct(String id)
        {
            try
            {
                var collectionReconstructs = from d in db.trnCollectionReconstructs where d.Id == Convert.ToInt32(id) select d;
                if (collectionReconstructs.Any())
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
                            db.trnCollectionReconstructs.DeleteOnSubmit(collectionReconstructs.First());
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
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
