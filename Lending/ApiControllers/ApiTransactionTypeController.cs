using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiTransactionTypeController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // transaction type list
        [Authorize]
        [HttpGet]
        [Route("api/transactionType/list")]
        public List<Models.MstTransactionType> listTransactionType()
        {
            var transactionTypes = from d in db.mstTransactionTypes
                                   select new Models.MstTransactionType
                                   {
                                       Id = d.Id,
                                       TransactionType = d.TransactionType,
                                   };

            return transactionTypes.ToList();
        }
    }
}
