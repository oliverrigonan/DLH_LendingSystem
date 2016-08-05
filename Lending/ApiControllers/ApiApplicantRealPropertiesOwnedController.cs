using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
        public List<Models.tblApplicantRealPropertiesOwned> listApplicantRealPropertiesOwned(String applicantId)
        {
            var applicantRealPropertiesOwneds = from d in db.tblApplicantRealPropertiesOwneds
                                                where d.ApplicantId == Convert.ToInt32(applicantId)
                                                select new Models.tblApplicantRealPropertiesOwned
                                                {
                                                    Id = d.Id,
                                                    ApplicantId = d.ApplicantId,
                                                    Applicant = d.tblApplicant.FullName,
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
        public HttpResponseMessage addApplicantRealPropertiesOwned(Models.tblApplicantRealPropertiesOwned realPropertiesOwned)
        {
            try
            {
                Data.tblApplicantRealPropertiesOwned newRealPropertiesOwned = new Data.tblApplicantRealPropertiesOwned();
                newRealPropertiesOwned.ApplicantId = realPropertiesOwned.ApplicantId;
                newRealPropertiesOwned.Real = realPropertiesOwned.Real;
                newRealPropertiesOwned.Location = realPropertiesOwned.Location;
                newRealPropertiesOwned.PresentValue = realPropertiesOwned.PresentValue;
                newRealPropertiesOwned.EcumberedTo = realPropertiesOwned.EcumberedTo;

                db.tblApplicantRealPropertiesOwneds.InsertOnSubmit(newRealPropertiesOwned);
                db.SubmitChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
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
        public HttpResponseMessage updateApplicantRealPropertiesOwned(String id, Models.tblApplicantRealPropertiesOwned realPropertiesOwned)
        {
            try
            {
                var realPropertiesOwneds = from d in db.tblApplicantRealPropertiesOwneds where d.Id == Convert.ToInt32(id) select d;
                if (realPropertiesOwneds.Any())
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
                var realPropertiesOwneds = from d in db.tblApplicantRealPropertiesOwneds where d.Id == Convert.ToInt32(id) select d;
                if (realPropertiesOwneds.Any())
                {
                    db.tblApplicantRealPropertiesOwneds.DeleteOnSubmit(realPropertiesOwneds.First());
                    db.SubmitChanges();

                    return Request.CreateResponse(HttpStatusCode.OK);
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
