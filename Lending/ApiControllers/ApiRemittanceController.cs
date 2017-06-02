using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiRemittanceController : ApiController
    {
        private Data.LendingDataContext db = new Data.LendingDataContext();

        [Authorize]
        [HttpGet]
        [Route("api/remmittance/list/{startDate}/{endDate}")]
        public List<Models.TrnRemittance> listRemmittance(String startDate, String endDate)
        {
            var remmitance = from d in db.trnRemittances.OrderByDescending(d => d.Id)
                             where d.RemittanceDate >= Convert.ToDateTime(startDate)
                             && d.RemittanceDate <= Convert.ToDateTime(endDate)
                             select new Models.TrnRemittance
                             {
                                 Id = d.Id,
                                 RemittanceNumber = d.RemittanceNumber,
                                 RemittanceDate = d.RemittanceDate.ToShortDateString(),
                                 AreaId = d.AreaId,
                                 Area = d.mstArea.Area,
                                 StaffId = d.StaffId,
                                 Staff = d.mstStaff.Staff,
                                 Particulars = d.Particulars,
                                 PreparedByUserId = d.PreparedByUserId,
                                 PreparedByUser = d.mstUser.FullName,
                                 RemitAmount = d.RemitAmount,
                                 IsLocked = d.IsLocked,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.mstUser1.FullName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.mstUser2.FullName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            return remmitance.ToList();
        }

        [Authorize]
        [HttpGet]
        [Route("api/remmittance/get/{id}")]
        public Models.TrnRemittance listRemmittance(String id)
        {
            var remmitance = from d in db.trnRemittances
                             where d.Id == Convert.ToInt32(id)
                             select new Models.TrnRemittance
                             {
                                 Id = d.Id,
                                 RemittanceNumber = d.RemittanceNumber,
                                 RemittanceDate = d.RemittanceDate.ToShortDateString(),
                                 AreaId = d.AreaId,
                                 Area = d.mstArea.Area,
                                 StaffId = d.StaffId,
                                 Staff = d.mstStaff.Staff,
                                 Particulars = d.Particulars,
                                 PreparedByUserId = d.PreparedByUserId,
                                 PreparedByUser = d.mstUser.FullName,
                                 RemitAmount = d.RemitAmount,
                                 IsLocked = d.IsLocked,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.mstUser1.FullName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.mstUser2.FullName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            return (Models.TrnRemittance)remmitance.FirstOrDefault();
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

        // add remittance
        [Authorize]
        [HttpPost]
        [Route("api/remittance/add")]
        public Int32 addLockRemittance()
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
                    String matchPageString = "RemittanceList";
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
                        String remittanceNumber = "0000000001";
                        var remittances = from d in db.trnRemittances.OrderByDescending(d => d.Id) select d;
                        if (remittances.Any())
                        {
                            var newRemittanceNumber = Convert.ToInt32(remittances.FirstOrDefault().RemittanceNumber) + 0000000001;
                            remittanceNumber = newRemittanceNumber.ToString();
                        }

                        var area = from d in db.mstAreas.OrderByDescending(d => d.Id)
                                   select d;

                        if (area.Any())
                        {
                            var staff = from d in db.mstStaffs.OrderByDescending(d => d.Id)
                                        select d;

                            if (staff.Any())
                            {
                                Data.trnRemittance newRemittance = new Data.trnRemittance();
                                newRemittance.RemittanceNumber = zeroFill(Convert.ToInt32(remittanceNumber), 10);
                                newRemittance.RemittanceDate = DateTime.Today;
                                newRemittance.AreaId = area.FirstOrDefault().Id;
                                newRemittance.StaffId = staff.FirstOrDefault().Id;
                                newRemittance.Particulars = "NA";
                                newRemittance.PreparedByUserId = userId;
                                newRemittance.RemitAmount = 0;
                                newRemittance.IsLocked = false;
                                newRemittance.CreatedByUserId = userId;
                                newRemittance.CreatedDateTime = DateTime.Now;
                                newRemittance.UpdatedByUserId = userId;
                                newRemittance.UpdatedDateTime = DateTime.Now;

                                db.trnRemittances.InsertOnSubmit(newRemittance);
                                db.SubmitChanges();

                                return newRemittance.Id;
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

        // lock remittance
        [Authorize]
        [HttpPut]
        [Route("api/remittance/lock/{id}")]
        public HttpResponseMessage lockRemittance(String id, Models.TrnRemittance remittance)
        {
            try
            {
                var remittances = from d in db.trnRemittances where d.Id == Convert.ToInt32(id) select d;
                if (remittances.Any())
                {
                    if (!remittances.FirstOrDefault().IsLocked)
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
                            String matchPageString = "RemittanceDetail";
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
                                var lockRemittance = remittances.FirstOrDefault();
                                lockRemittance.RemittanceDate = Convert.ToDateTime(remittance.RemittanceDate);
                                lockRemittance.AreaId = remittance.AreaId;
                                lockRemittance.StaffId = remittance.StaffId;
                                lockRemittance.Particulars = remittance.Particulars;
                                lockRemittance.RemitAmount = remittance.RemitAmount;
                                lockRemittance.IsLocked = true;
                                lockRemittance.UpdatedByUserId = userId;
                                lockRemittance.UpdatedDateTime = DateTime.Now;
                                
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

        // unlock remittance
        [Authorize]
        [HttpPut]
        [Route("api/remittance/unlock/{id}")]
        public HttpResponseMessage unlockRemittance(String id, Models.TrnRemittance remittance)
        {
            try
            {
                var remittances = from d in db.trnRemittances where d.Id == Convert.ToInt32(id) select d;
                if (remittances.Any())
                {
                    if (remittances.FirstOrDefault().IsLocked)
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
                            String matchPageString = "RemittanceDetail";
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
                                var unlockRemittance = remittances.FirstOrDefault();
                                unlockRemittance.IsLocked = false;
                                unlockRemittance.UpdatedByUserId = userId;
                                unlockRemittance.UpdatedDateTime = DateTime.Now;
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

        // delete remittance
        [Authorize]
        [HttpDelete]
        [Route("api/remittance/delete/{id}")]
        public HttpResponseMessage deleteRemittance(String id)
        {
            try
            {
                var remittances = from d in db.trnRemittances where d.Id == Convert.ToInt32(id) select d;
                if (remittances.Any())
                {
                    if (!remittances.FirstOrDefault().IsLocked)
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
                            String matchPageString = "RemittanceList";
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
                                db.trnRemittances.DeleteOnSubmit(remittances.First());
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
