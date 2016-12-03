using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiCoMakerStatementApplianceOwnedController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // co maker appliance owned
        [Authorize]
        [HttpGet]
        [Route("api/coMakerStatementApplianceOwned/listByCoMakerId/{coMakerId}")]
        public List<Models.MstCoMakerStatementApplianceOwned> listCoMakerStatementApplianceOwnedByCoMakerId(String coMakerId)
        {
            var coMakerStatementApplianceOwneds = from d in db.mstCoMakerStatementApplianceOwneds
                                                  where d.CoMakerId == Convert.ToInt32(coMakerId)
                                                  select new Models.MstCoMakerStatementApplianceOwned
                                                  {
                                                      Id = d.Id,
                                                      CoMakerId = d.CoMakerId,
                                                      CoMaker = d.mstCoMakerStatement.CoMakerLastName + ", " + d.mstCoMakerStatement.CoMakerFirstName + " " + d.mstCoMakerStatement.CoMakerMiddleName,
                                                      ApplianceBrand = d.ApplianceBrand,
                                                      PresentValue = d.PresentValue
                                                  };

            return coMakerStatementApplianceOwneds.ToList();
        }

        // add co maker appliance owned
        [Authorize]
        [HttpPost]
        [Route("api/coMakerStatementApplianceOwned/add")]
        public HttpResponseMessage addCoMakerStatementApplianceOwned(Models.MstCoMakerStatementApplianceOwned coMakerApplianceOwned)
        {
            try
            {
                var applicants = from d in db.mstApplicants where d.mstCoMakerStatements.FirstOrDefault().Id == coMakerApplianceOwned.CoMakerId select d;
                if (applicants.Any())
                {
                    if(!applicants.FirstOrDefault().IsLocked) 
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
                            String matchPageString = "ApplicantDetail";
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
                                Data.mstCoMakerStatementApplianceOwned newCoMakerApplianceOwned = new Data.mstCoMakerStatementApplianceOwned();
                                newCoMakerApplianceOwned.CoMakerId = coMakerApplianceOwned.CoMakerId;
                                newCoMakerApplianceOwned.ApplianceBrand = coMakerApplianceOwned.ApplianceBrand;
                                newCoMakerApplianceOwned.PresentValue = coMakerApplianceOwned.PresentValue;

                                db.mstCoMakerStatementApplianceOwneds.InsertOnSubmit(newCoMakerApplianceOwned);
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
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }


        // update co maker appliance owned
        [Authorize]
        [HttpPut]
        [Route("api/coMakerStatementApplianceOwned/update/{id}")]
        public HttpResponseMessage updateCoMakerStatementApplianceOwned(String id, Models.MstCoMakerStatementApplianceOwned coMakerApplianceOwned)
        {
            try
            {
                var applicants = from d in db.mstApplicants where d.mstCoMakerStatements.FirstOrDefault().Id == coMakerApplianceOwned.CoMakerId select d;
                if (applicants.Any())
                {
                    if (!applicants.FirstOrDefault().IsLocked)
                    {
                        var coMakerApplianceOwneds = from d in db.mstCoMakerStatementApplianceOwneds where d.Id == Convert.ToInt32(id) select d;
                        if (coMakerApplianceOwneds.Any())
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
                                String matchPageString = "ApplicantDetail";
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
                                    var updateCoMakerApplianceOwned = coMakerApplianceOwneds.FirstOrDefault();
                                    updateCoMakerApplianceOwned.CoMakerId = coMakerApplianceOwned.CoMakerId;
                                    updateCoMakerApplianceOwned.ApplianceBrand = coMakerApplianceOwned.ApplianceBrand;
                                    updateCoMakerApplianceOwned.PresentValue = coMakerApplianceOwned.PresentValue;

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

        // delete  co maker appliance owned
        [Authorize]
        [HttpDelete]
        [Route("api/coMakerStatementApplianceOwned/delete/{id}")]
        public HttpResponseMessage deleteCoMakerStatementApplianceOwned(String id)
        {
            try
            {
                var coMakerApplianceOwneds = from d in db.mstCoMakerStatementApplianceOwneds where d.Id == Convert.ToInt32(id) select d;
                if (coMakerApplianceOwneds.Any())
                {
                    var applicants = from d in db.mstApplicants where d.mstCoMakerStatements.FirstOrDefault().Id == coMakerApplianceOwneds.FirstOrDefault().CoMakerId select d;
                    if (applicants.Any())
                    {
                        if (!applicants.FirstOrDefault().IsLocked)
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
                                String matchPageString = "ApplicantDetail";
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
                                    db.mstCoMakerStatementApplianceOwneds.DeleteOnSubmit(coMakerApplianceOwneds.First());
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
