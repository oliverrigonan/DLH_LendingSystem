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
        public List<Models.SysCivilStatus> listApplicantCivilStatus()
        {
            var civilStatus = from d in db.sysCivilStatus
                              select new Models.SysCivilStatus
                              {
                                  Id = d.Id,
                                  CivilStatus = d.CivilStatus
                              };

            return civilStatus.ToList();
        }
    }
}
