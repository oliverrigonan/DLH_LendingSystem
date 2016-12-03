using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiApplicantRealPropertiesOwnedController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // applicant real properties owned list by applicant id
        [Authorize]
        [HttpGet]
        [Route("api/applicantRealPropertiesOwned/listByApplicantId/{applicantId}")]
        public List<Models.MstApplicantRealPropertiesOwned> listApplicantRealPropertiesOwnedByApplicantId(String applicantId)
        {
            var applicantRealPropertiesOwneds = from d in db.mstApplicantRealPropertiesOwneds
                                                where d.ApplicantId == Convert.ToInt32(applicantId)
                                                select new Models.MstApplicantRealPropertiesOwned
                                                {
                                                    Id = d.Id,
                                                    ApplicantId = d.ApplicantId,
                                                    Applicant = d.mstApplicant.ApplicantLastName + " " + d.mstApplicant.ApplicantFirstName + ", " + d.mstApplicant.ApplicantMiddleName,
                                                    Real = d.Real,
                                                    Location = d.Location,
                                                    PresentValue = d.PresentValue,
                                                    EcumberedTo = d.EcumberedTo
                                                };

            return applicantRealPropertiesOwneds.ToList();
        }

        // add applicant Real Properties Owned 
        [Authorize]
        [HttpPost]
        [Route("api/applicantRealPropertiesOwned/add")]
        public HttpResponseMessage addApplicantRealPropertiesOwned(Models.MstApplicantRealPropertiesOwned realPropertiesOwned)
        {
            try
            {
                var applicants = from d in db.mstApplicants where d.Id == Convert.ToInt32(realPropertiesOwned.ApplicantId) select d;
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
                                Data.mstApplicantRealPropertiesOwned newRealPropertiesOwned = new Data.mstApplicantRealPropertiesOwned();
                                newRealPropertiesOwned.ApplicantId = realPropertiesOwned.ApplicantId;
                                newRealPropertiesOwned.Real = realPropertiesOwned.Real;
                                newRealPropertiesOwned.Location = realPropertiesOwned.Location;
                                newRealPropertiesOwned.PresentValue = realPropertiesOwned.PresentValue;
                                newRealPropertiesOwned.EcumberedTo = realPropertiesOwned.EcumberedTo;

                                db.mstApplicantRealPropertiesOwneds.InsertOnSubmit(newRealPropertiesOwned);
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

        // update applicant Real Properties Owned 
        [Authorize]
        [HttpPut]
        [Route("api/applicantRealPropertiesOwned/update/{id}")]
        public HttpResponseMessage updateApplicantRealPropertiesOwned(String id, Models.MstApplicantRealPropertiesOwned realPropertiesOwned)
        {
            try
            {
                var applicants = from d in db.mstApplicants where d.Id == Convert.ToInt32(realPropertiesOwned.ApplicantId) select d;
                if (applicants.Any())
                {
                    if (!applicants.FirstOrDefault().IsLocked)
                    {
                        var realPropertiesOwneds = from d in db.mstApplicantRealPropertiesOwneds where d.Id == Convert.ToInt32(id) select d;
                        if (realPropertiesOwneds.Any())
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
                                    var updateRealPropertiesOwneds = realPropertiesOwneds.FirstOrDefault();
                                    updateRealPropertiesOwneds.ApplicantId = realPropertiesOwned.ApplicantId;
                                    updateRealPropertiesOwneds.Real = realPropertiesOwned.Real;
                                    updateRealPropertiesOwneds.Location = realPropertiesOwned.Location;
                                    updateRealPropertiesOwneds.PresentValue = realPropertiesOwned.PresentValue;
                                    updateRealPropertiesOwneds.EcumberedTo = realPropertiesOwned.EcumberedTo;

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

        // delete applicant Real Properties Owned 
        [Authorize]
        [HttpDelete]
        [Route("api/applicantRealPropertiesOwned/delete/{id}")]
        public HttpResponseMessage deleteApplicantRealPropertiesOwned(String id)
        {
            try
            {
                var realPropertiesOwneds = from d in db.mstApplicantRealPropertiesOwneds where d.Id == Convert.ToInt32(id) select d;
                if (realPropertiesOwneds.Any())
                {
                    var applicants = from d in db.mstApplicants where d.Id == realPropertiesOwneds.FirstOrDefault().ApplicantId select d;
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
                                    db.mstApplicantRealPropertiesOwneds.DeleteOnSubmit(realPropertiesOwneds.First());
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
