using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiApplicantController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // applicant list
        [Authorize]
        [HttpGet]
        [Route("api/applicant/list")]
        public List<Models.tblApplicant> listApplicant()
        {
            var applicants = from d in db.tblApplicants
                             select new Models.tblApplicant
                             {
                                 Id = d.Id,
                                 ApplicantCode = d.ApplicantCode,
                                 FullName = d.FullName,
                                 BirthDate = d.BirthDate.ToShortDateString(),
                                 CivilStatusId = d.CivilStatusId,
                                 CivilStatus = d.tblCivilStatus.CivilStatus,
                                 CityAddress = d.CityAddress,
                                 ProvinceAddress = d.ProvinceAddress,
                                 ResidenceTypeId = d.ResidenceTypeId,
                                 ResidenceType = d.tblResidenceType.ResidenceType,
                                 LengthOfStay = d.LengthOfStay,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.tblUser.FirstName + " " + d.tblUser.MiddleName + " " + d.tblUser.LastName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.tblUser1.FirstName + " " + d.tblUser1.MiddleName + " " + d.tblUser1.LastName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            return applicants.ToList();
        }

        // get applicant
        [Authorize]
        [HttpGet]
        [Route("api/applicant/getById/{id}")]
        public Models.tblApplicant listApplicant(String id)
        {
            var applicants = from d in db.tblApplicants
                             where d.Id == Convert.ToInt32(id)
                             select new Models.tblApplicant
                             {
                                 Id = d.Id,
                                 ApplicantCode = d.ApplicantCode,
                                 FullName = d.FullName,
                                 BirthDate = d.BirthDate.ToShortDateString(),
                                 CivilStatusId = d.CivilStatusId,
                                 CivilStatus = d.tblCivilStatus.CivilStatus,
                                 CityAddress = d.CityAddress,
                                 ProvinceAddress = d.ProvinceAddress,
                                 ResidenceTypeId = d.ResidenceTypeId,
                                 ResidenceType = d.tblResidenceType.ResidenceType,
                                 LengthOfStay = d.LengthOfStay,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.tblUser.FirstName + " " + d.tblUser.MiddleName + " " + d.tblUser.LastName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.tblUser1.FirstName + " " + d.tblUser1.MiddleName + " " + d.tblUser1.LastName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            if (applicants.Any())
            {
                return (Models.tblApplicant)applicants.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        // add applicant 
        [Authorize]
        [HttpPost]
        [Route("api/applicant/add")]
        public Int32 addApplicant()
        {
            try
            {
                var userId = (from d in db.tblUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                Data.tblApplicant newApplicant = new Data.tblApplicant();
                newApplicant.ApplicantCode = "0000000001";
                newApplicant.FullName = "NA";
                newApplicant.BirthDate = DateTime.Today;
                newApplicant.CivilStatusId = (from d in db.tblCivilStatus select d.Id).FirstOrDefault();
                newApplicant.CityAddress = "NA";
                newApplicant.ProvinceAddress = "NA";
                newApplicant.ResidenceTypeId = (from d in db.tblResidenceTypes select d.Id).FirstOrDefault();
                newApplicant.LengthOfStay = 0;
                newApplicant.CreatedByUserId = userId;
                newApplicant.CreatedDateTime = DateTime.Now;
                newApplicant.UpdatedByUserId = userId;
                newApplicant.UpdatedDateTime = DateTime.Now;

                db.tblApplicants.InsertOnSubmit(newApplicant);
                db.SubmitChanges();

                return newApplicant.Id;
            }
            catch
            {
                return 0;
            }
        }

        // delete applicant
        [Authorize]
        [HttpDelete]
        [Route("api/applicant/delete/{id}")]
        public HttpResponseMessage deleteApplicant(String id)
        {
            try
            {
                var applicants = from d in db.tblApplicants where d.Id == Convert.ToInt32(id) select d;
                if (applicants.Any())
                {
                    db.tblApplicants.DeleteOnSubmit(applicants.First());
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
