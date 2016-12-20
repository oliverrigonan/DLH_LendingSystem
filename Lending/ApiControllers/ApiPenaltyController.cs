using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiPenaltyController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // penalty list
        [Authorize]
        [HttpGet]
        [Route("api/penalty/list")]
        public List<Models.MstPenalty> listPenalty()
        {
            var penalty = from d in db.mstPenalties
                          select new Models.MstPenalty
                          {
                              Id = d.Id,
                              Penalty = d.Penalty,
                              Description = d.Description,
                              DefaultPenaltyAmount = d.DefaultPenaltyAmount,
                              IsPenaltyEveryAbsent = d.IsPenaltyEveryAbsent,
                              NoOfLimitAbsent = d.NoOfLimitAbsent,
                              PenaltyAmountOverNoOfLimitAbsent = d.PenaltyAmountOverNoOfLimitAbsent,
                              CreatedByUserId = d.CreatedByUserId,
                              CreatedByUser = d.mstUser.FullName,
                              CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                              UpdatedByUserId = d.UpdatedByUserId,
                              UpdatedByUser = d.mstUser1.FullName,
                              UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                          };

            return penalty.ToList();
        }

        // add penalty 
        [Authorize]
        [HttpPost]
        [Route("api/penalty/add")]
        public HttpResponseMessage addPenalty(Models.MstPenalty penalty)
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
                    String matchPageString = "SystemTables";
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
                        Data.mstPenalty newPenalty = new Data.mstPenalty();
                        newPenalty.Penalty = penalty.Penalty;
                        newPenalty.Description = penalty.Description;
                        newPenalty.DefaultPenaltyAmount = penalty.DefaultPenaltyAmount;
                        newPenalty.IsPenaltyEveryAbsent = penalty.IsPenaltyEveryAbsent;
                        newPenalty.NoOfLimitAbsent = penalty.NoOfLimitAbsent;
                        newPenalty.PenaltyAmountOverNoOfLimitAbsent = penalty.PenaltyAmountOverNoOfLimitAbsent;
                        newPenalty.CreatedByUserId = userId;
                        newPenalty.CreatedDateTime = DateTime.Now;
                        newPenalty.UpdatedByUserId = userId;
                        newPenalty.UpdatedDateTime = DateTime.Now;

                        db.mstPenalties.InsertOnSubmit(newPenalty);
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

        // update penalty 
        [Authorize]
        [HttpPut]
        [Route("api/penalty/update/{id}")]
        public HttpResponseMessage updatePenalty(String id, Models.MstPenalty penalty)
        {
            try
            {
                var penaltys = from d in db.mstPenalties where d.Id == Convert.ToInt32(id) select d;
                if (penaltys.Any())
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
                        String matchPageString = "SystemTables";
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
                            var updatePenalty = penaltys.FirstOrDefault();

                            updatePenalty.Penalty = penalty.Penalty;
                            updatePenalty.Description = penalty.Description;
                            updatePenalty.DefaultPenaltyAmount = penalty.DefaultPenaltyAmount;
                            updatePenalty.IsPenaltyEveryAbsent = penalty.IsPenaltyEveryAbsent;
                            updatePenalty.NoOfLimitAbsent = penalty.NoOfLimitAbsent;
                            updatePenalty.PenaltyAmountOverNoOfLimitAbsent = penalty.PenaltyAmountOverNoOfLimitAbsent;
                            updatePenalty.UpdatedByUserId = userId;
                            updatePenalty.UpdatedDateTime = DateTime.Now;
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

        // delete penalty
        [Authorize]
        [HttpDelete]
        [Route("api/penalty/delete/{id}")]
        public HttpResponseMessage deletePenalty(String id)
        {
            try
            {
                var penalty = from d in db.mstPenalties where d.Id == Convert.ToInt32(id) select d;
                if (penalty.Any())
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
                        String matchPageString = "SystemTables";
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
                            db.mstPenalties.DeleteOnSubmit(penalty.First());
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
