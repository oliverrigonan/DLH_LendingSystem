using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiStaffController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // staff list
        [Authorize]
        [HttpGet]
        [Route("api/staff/list")]
        public List<Models.MstStaff> listStaff()
        {
            var staffs = from d in db.mstStaffs
                         select new Models.MstStaff
                         {
                             Id = d.Id,
                             StaffNumber = d.StaffNumber,
                             Staff = d.Staff,
                             ContactNumber = d.ContactNumber,
                             Address = d.Address,
                             StaffManualNumber = d.StaffManualNumber,
                             StaffRoleId = d.StaffRoleId,
                             StaffRole = d.sysStaffRole.StaffRole,
                             IsLocked = d.IsLocked,
                             CreatedByUserId = d.CreatedByUserId,
                             CreatedByUser = d.mstUser.FullName,
                             CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                             UpdatedByUserId = d.UpdatedByUserId,
                             UpdatedByUser = d.mstUser1.FullName,
                             UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                         };

            return staffs.ToList();
        }

        // staff by Id
        [Authorize]
        [HttpGet]
        [Route("api/staff/getById/{id}")]
        public Models.MstStaff getStaffById(String id)
        {
            var staffs = from d in db.mstStaffs
                         where d.Id == Convert.ToInt32(id)
                         select new Models.MstStaff
                         {
                             Id = d.Id,
                             StaffNumber = d.StaffNumber,
                             Staff = d.Staff,
                             ContactNumber = d.ContactNumber,
                             Address = d.Address,
                             StaffManualNumber = d.StaffManualNumber,
                             StaffRoleId = d.StaffRoleId,
                             StaffRole = d.sysStaffRole.StaffRole,
                             IsLocked = d.IsLocked,
                             CreatedByUserId = d.CreatedByUserId,
                             CreatedByUser = d.mstUser.FullName,
                             CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                             UpdatedByUserId = d.UpdatedByUserId,
                             UpdatedByUser = d.mstUser1.FullName,
                             UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                         };

            return (Models.MstStaff)staffs.FirstOrDefault();
        }

        // staff list by staffroleid
        [Authorize]
        [HttpGet]
        [Route("api/staff/list/byStaffRoleId/{staffRoleId}")]
        public List<Models.MstStaff> listStaffByStaffRoleId(String staffRoleId)
        {
            var staffs = from d in db.mstStaffs
                         where d.StaffRoleId == Convert.ToInt32(staffRoleId)
                         select new Models.MstStaff
                         {
                             Id = d.Id,
                             StaffNumber = d.StaffNumber,
                             Staff = d.Staff,
                             ContactNumber = d.ContactNumber,
                             Address = d.Address,
                             StaffManualNumber = d.StaffManualNumber,
                             StaffRoleId = d.StaffRoleId,
                             StaffRole = d.sysStaffRole.StaffRole,
                             IsLocked = d.IsLocked,
                             CreatedByUserId = d.CreatedByUserId,
                             CreatedByUser = d.mstUser.FullName,
                             CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                             UpdatedByUserId = d.UpdatedByUserId,
                             UpdatedByUser = d.mstUser1.FullName,
                             UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                         };

            return staffs.ToList();
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

        // add staff
        [Authorize]
        [HttpPost]
        [Route("api/staff/add")]
        public Int32 addStaff(Models.MstStaff staff)
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
                    String matchPageString = "StaffList";
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
                        String staffNumber = "0000000001";
                        var staffs = from d in db.mstStaffs.OrderByDescending(d => d.Id) select d;
                        if (staffs.Any())
                        {
                            var newStaffNumber = Convert.ToInt32(staffs.FirstOrDefault().StaffNumber) + 0000000001;
                            staffNumber = newStaffNumber.ToString();
                        }

                        Data.mstStaff newStaff = new Data.mstStaff();
                        newStaff.StaffNumber = zeroFill(Convert.ToInt32(staffNumber), 10);
                        newStaff.Staff = "NA";
                        newStaff.ContactNumber = "NA";
                        newStaff.Address = "NA";
                        newStaff.StaffManualNumber = "NA";
                        newStaff.StaffRoleId = (from d in db.sysStaffRoles select d.Id).FirstOrDefault();
                        newStaff.IsLocked = false;
                        newStaff.CreatedByUserId = userId;
                        newStaff.CreatedDateTime = DateTime.Now;
                        newStaff.UpdatedByUserId = userId;
                        newStaff.UpdatedDateTime = DateTime.Now;

                        db.mstStaffs.InsertOnSubmit(newStaff);
                        db.SubmitChanges();

                        return newStaff.Id;
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

        // lock staff
        [Authorize]
        [HttpPut]
        [Route("api/staff/lock/{id}")]
        public HttpResponseMessage lockStaff(String id, Models.MstStaff staff)
        {
            try
            {
                var staffs = from d in db.mstStaffs where d.Id == Convert.ToInt32(id) select d;
                if (staffs.Any())
                {
                    if (!staffs.FirstOrDefault().IsLocked)
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
                            String matchPageString = "StaffDetail";
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
                                var lockStaff = staffs.FirstOrDefault();
                                lockStaff.Staff = staff.Staff;
                                lockStaff.ContactNumber = staff.ContactNumber;
                                lockStaff.Address = staff.Address;
                                lockStaff.StaffManualNumber = staff.StaffManualNumber;
                                lockStaff.StaffRoleId = staff.StaffRoleId;
                                lockStaff.IsLocked = true;
                                lockStaff.UpdatedByUserId = userId;
                                lockStaff.UpdatedDateTime = DateTime.Now;
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

        // unlock staff
        [Authorize]
        [HttpPut]
        [Route("api/staff/unlock/{id}")]
        public HttpResponseMessage unlockStaff(String id, Models.MstStaff staff)
        {
            try
            {
                var staffs = from d in db.mstStaffs where d.Id == Convert.ToInt32(id) select d;
                if (staffs.Any())
                {
                    if (staffs.FirstOrDefault().IsLocked)
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
                            String matchPageString = "StaffDetail";
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
                                var unlockStaff = staffs.FirstOrDefault();
                                unlockStaff.IsLocked = false;
                                unlockStaff.UpdatedByUserId = userId;
                                unlockStaff.UpdatedDateTime = DateTime.Now;
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

        // delete staff
        [Authorize]
        [HttpDelete]
        [Route("api/staff/delete/{id}")]
        public HttpResponseMessage deleteStaff(String id)
        {
            try
            {
                var staffs = from d in db.mstStaffs where d.Id == Convert.ToInt32(id) select d;
                if (staffs.Any())
                {
                    if (!staffs.FirstOrDefault().IsLocked)
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
                            String matchPageString = "StaffDetail";
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
                                db.mstStaffs.DeleteOnSubmit(staffs.First());
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
