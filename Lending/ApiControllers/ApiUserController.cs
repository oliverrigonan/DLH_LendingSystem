using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiUserController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // current logged in user
        [Authorize]
        [HttpGet]
        [Route("api/user/get/currentLoggedInUser")]
        public Models.MstUser getCurrentLoggedInUser()
        {
            var currentUser = from d in db.mstUsers
                              where d.AspUserId == User.Identity.GetUserId()
                              select new Models.MstUser
                              {
                                  Id = d.Id,
                                  AspUserId = d.AspUserId,
                                  Username = d.Username,
                                  Password = d.Password,
                                  FullName = d.FullName,
                                  CompanyId = d.CompanyId,
                                  Company = d.mstCompany.Company,
                                  IsLocked = d.IsLocked,
                                  CreatedDate = d.CreatedDate.ToString(),
                                  UpdatedDate = d.UpdatedDate.ToString()
                              };

            return (Models.MstUser)currentUser.FirstOrDefault();
        }

        // user list
        [Authorize]
        [HttpGet]
        [Route("api/user/list")]
        public List<Models.MstUser> listUser()
        {
            var users = from d in db.mstUsers
                        select new Models.MstUser
                        {
                            Id = d.Id,
                            AspUserId = d.AspUserId,
                            Username = d.Username,
                            Password = d.Password,
                            FullName = d.FullName,
                            CompanyId = d.CompanyId,
                            Company = d.mstCompany.Company,
                            IsLocked = d.IsLocked,
                            CreatedDate = d.CreatedDate.ToString(),
                            UpdatedDate = d.UpdatedDate.ToString()
                        };

            return users.ToList();
        }

        // get user by Id
        [Authorize]
        [HttpGet]
        [Route("api/user/getById/{id}")]
        public Models.MstUser getUserById(String id)
        {
            var users = from d in db.mstUsers
                        where d.Id == Convert.ToInt32(id)
                        select new Models.MstUser
                        {
                            Id = d.Id,
                            AspUserId = d.AspUserId,
                            Username = d.Username,
                            Password = d.Password,
                            FullName = d.FullName,
                            CompanyId = d.CompanyId,
                            Company = d.mstCompany.Company,
                            IsLocked = d.IsLocked,
                            CreatedDate = d.CreatedDate.ToString(),
                            UpdatedDate = d.UpdatedDate.ToString()
                        };

            return (Models.MstUser)users.FirstOrDefault();
        }

        // lock user
        [Authorize]
        [HttpPut]
        [Route("api/user/lock/{id}")]
        public HttpResponseMessage lockUser(String id, Models.MstUser mstUser)
        {
            try
            {
                var mstUsers = from d in db.mstUsers where d.Id == Convert.ToInt32(id) select d;
                if (mstUsers.Any())
                {
                    if (!mstUsers.FirstOrDefault().IsLocked)
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
                            String matchPageString = "UserDetail";
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
                                var lockUser = mstUsers.FirstOrDefault();
                                lockUser.CompanyId = mstUser.CompanyId;
                                lockUser.IsLocked = true;
                                lockUser.UpdatedDate = DateTime.Now;
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

        // unlock user
        [Authorize]
        [HttpPut]
        [Route("api/user/unlock/{id}")]
        public HttpResponseMessage unlockUser(String id, Models.MstUser mstUser)
        {
            try
            {
                var mstUsers = from d in db.mstUsers where d.Id == Convert.ToInt32(id) select d;
                if (mstUsers.Any())
                {
                    if (mstUsers.FirstOrDefault().IsLocked)
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
                            String matchPageString = "UserDetail";
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
                                var unlockUser = mstUsers.FirstOrDefault();
                                unlockUser.IsLocked = false;
                                unlockUser.UpdatedDate = DateTime.Now;
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

        // update user
        [Authorize]
        [HttpPut]
        [Route("api/user/update/currentLoggedInUser")]
        public HttpResponseMessage updateCurrentLoggedInUser(Models.MstUser user)
        {
            try
            {
                var currentUser = from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d;
                if (currentUser.Any())
                {
                    var updateUser = currentUser.FirstOrDefault();
                    updateUser.FullName = user.FullName;
                    updateUser.UpdatedDate = DateTime.Now;
                    db.SubmitChanges();

                    var currentAspUser = from d in db.AspNetUsers where d.Id == User.Identity.GetUserId() select d;
                    if (currentAspUser.Any())
                    {
                        var updateAspUser = currentAspUser.FirstOrDefault();
                        updateAspUser.FullName = user.FullName;
                        db.SubmitChanges();
                    }

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
