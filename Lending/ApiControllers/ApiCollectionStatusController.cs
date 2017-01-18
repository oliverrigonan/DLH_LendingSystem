using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiCollectionStatusController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // collection status list
        [Authorize]
        [HttpGet]
        [Route("api/collectionStatus/list")]
        public List<Models.SysCollectionStatus> listCollectionStatus()
        {
            var status = from d in db.sysCollectionStatus
                         select new Models.SysCollectionStatus
                         {
                             Id = d.Id,
                             Status = d.Status,
                         };

            return status.ToList();
        }
    }
}
