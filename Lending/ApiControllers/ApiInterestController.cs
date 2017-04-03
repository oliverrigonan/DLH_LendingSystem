using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiInterestController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // interest list
        [Authorize]
        [HttpGet]
        [Route("api/interest/list")]
        public List<Models.MstInterest> listInterest()
        {
            var interest = from d in db.mstInterests.OrderByDescending(d => d.Id)
                           select new Models.MstInterest
                            {
                                Id = d.Id,
                                Interest = d.Interest,
                                Description = d.Description,
                                Rate = d.Rate,
                                CreatedByUserId = d.CreatedByUserId,
                                CreatedByUser = d.mstUser.FullName,
                                CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                UpdatedByUserId = d.UpdatedByUserId,
                                UpdatedByUser = d.mstUser1.FullName,
                                UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                            };

            return interest.ToList();
        }

        // add interest 
        [Authorize]
        [HttpPost]
        [Route("api/interest/add")]
        public HttpResponseMessage addInterest(Models.MstInterest interest)
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
                        Data.mstInterest newInterest = new Data.mstInterest();

                        newInterest.Interest = interest.Interest;
                        newInterest.Description = interest.Description;
                        newInterest.Rate = interest.Rate;
                        newInterest.CreatedByUserId = userId;
                        newInterest.CreatedDateTime = DateTime.Now;
                        newInterest.UpdatedByUserId = userId;
                        newInterest.UpdatedDateTime = DateTime.Now;

                        db.mstInterests.InsertOnSubmit(newInterest);
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

        // update interest 
        [Authorize]
        [HttpPut]
        [Route("api/interest/update/{id}")]
        public HttpResponseMessage updateInterest(String id, Models.MstInterest interest)
        {
            try
            {
                var interests = from d in db.mstInterests where d.Id == Convert.ToInt32(id) select d;
                if (interests.Any())
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
                            var updateInterest = interests.FirstOrDefault();

                            updateInterest.Interest = interest.Interest;
                            updateInterest.Description = interest.Description;
                            updateInterest.Rate = interest.Rate;
                            updateInterest.UpdatedByUserId = userId;
                            updateInterest.UpdatedDateTime = DateTime.Now;
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

        // delete interest
        [Authorize]
        [HttpDelete]
        [Route("api/interest/delete/{id}")]
        public HttpResponseMessage deleteInterest(String id)
        {
            try
            {
                var interest = from d in db.mstInterests where d.Id == Convert.ToInt32(id) select d;
                if (interest.Any())
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
                            db.mstInterests.DeleteOnSubmit(interest.First());
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
