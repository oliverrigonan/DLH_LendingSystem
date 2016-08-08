using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiAreaController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // area list
        [Authorize]
        [HttpGet]
        [Route("api/area/list")]
        public List<Models.MstArea> listArea()
        {
            var areas = from d in db.mstAreas.OrderByDescending(d => d.Id)
                        select new Models.MstArea
                        {
                            Id = d.Id,
                            Area = d.Area,
                            Description = d.Description,
                            CreatedByUserId = d.CreatedByUserId,
                            CreatedByUser = d.mstUser.FirstName + " " + d.mstUser.MiddleName + " " + d.mstUser.LastName,
                            CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                            UpdatedByUserId = d.UpdatedByUserId,
                            UpdatedByUser = d.mstUser1.FirstName + " " + d.mstUser1.MiddleName + " " + d.mstUser1.LastName,
                            UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                        };

            return areas.ToList();
        }

        // add area
        [Authorize]
        [HttpPost]
        [Route("api/area/add")]
        public HttpResponseMessage addArea(Models.MstArea area)
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                Data.mstArea newArea = new Data.mstArea();
                newArea.Area = area.Area;
                newArea.Description = area.Description;
                newArea.CreatedByUserId = userId;
                newArea.CreatedDateTime = DateTime.Now;
                newArea.UpdatedByUserId = userId;
                newArea.UpdatedDateTime = DateTime.Now;

                db.mstAreas.InsertOnSubmit(newArea);
                db.SubmitChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // update area
        [Authorize]
        [HttpPut]
        [Route("api/area/update/{id}")]
        public HttpResponseMessage updateArea(String id, Models.MstArea area)
        {
            try
            {
                var areas = from d in db.mstAreas where d.Id == Convert.ToInt32(id) select d;
                if (areas.Any())
                {
                    var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                    var updateArea = areas.FirstOrDefault();
                    updateArea.Area = area.Area;
                    updateArea.Description = area.Description;
                    updateArea.UpdatedByUserId = userId;
                    updateArea.UpdatedDateTime = DateTime.Now;

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

        // delete area
        [Authorize]
        [HttpDelete]
        [Route("api/area/delete/{id}")]
        public HttpResponseMessage deleteArea(String id)
        {
            try
            {
                var areas = from d in db.mstAreas where d.Id == Convert.ToInt32(id) select d;
                if (areas.Any())
                {
                    db.mstAreas.DeleteOnSubmit(areas.First());
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
