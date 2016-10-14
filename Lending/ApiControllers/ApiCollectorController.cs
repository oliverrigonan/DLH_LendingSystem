using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiCollectorController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // collector list
        [Authorize]
        [HttpGet]
        [Route("api/collector/list")]
        public List<Models.MstCollector> listCollector()
        {
            var collectors = from d in db.mstCollectors.OrderByDescending(d => d.Id)
                             select new Models.MstCollector
                             {
                                 Id = d.Id,
                                 Collector = d.Collector,
                                 ContactNumber = d.ContactNumber,
                                 AreaId = d.AreaId,
                                 Area = d.mstArea.Area,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.mstUser.FullName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.mstUser1.FullName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            return collectors.ToList();
        }

        // collector list by Area Id
        [Authorize]
        [HttpGet]
        [Route("api/collector/listByAreaId/{areaId}")]
        public List<Models.MstCollector> listCollectorByAreaId(String areaId)
        {
            var collectors = from d in db.mstCollectors.OrderByDescending(d => d.Id)
                             where d.AreaId == Convert.ToInt32(areaId)
                             select new Models.MstCollector
                             {
                                 Id = d.Id,
                                 Collector = d.Collector,
                                 ContactNumber = d.ContactNumber,
                                 AreaId = d.AreaId,
                                 Area = d.mstArea.Area,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.mstUser.FullName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.mstUser1.FullName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            return collectors.ToList();
        }

        // add collector 
        [Authorize]
        [HttpPost]
        [Route("api/collector/add")]
        public HttpResponseMessage addCollector(Models.MstCollector collector)
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                Data.mstCollector newCollector = new Data.mstCollector();

                newCollector.Collector = collector.Collector;
                newCollector.ContactNumber = collector.ContactNumber;
                newCollector.AreaId = collector.AreaId;
                newCollector.CreatedByUserId = userId;
                newCollector.CreatedDateTime = DateTime.Now;
                newCollector.UpdatedByUserId = userId;
                newCollector.UpdatedDateTime = DateTime.Now;

                db.mstCollectors.InsertOnSubmit(newCollector);
                db.SubmitChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // update collector
        [Authorize]
        [HttpPut]
        [Route("api/collector/update/{id}")]
        public HttpResponseMessage updateCollector(String id, Models.MstCollector collector)
        {
            try
            {
                var collectors = from d in db.mstCollectors where d.Id == Convert.ToInt32(id) select d;
                if (collectors.Any())
                {
                    var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                    var updateCollector = collectors.FirstOrDefault();

                    updateCollector.Collector = collector.Collector;
                    updateCollector.ContactNumber = collector.ContactNumber;
                    updateCollector.AreaId = collector.AreaId;
                    updateCollector.UpdatedByUserId = userId;
                    updateCollector.UpdatedDateTime = DateTime.Now;
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

        // delete collector
        [Authorize]
        [HttpDelete]
        [Route("api/collector/delete/{id}")]
        public HttpResponseMessage deleteCollector(String id)
        {
            try
            {
                var collectors = from d in db.mstCollectors where d.Id == Convert.ToInt32(id) select d;
                if (collectors.Any())
                {
                    db.mstCollectors.DeleteOnSubmit(collectors.First());
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
