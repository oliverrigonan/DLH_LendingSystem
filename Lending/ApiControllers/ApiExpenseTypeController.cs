using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiExpenseTypeController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // expense Type list
        [Authorize]
        [HttpGet]
        [Route("api/expenseType/list")]
        public List<Models.MstExpenseType> listExpenseType()
        {
            var expenseTypes = from d in db.mstExpenseTypes
                        select new Models.MstExpenseType
                        {
                            Id = d.Id,
                            ExpenseType = d.ExpenseType,
                            Description = d.Description,
                            CreatedByUserId = d.CreatedByUserId,
                            CreatedByUser = d.mstUser.FullName,
                            CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                            UpdatedByUserId = d.UpdatedByUserId,
                            UpdatedByUser = d.mstUser1.FullName,
                            UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                        };

            return expenseTypes.ToList();
        }

        // add expense Type
        [Authorize]
        [HttpPost]
        [Route("api/expenseType/add")]
        public HttpResponseMessage addExpenseType(Models.MstExpenseType expenseType)
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                Data.mstExpenseType newExpenseType = new Data.mstExpenseType();
                newExpenseType.ExpenseType = expenseType.ExpenseType;
                newExpenseType.Description = expenseType.Description;
                newExpenseType.CreatedByUserId = userId;
                newExpenseType.CreatedDateTime = DateTime.Now;
                newExpenseType.UpdatedByUserId = userId;
                newExpenseType.UpdatedDateTime = DateTime.Now;

                db.mstExpenseTypes.InsertOnSubmit(newExpenseType);
                db.SubmitChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // update expense Type
        [Authorize]
        [HttpPut]
        [Route("api/expenseType/update/{id}")]
        public HttpResponseMessage updateExpenseType(String id, Models.MstExpenseType expenseType)
        {
            try
            {
                var expenseTypes = from d in db.mstExpenseTypes where d.Id == Convert.ToInt32(id) select d;
                if (expenseTypes.Any())
                {
                    var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                    var updateExpenseType = expenseTypes.FirstOrDefault();
                    updateExpenseType.ExpenseType = expenseType.ExpenseType;
                    updateExpenseType.Description = expenseType.Description;
                    updateExpenseType.UpdatedByUserId = userId;
                    updateExpenseType.UpdatedDateTime = DateTime.Now;

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

        // delete expense Type
        [Authorize]
        [HttpDelete]
        [Route("api/expenseType/delete/{id}")]
        public HttpResponseMessage deleteExpenseType(String id)
        {
            try
            {
                var expenseTypes = from d in db.mstExpenseTypes where d.Id == Convert.ToInt32(id) select d;
                if (expenseTypes.Any())
                {
                    db.mstExpenseTypes.DeleteOnSubmit(expenseTypes.First());
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
