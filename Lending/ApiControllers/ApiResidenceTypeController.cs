using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiResidenceTypeController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // residence type list
        [Authorize]
        [HttpGet]
        [Route("api/residenceType/list")]
        public List<Models.tblResidenceType> listApplicant()
        {
            var residenceTypes = from d in db.tblResidenceTypes
                                 select new Models.tblResidenceType
                                 {
                                     Id = d.Id,
                                     ResidenceType = d.ResidenceType,
                                     CreatedByUserId = d.CreatedByUserId,
                                     CreatedByUser = d.tblUser.FirstName + " " + d.tblUser.MiddleName + " " + d.tblUser.LastName,
                                     CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                     UpdatedByUserId = d.UpdatedByUserId,
                                     UpdatedByUser = d.tblUser1.FirstName + " " + d.tblUser1.MiddleName + " " + d.tblUser1.LastName,
                                     UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                                 };

            return residenceTypes.ToList();
        }
    }
}
