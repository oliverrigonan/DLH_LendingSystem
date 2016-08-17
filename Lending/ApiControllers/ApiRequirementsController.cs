using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Diagnostics;

namespace Lending.ApiControllers
{
    public class ApiRequirementsController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan requirements list
        [Authorize]
        [HttpGet]
        [Route("api/requirements/list")]
        public List<Models.MstRequirements> listRequirements()
        {
            var requirements = from d in db.mstRequirements.OrderByDescending(d => d.Id)
                             select new Models.MstRequirements
                             {
                                 Id = d.Id,
                                 Requirement = d.Requirement,
                                 Description = d.Description,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.mstUser.FullName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.mstUser1.FullName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            return requirements.ToList();
        }

        // add loan requirements 
        [Authorize]
        [HttpPost]
        [Route("api/requirements/add")]
        public HttpResponseMessage addRequirements(Models.MstRequirements requirement)
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                Data.mstRequirement newRequirement = new Data.mstRequirement();

                newRequirement.Requirement = requirement.Requirement;
                newRequirement.Description = requirement.Description;
                newRequirement.CreatedByUserId = userId;
                newRequirement.CreatedDateTime = DateTime.Now;
                newRequirement.UpdatedByUserId = userId;
                newRequirement.UpdatedDateTime = DateTime.Now;

                db.mstRequirements.InsertOnSubmit(newRequirement);
                db.SubmitChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // update loan requirements 
        [Authorize]
        [HttpPut]
        [Route("api/requirements/update/{id}")]
        public HttpResponseMessage updateRequirements(String id, Models.MstRequirements requirement)
        {
            try
            {
                var requirements = from d in db.mstRequirements where d.Id == Convert.ToInt32(id) select d;
                if (requirements.Any())
                {
                    var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                    var updateRequirement = requirements.FirstOrDefault();

                    updateRequirement.Requirement = requirement.Requirement;
                    updateRequirement.Description = requirement.Description;
                    updateRequirement.UpdatedByUserId = userId;
                    updateRequirement.UpdatedDateTime = DateTime.Now;
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

        // delete loan requirements
        [Authorize]
        [HttpDelete]
        [Route("api/requirements/delete/{id}")]
        public HttpResponseMessage deleteRequirements(String id)
        {
            try
            {
                var requirements = from d in db.mstRequirements where d.Id == Convert.ToInt32(id) select d;
                if (requirements.Any())
                {
                    db.mstRequirements.DeleteOnSubmit(requirements.First());
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
