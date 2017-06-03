using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiVaultController : ApiController
    {
        private Data.LendingDataContext db = new Data.LendingDataContext();

        [Authorize]
        [HttpGet]
        [Route("api/vault/list/{startDate}/{endDate}")]
        public List<Models.TrnVault> listVault(String startDate, String endDate)
        {
            var remmitance = from d in db.trnVaults.OrderByDescending(d => d.Id)
                             where d.VaultDate >= Convert.ToDateTime(startDate)
                             && d.VaultDate <= Convert.ToDateTime(endDate)
                             select new Models.TrnVault
                             {
                                 Id = d.Id,
                                 VaultNumber = d.VaultNumber,
                                 VaultDate = d.VaultDate.ToShortDateString(),
                                 StaffId = d.StaffId,
                                 Staff = d.mstStaff.Staff,
                                 Particulars = d.Particulars,
                                 PreparedByUserId = d.PreparedByUserId,
                                 PreparedByUser = d.mstUser.FullName,
                                 Amount = d.Amount,
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
        [Route("api/vault/get/{id}")]
        public Models.TrnVault listVault(String id)
        {
            var remmitance = from d in db.trnVaults
                             where d.Id == Convert.ToInt32(id)
                             select new Models.TrnVault
                             {
                                 Id = d.Id,
                                 VaultNumber = d.VaultNumber,
                                 VaultDate = d.VaultDate.ToShortDateString(),
                                 StaffId = d.StaffId,
                                 Staff = d.mstStaff.Staff,
                                 Particulars = d.Particulars,
                                 PreparedByUserId = d.PreparedByUserId,
                                 PreparedByUser = d.mstUser.FullName,
                                 Amount = d.Amount,
                                 IsLocked = d.IsLocked,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.mstUser1.FullName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.mstUser2.FullName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            return (Models.TrnVault)remmitance.FirstOrDefault();
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

        // add vault
        [Authorize]
        [HttpPost]
        [Route("api/vault/add")]
        public Int32 addLockVault()
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
                    String matchPageString = "VaultList";
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
                        String vaultNumber = "0000000001";
                        var vaults = from d in db.trnVaults.OrderByDescending(d => d.Id) select d;
                        if (vaults.Any())
                        {
                            var newVaultNumber = Convert.ToInt32(vaults.FirstOrDefault().VaultNumber) + 0000000001;
                            vaultNumber = newVaultNumber.ToString();
                        }

                        var staff = from d in db.mstStaffs.OrderByDescending(d => d.Id)
                                    select d;

                        if (staff.Any())
                        {
                            Data.trnVault newVault = new Data.trnVault();
                            newVault.VaultNumber = zeroFill(Convert.ToInt32(vaultNumber), 10);
                            newVault.VaultDate = DateTime.Today;
                            newVault.StaffId = staff.FirstOrDefault().Id;
                            newVault.Particulars = "NA";
                            newVault.PreparedByUserId = userId;
                            newVault.Amount = 0;
                            newVault.IsLocked = false;
                            newVault.CreatedByUserId = userId;
                            newVault.CreatedDateTime = DateTime.Now;
                            newVault.UpdatedByUserId = userId;
                            newVault.UpdatedDateTime = DateTime.Now;

                            db.trnVaults.InsertOnSubmit(newVault);
                            db.SubmitChanges();

                            return newVault.Id;
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

        // lock vault
        [Authorize]
        [HttpPut]
        [Route("api/vault/lock/{id}")]
        public HttpResponseMessage lockVault(String id, Models.TrnVault vault)
        {
            try
            {
                var vaults = from d in db.trnVaults where d.Id == Convert.ToInt32(id) select d;
                if (vaults.Any())
                {
                    if (!vaults.FirstOrDefault().IsLocked)
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
                            String matchPageString = "VaultDetail";
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
                                var lockVault = vaults.FirstOrDefault();
                                lockVault.VaultDate = Convert.ToDateTime(vault.VaultDate);
                                lockVault.StaffId = vault.StaffId;
                                lockVault.Particulars = vault.Particulars;
                                lockVault.Amount = vault.Amount;
                                lockVault.IsLocked = true;
                                lockVault.UpdatedByUserId = userId;
                                lockVault.UpdatedDateTime = DateTime.Now;

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

        // unlock vault
        [Authorize]
        [HttpPut]
        [Route("api/vault/unlock/{id}")]
        public HttpResponseMessage unlockVault(String id, Models.TrnVault vault)
        {
            try
            {
                var vaults = from d in db.trnVaults where d.Id == Convert.ToInt32(id) select d;
                if (vaults.Any())
                {
                    if (vaults.FirstOrDefault().IsLocked)
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
                            String matchPageString = "VaultDetail";
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
                                var unlockVault = vaults.FirstOrDefault();
                                unlockVault.IsLocked = false;
                                unlockVault.UpdatedByUserId = userId;
                                unlockVault.UpdatedDateTime = DateTime.Now;
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

        // delete vault
        [Authorize]
        [HttpDelete]
        [Route("api/vault/delete/{id}")]
        public HttpResponseMessage deleteVault(String id)
        {
            try
            {
                var vaults = from d in db.trnVaults where d.Id == Convert.ToInt32(id) select d;
                if (vaults.Any())
                {
                    if (!vaults.FirstOrDefault().IsLocked)
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
                            String matchPageString = "VaultList";
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
                                db.trnVaults.DeleteOnSubmit(vaults.First());
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
