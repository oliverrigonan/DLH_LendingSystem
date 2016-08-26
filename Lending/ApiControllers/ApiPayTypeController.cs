using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiPayTypeController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // pay type list
        [Authorize]
        [HttpGet]
        [Route("api/payType/list")]
        public List<Models.MstPayType> listPayType()
        {
            var payTypes = from d in db.mstPayTypes.OrderByDescending(d => d.Id)
                        select new Models.MstPayType
                        {
                            Id = d.Id,
                            PayType = d.PayType,
                            Description = d.Description,
                            CreatedByUserId = d.CreatedByUserId,
                            CreatedByUser = d.mstUser.FullName,
                            CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                            UpdatedByUserId = d.UpdatedByUserId,
                            UpdatedByUser = d.mstUser1.FullName,
                            UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                        };

            return payTypes.ToList();
        }

        // add pay type
        [Authorize]
        [HttpPost]
        [Route("api/payType/add")]
        public HttpResponseMessage addPayType(Models.MstPayType area)
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                Data.mstPayType newPayType = new Data.mstPayType();
                newPayType.PayType = area.PayType;
                newPayType.Description = area.Description;
                newPayType.CreatedByUserId = userId;
                newPayType.CreatedDateTime = DateTime.Now;
                newPayType.UpdatedByUserId = userId;
                newPayType.UpdatedDateTime = DateTime.Now;

                db.mstPayTypes.InsertOnSubmit(newPayType);
                db.SubmitChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // update pay type
        [Authorize]
        [HttpPut]
        [Route("api/payType/update/{id}")]
        public HttpResponseMessage updatePayType(String id, Models.MstPayType area)
        {
            try
            {
                var payTypes = from d in db.mstPayTypes where d.Id == Convert.ToInt32(id) select d;
                if (payTypes.Any())
                {
                    var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                    var updatePayType = payTypes.FirstOrDefault();
                    updatePayType.PayType = area.PayType;
                    updatePayType.Description = area.Description;
                    updatePayType.UpdatedByUserId = userId;
                    updatePayType.UpdatedDateTime = DateTime.Now;

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

        // delete pay type
        [Authorize]
        [HttpDelete]
        [Route("api/payType/delete/{id}")]
        public HttpResponseMessage deletePayType(String id)
        {
            try
            {
                var payTypes = from d in db.mstPayTypes where d.Id == Convert.ToInt32(id) select d;
                if (payTypes.Any())
                {
                    db.mstPayTypes.DeleteOnSubmit(payTypes.First());
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
