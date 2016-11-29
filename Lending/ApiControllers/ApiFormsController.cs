using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiFormsController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // forms list
        [Authorize]
        [HttpGet]
        [Route("api/forms/list")]
        public List<Models.SysForms> listForms()
        {
            var forms = from d in db.sysForms
                        select new Models.SysForms
                        {
                            Id = d.Id,
                            Form = d.Form
                        };

            return forms.ToList();
        }
    }
}
