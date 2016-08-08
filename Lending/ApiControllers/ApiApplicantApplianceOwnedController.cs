using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiApplicantApplianceOwnedController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // applicant appliance owned
        [Authorize]
        [HttpGet]
        [Route("api/applicantApplianceOwned/listByApplicantId/{applicantId}")]
        public List<Models.MstApplicantApplianceOwned> listApplicantApplianceOwned(String applicantId)
        {
            var applicantApplianceOwneds = from d in db.mstApplicantApplianceOwneds
                                           where d.ApplicantId == Convert.ToInt32(applicantId)
                                           select new Models.MstApplicantApplianceOwned
                                           {
                                               Id = d.Id,
                                               ApplicantId = d.ApplicantId,
                                               Applicant = d.mstApplicant.FullName,
                                               ApplianceBrand = d.ApplianceBrand,
                                               PresentValue = d.PresentValue
                                           };

            return applicantApplianceOwneds.ToList();
        }

        // add applicant appliance owned
        [Authorize]
        [HttpPost]
        [Route("api/applicantApplianceOwned/add")]
        public HttpResponseMessage addApplicantApplianceOwned(Models.MstApplicantApplianceOwned applianceOwned)
        {
            try
            {
                Data.mstApplicantApplianceOwned newApplianceOwned = new Data.mstApplicantApplianceOwned();
                newApplianceOwned.ApplicantId = applianceOwned.ApplicantId;
                newApplianceOwned.ApplianceBrand = applianceOwned.ApplianceBrand;
                newApplianceOwned.PresentValue = applianceOwned.PresentValue;

                db.mstApplicantApplianceOwneds.InsertOnSubmit(newApplianceOwned);
                db.SubmitChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // update applicant appliance owned
        [Authorize]
        [HttpPut]
        [Route("api/applicantApplianceOwned/update/{id}")]
        public HttpResponseMessage updateApplicantApplianceOwned(String id, Models.MstApplicantApplianceOwned applianceOwned)
        {
            try
            {
                var applianceOwneds = from d in db.mstApplicantApplianceOwneds where d.Id == Convert.ToInt32(id) select d;
                if (applianceOwneds.Any())
                {
                    var updateApplianceOwned = applianceOwneds.FirstOrDefault();
                    updateApplianceOwned.ApplicantId = applianceOwned.ApplicantId;
                    updateApplianceOwned.ApplianceBrand = applianceOwned.ApplianceBrand;
                    updateApplianceOwned.PresentValue = applianceOwned.PresentValue;

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

        // delete  applicant appliance owned
        [Authorize]
        [HttpDelete]
        [Route("api/applicantApplianceOwned/delete/{id}")]
        public HttpResponseMessage deleteApplicantApplianceOwned(String id)
        {
            try
            {
                var applianceOwneds = from d in db.mstApplicantApplianceOwneds where d.Id == Convert.ToInt32(id) select d;
                if (applianceOwneds.Any())
                {
                    db.mstApplicantApplianceOwneds.DeleteOnSubmit(applianceOwneds.First());
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
