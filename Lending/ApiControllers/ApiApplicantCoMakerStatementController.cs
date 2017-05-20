using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiApplicantCoMakerStatementController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // applicant co maker statement
        [Authorize]
        [HttpGet]
        [Route("api/applicantCoMakerStatement/listByApplicantId/{applicantId}")]
        public List<Models.MstApplicantCoMakerStatement> listApplicantRealPropertiesOwnedByApplicantId(String applicantId)
        {
            var applicantCoMakerStatements = from d in db.mstApplicantCoMakerStatements.OrderByDescending(d => d.Id)
                                             where d.ApplicantId == Convert.ToInt32(applicantId)
                                             select new Models.MstApplicantCoMakerStatement
                                             {
                                                 Id = d.Id,
                                                 ApplicantId = d.ApplicantId,
                                                 CoMakerApplicantId = d.CoMakerApplicantId,
                                                 CoMaker = d.mstApplicant1.ApplicantLastName + " " + d.mstApplicant1.ApplicantFirstName + ", " + d.mstApplicant1.ApplicantMiddleName,
                                                 ContactNumber = d.mstApplicant1.ContactNumber
                                             };

            return applicantCoMakerStatements.ToList();
        }

        // add co maker statement
        [Authorize]
        [HttpPost]
        [Route("api/applicantCoMakerStatement/add")]
        public HttpResponseMessage addApplicantCoMakerStatement(Models.MstApplicantCoMakerStatement coMakerStatement)
        {
            try
            {
                var applicants = from d in db.mstApplicants where d.Id == Convert.ToInt32(coMakerStatement.ApplicantId) select d;
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
                                Data.mstApplicantCoMakerStatement newCoMaker = new Data.mstApplicantCoMakerStatement();
                                newCoMaker.ApplicantId = coMakerStatement.ApplicantId;
                                newCoMaker.CoMakerApplicantId = coMakerStatement.CoMakerApplicantId;
                                db.mstApplicantCoMakerStatements.InsertOnSubmit(newCoMaker);
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

        // update co maker statement
        [Authorize]
        [HttpPut]
        [Route("api/applicantCoMakerStatement/update/{id}")]
        public HttpResponseMessage updateApplicantCoMakerStatement(String id, Models.MstApplicantCoMakerStatement coMakerStatement)
        {
            try
            {
                var applicants = from d in db.mstApplicants where d.Id == Convert.ToInt32(coMakerStatement.ApplicantId) select d;
                if (applicants.Any())
                {
                    if (!applicants.FirstOrDefault().IsLocked)
                    {
                        var coMakerStatements = from d in db.mstApplicantCoMakerStatements where d.Id == Convert.ToInt32(id) select d;
                        if (coMakerStatements.Any())
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
                                    var updateCoMaker = coMakerStatements.FirstOrDefault();
                                    updateCoMaker.CoMakerApplicantId = coMakerStatement.CoMakerApplicantId;
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

        // delete co maker statement
        [Authorize]
        [HttpDelete]
        [Route("api/applicantCoMakerStatement/delete/{id}")]
        public HttpResponseMessage deleteApplicantCoMakerStatement(String id)
        {
            try
            {
                var coMakerStatements = from d in db.mstApplicantCoMakerStatements where d.Id == Convert.ToInt32(id) select d;
                if (coMakerStatements.Any())
                {
                    var applicants = from d in db.mstApplicants where d.Id == coMakerStatements.FirstOrDefault().ApplicantId select d;
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
                                    db.mstApplicantCoMakerStatements.DeleteOnSubmit(coMakerStatements.First());
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
