using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiAreaStaffController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // area staff list
        [Authorize]
        [HttpGet]
        [Route("api/areaStaff/listByAreaId/{areaId}")]
        public List<Models.MstAreaStaff> listAreaStaffByAreaId(String areaId)
        {
            var areaStaffs = from d in db.mstAreaStaffs
                             where d.AreaId == Convert.ToInt32(areaId)
                             select new Models.MstAreaStaff
                             {
                                 Id = d.Id,
                                 StaffId = d.StaffId,
                                 Staff = d.mstStaff.Staff
                             };

            return areaStaffs.ToList();
        }

        // add area staff
        [Authorize]
        [HttpPost]
        [Route("api/areaStaff/add")]
        public HttpResponseMessage addAreaStaff(Models.MstAreaStaff areaStaff)
        {
            try
            {
                var area = from d in db.mstAreas where d.Id == areaStaff.AreaId select d;
                if (area.Any())
                {
                    if (!area.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                        var mstUserForms = from d in db.mstUserForms
                                           where d.UserId == userId
                                           select new Models.MstUserForm
                                           {
                                               Id = d.Id,
                                               Form = d.sysForm.Form,
                                               CanPerformActions = d.CanPerformActions
                                           };

                        if (mstUserForms.Any())
                        {
                            String matchPageString = "AreaDetail";
                            Boolean canPerformActions = false;

                            foreach (var mstUserForm in mstUserForms)
                            {
                                if (mstUserForm.Form.Equals(matchPageString))
                                {
                                    if (mstUserForm.CanPerformActions)
                                    {
                                        canPerformActions = true;
                                    }

                                    break;
                                }
                            }

                            if (canPerformActions)
                            {
                                Data.mstAreaStaff newAreaStaff = new Data.mstAreaStaff();
                                newAreaStaff.AreaId = areaStaff.AreaId;
                                newAreaStaff.StaffId = areaStaff.StaffId;
                                db.mstAreaStaffs.InsertOnSubmit(newAreaStaff);
                                db.SubmitChanges();

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
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

        // update area staff
        [Authorize]
        [HttpPut]
        [Route("api/areaStaff/update/{id}")]
        public HttpResponseMessage updateAreaStaff(String id, Models.MstAreaStaff areaStaff)
        {
            try
            {
                var area = from d in db.mstAreas where d.Id == areaStaff.AreaId select d;
                if (area.Any())
                {
                    if (!area.FirstOrDefault().IsLocked)
                    {
                        var areaStaffs = from d in db.mstAreaStaffs where d.Id == Convert.ToInt32(id) select d;
                        if (areaStaffs.Any())
                        {
                            var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                            var mstUserForms = from d in db.mstUserForms
                                               where d.UserId == userId
                                               select new Models.MstUserForm
                                               {
                                                   Id = d.Id,
                                                   Form = d.sysForm.Form,
                                                   CanPerformActions = d.CanPerformActions
                                               };

                            if (mstUserForms.Any())
                            {
                                String matchPageString = "AreaDetail";
                                Boolean canPerformActions = false;

                                foreach (var mstUserForm in mstUserForms)
                                {
                                    if (mstUserForm.Form.Equals(matchPageString))
                                    {
                                        if (mstUserForm.CanPerformActions)
                                        {
                                            canPerformActions = true;
                                        }

                                        break;
                                    }
                                }

                                if (canPerformActions)
                                {
                                    var updateAreaStaff = areaStaffs.FirstOrDefault();
                                    updateAreaStaff.StaffId = areaStaff.StaffId;
                                    db.SubmitChanges();

                                    return Request.CreateResponse(HttpStatusCode.OK);
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
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

        // delete area staff
        [Authorize]
        [HttpDelete]
        [Route("api/areaStaff/delete/{id}")]
        public HttpResponseMessage deleteAreaStaff(String id)
        {
            try
            {
                var areaStaffs = from d in db.mstAreaStaffs where d.Id == Convert.ToInt32(id) select d;
                if (areaStaffs.Any())
                {
                    var area = from d in db.mstAreas where d.Id == areaStaffs.FirstOrDefault().AreaId select d;
                    if (area.Any())
                    {
                        if (!area.FirstOrDefault().IsLocked)
                        {
                            var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                            var mstUserForms = from d in db.mstUserForms
                                               where d.UserId == userId
                                               select new Models.MstUserForm
                                               {
                                                   Id = d.Id,
                                                   Form = d.sysForm.Form,
                                                   CanPerformActions = d.CanPerformActions
                                               };

                            if (mstUserForms.Any())
                            {
                                String matchPageString = "AreaDetail";
                                Boolean canPerformActions = false;

                                foreach (var mstUserForm in mstUserForms)
                                {
                                    if (mstUserForm.Form.Equals(matchPageString))
                                    {
                                        if (mstUserForm.CanPerformActions)
                                        {
                                            canPerformActions = true;
                                        }

                                        break;
                                    }
                                }

                                if (canPerformActions)
                                {
                                    db.mstAreaStaffs.DeleteOnSubmit(areaStaffs.First());
                                    db.SubmitChanges();

                                    return Request.CreateResponse(HttpStatusCode.OK);
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
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
