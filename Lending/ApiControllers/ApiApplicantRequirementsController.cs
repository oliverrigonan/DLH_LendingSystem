using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiApplicantRequirementsController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // applicant requirements list
        [Authorize]
        [HttpGet]
        [Route("api/applicantRequirements/listByLoanId/{applicantId}")]
        public List<Models.MstApplicantRequirements> listApplicantRequirementsByApplicantId(String applicantId)
        {
            var applicantRequirements = from d in db.mstApplicantRequirements
                                   where d.ApplicantId == Convert.ToInt32(applicantId)
                                   select new Models.MstApplicantRequirements
                                   {
                                       Id = d.Id,
                                       ApplicantId = d.ApplicantId,
                                       RequirementId = d.RequirementId,
                                       Requirement = d.mstRequirement.Requirement,
                                       Note = d.Note,
                                       ValidDateUntil = d.ValidDateUntil.ToShortDateString()
                                   };

            return applicantRequirements.ToList();
        }

        // add applicant requirements
        [Authorize]
        [HttpPost]
        [Route("api/applicantRequirements/add")]
        public HttpResponseMessage addApplicantRequirements(Models.MstApplicantRequirements applicantRequirement)
        {
            try
            {
                var applicant = from d in db.mstApplicants where d.Id == applicantRequirement.ApplicantId select d;
                if (applicant.Any())
                {
                    if (!applicant.FirstOrDefault().IsLocked)
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
                                Data.mstApplicantRequirement newApplicantRequirement = new Data.mstApplicantRequirement();
                                newApplicantRequirement.ApplicantId = applicantRequirement.ApplicantId;
                                newApplicantRequirement.RequirementId = applicantRequirement.RequirementId;
                                newApplicantRequirement.Note = applicantRequirement.Note;
                                newApplicantRequirement.ValidDateUntil = Convert.ToDateTime(applicantRequirement.ValidDateUntil);
                                db.mstApplicantRequirements.InsertOnSubmit(newApplicantRequirement);
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

        // update applicant requirements
        [Authorize]
        [HttpPut]
        [Route("api/applicantRequirements/update/{id}")]
        public HttpResponseMessage updateApplicantRequirements(String id, Models.MstApplicantRequirements applicantRequirement)
        {
            try
            {
                var applicant = from d in db.mstApplicants where d.Id == applicantRequirement.ApplicantId select d;
                if (applicant.Any())
                {
                    if (!applicant.FirstOrDefault().IsLocked)
                    {
                        var applicantRequirements = from d in db.mstApplicantRequirements where d.Id == Convert.ToInt32(id) select d;
                        if (applicantRequirements.Any())
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
                                    var updateLoanRequirement = applicantRequirements.FirstOrDefault();
                                    updateLoanRequirement.RequirementId = applicantRequirement.RequirementId;
                                    updateLoanRequirement.Note = applicantRequirement.Note;
                                    updateLoanRequirement.ValidDateUntil = Convert.ToDateTime(applicantRequirement.ValidDateUntil);
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

        // delete applicant Requirements
        [Authorize]
        [HttpDelete]
        [Route("api/applicantRequirements/delete/{id}")]
        public HttpResponseMessage deleteApplicantRequirements(String id)
        {
            try
            {
                var applicantRequirements = from d in db.mstApplicantRequirements where d.Id == Convert.ToInt32(id) select d;
                if (applicantRequirements.Any())
                {
                    var applicant = from d in db.mstApplicants where d.Id == applicantRequirements.FirstOrDefault().ApplicantId select d;
                    if (applicant.Any())
                    {
                        if (!applicant.FirstOrDefault().IsLocked)
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
                                    db.mstApplicantRequirements.DeleteOnSubmit(applicantRequirements.First());
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
