using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
                                  FirstName = d.FirstName,
                                  MiddleName = d.MiddleName,
                                  LastName = d.LastName,
                                  BirthDate = d.BirthDate.ToShortDateString(),
                                  Gender = d.Gender,
                                  Status = d.Status,
                                  CityAddress = d.CityAddress,
                                  ProvinceAddress = d.ProvinceAddress,
                                  ResidenceTypeId = d.ResidenceTypeId,
                                  ResidenceType = d.tblResidenceType.ResidenceType,
                                  LengthOfStay = d.LengthOfStay,
                                  CreatedByUserId = d.CreatedByUserId,
                                  CreatedByUser = d.tblUser.FirstName + " " + d.tblUser.MiddleName + " " +  d.tblUser.LastName,
                                  CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                  UpdatedByUserId = d.UpdatedByUserId,
                                  UpdatedByUser = d.tblUser1.FirstName + " " + d.tblUser1.MiddleName + " " + d.tblUser1.LastName,
                                  UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            return applicants.ToList();
        }

    }
}
