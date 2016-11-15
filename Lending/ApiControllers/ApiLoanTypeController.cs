using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiLoanTypeController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan Types list
        [Authorize]
        [HttpGet]
        [Route("api/loanTypes/list")]
        public List<Models.MstLoanType> listLoanTypes()
        {
            var loanTypes = from d in db.mstLoanTypes.OrderByDescending(d => d.Id)
                            select new Models.MstLoanType
                            {
                                Id = d.Id,
                                LoanType = d.LoanType,
                                Description = d.Description,
                                CreatedByUserId = d.CreatedByUserId,
                                CreatedByUser = d.mstUser.FullName,
                                CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                UpdatedByUserId = d.UpdatedByUserId,
                                UpdatedByUser = d.mstUser1.FullName,
                                UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                            };

            return loanTypes.ToList();
        }

        // add loan Types 
        [Authorize]
        [HttpPost]
        [Route("api/loanTypes/add")]
        public HttpResponseMessage addLoanTypes(Models.MstLoanType loanType)
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                Data.mstLoanType newLoanType = new Data.mstLoanType();

                newLoanType.LoanType = loanType.LoanType;
                newLoanType.Description = loanType.Description;
                newLoanType.CreatedByUserId = userId;
                newLoanType.CreatedDateTime = DateTime.Now;
                newLoanType.UpdatedByUserId = userId;
                newLoanType.UpdatedDateTime = DateTime.Now;

                db.mstLoanTypes.InsertOnSubmit(newLoanType);
                db.SubmitChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // update loan Types 
        [Authorize]
        [HttpPut]
        [Route("api/loanTypes/update/{id}")]
        public HttpResponseMessage updateLoanTypes(String id, Models.MstLoanType loanType)
        {
            try
            {
                var loanTypes = from d in db.mstLoanTypes where d.Id == Convert.ToInt32(id) select d;
                if (loanTypes.Any())
                {
                    var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                    var updateLoanType = loanTypes.FirstOrDefault();

                    updateLoanType.LoanType = loanType.LoanType;
                    updateLoanType.Description = loanType.Description;
                    updateLoanType.UpdatedByUserId = userId;
                    updateLoanType.UpdatedDateTime = DateTime.Now;
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

        // delete loan Types
        [Authorize]
        [HttpDelete]
        [Route("api/loanTypes/delete/{id}")]
        public HttpResponseMessage deleteLoanTypes(String id)
        {
            try
            {
                var loanTypes = from d in db.mstLoanTypes where d.Id == Convert.ToInt32(id) select d;
                if (loanTypes.Any())
                {
                    db.mstLoanTypes.DeleteOnSubmit(loanTypes.First());
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
