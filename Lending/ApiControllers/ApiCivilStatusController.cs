using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiCivilStatusController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // residence type list
        [Authorize]
        [HttpGet]
        [Route("api/civilStatus/list")]
        public List<Models.MstApplicantCivilStatus> listApplicantCivilStatus()
        {
            var civilStatus = from d in db.mstCivilStatus
                                 select new Models.MstApplicantCivilStatus
                                 {
                                     Id = d.Id,
                                     CivilStatus = d.CivilStatus
                                 };

            return civilStatus.ToList();
        }
    }
}
