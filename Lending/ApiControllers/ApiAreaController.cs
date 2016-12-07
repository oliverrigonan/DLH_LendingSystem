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
            var areas = from d in db.mstAreas
                        select new Models.MstArea
                        {
                            Id = d.Id,
                            AreaNumber = d.AreaNumber,
                            Area = d.Area,
                            Description = d.Description,
                            SupervisorStaffId = d.SupervisorStaffId,
                            SupervisorStaff = d.mstStaff.Staff,
                            CollectorStaffId = d.CollectorStaffId,
                            CollectorStaff = d.mstStaff1.Staff,
                            IsLocked = d.IsLocked,
                            CreatedByUserId = d.CreatedByUserId,
                            CreatedByUser = d.mstUser.FullName,
                            CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                            UpdatedByUserId = d.UpdatedByUserId,
                            UpdatedByUser = d.mstUser1.FullName,
                            UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                        };

            return areas.ToList();
        }

        // area by Id
        [Authorize]
        [HttpGet]
        [Route("api/area/getById/{id}")]
        public Models.MstArea getAreaById(String id)
        {
            var areas = from d in db.mstAreas
                        where d.Id == Convert.ToInt32(id)
                        select new Models.MstArea
                        {
                            Id = d.Id,
                            AreaNumber = d.AreaNumber,
                            Area = d.Area,
                            Description = d.Description,
                            SupervisorStaffId = d.SupervisorStaffId,
                            SupervisorStaff = d.mstStaff.Staff,
                            CollectorStaffId = d.CollectorStaffId,
                            CollectorStaff = d.mstStaff1.Staff,
                            IsLocked = d.IsLocked,
                            CreatedByUserId = d.CreatedByUserId,
                            CreatedByUser = d.mstUser.FullName,
                            CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                            UpdatedByUserId = d.UpdatedByUserId,
                            UpdatedByUser = d.mstUser1.FullName,
                            UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                        };

            return (Models.MstArea)areas.FirstOrDefault();
        }

        // zero fill
        public String zeroFill(Int32 number, Int32 length)
        {
            var result = number.ToString();
            var pad = length - result.Length;
            while (pad > 0)
            {
                result = "0" + result;
                pad--;
            }

            return result;
        }

        // add area
        [Authorize]
        [HttpPost]
        [Route("api/area/add")]
        public Int32 addArea(Models.MstArea area)
        {
            try
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
                    String matchPageString = "AreaList";
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
                        String areaNumber = "0000000001";
                        var areas = from d in db.mstAreas.OrderByDescending(d => d.Id) select d;
                        if (areas.Any())
                        {
                            var newAreaNumber = Convert.ToInt32(areas.FirstOrDefault().AreaNumber) + 0000000001;
                            areaNumber = newAreaNumber.ToString();
                        }

                        Data.mstArea newArea = new Data.mstArea();
                        newArea.AreaNumber = zeroFill(Convert.ToInt32(areaNumber), 10);
                        newArea.Area = "NA";
                        newArea.Description = "NA";
                        newArea.SupervisorStaffId = (from d in db.mstStaffs select d.Id).FirstOrDefault();
                        newArea.CollectorStaffId = (from d in db.mstStaffs select d.Id).FirstOrDefault();
                        newArea.IsLocked = false;
                        newArea.CreatedByUserId = userId;
                        newArea.CreatedDateTime = DateTime.Now;
                        newArea.UpdatedByUserId = userId;
                        newArea.UpdatedDateTime = DateTime.Now;

                        db.mstAreas.InsertOnSubmit(newArea);
                        db.SubmitChanges();

                        return newArea.Id;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        // lock area
        [Authorize]
        [HttpPut]
        [Route("api/area/lock/{id}")]
        public HttpResponseMessage lockArea(String id, Models.MstArea area)
        {
            try
            {
                var areas = from d in db.mstAreas where d.Id == Convert.ToInt32(id) select d;
                if (areas.Any())
                {
                    if (!areas.FirstOrDefault().IsLocked)
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
                                var lockArea = areas.FirstOrDefault();
                                lockArea.Area = area.Area;
                                lockArea.Description = area.Description;
                                lockArea.SupervisorStaffId = area.SupervisorStaffId;
                                lockArea.CollectorStaffId = area.CollectorStaffId;
                                lockArea.IsLocked = true;
                                lockArea.UpdatedByUserId = userId;
                                lockArea.UpdatedDateTime = DateTime.Now;
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

        // unlock area
        [Authorize]
        [HttpPut]
        [Route("api/area/unlock/{id}")]
        public HttpResponseMessage unlockArea(String id, Models.MstArea area)
        {
            try
            {
                var areas = from d in db.mstAreas where d.Id == Convert.ToInt32(id) select d;
                if (areas.Any())
                {
                    if (areas.FirstOrDefault().IsLocked)
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
                                var unlockArea = areas.FirstOrDefault();
                                unlockArea.IsLocked = false;
                                unlockArea.UpdatedByUserId = userId;
                                unlockArea.UpdatedDateTime = DateTime.Now;
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
                    if (!areas.FirstOrDefault().IsLocked)
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
                                db.mstAreas.DeleteOnSubmit(areas.First());
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
    }
}
