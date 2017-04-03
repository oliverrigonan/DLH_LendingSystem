using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiUserFormsController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // user form list
        [Authorize]
        [HttpGet]
        [Route("api/userForm/listByUserId/{userId}")]
        public List<Models.MstUserForm> listUserFormByUserId(String userId)
        {
            var userForms = from d in db.mstUserForms.OrderByDescending(d => d.Id)
                            where d.UserId == Convert.ToInt32(userId)
                            select new Models.MstUserForm
                            {
                                Id = d.Id,
                                UserId = d.UserId,
                                User = d.mstUser.FullName,
                                FormId = d.FormId,
                                FormDescription = d.sysForm.FormDescription,
                                CanPerformActions = d.CanPerformActions,
                            };

            return userForms.ToList();
        }

        // add user form
        [Authorize]
        [HttpPost]
        [Route("api/userForm/add")]
        public HttpResponseMessage addUserForm(Models.MstUserForm userForm)
        {
            try
            {
                var users = from d in db.mstUsers where d.Id == userForm.UserId select d;
                if (users.Any())
                {
                    if (!users.FirstOrDefault().IsLocked)
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
                                // query to check if the form id already exist in the table
                                var existForm = from d in db.mstUserForms
                                                where d.FormId == userForm.FormId
                                                && d.UserId == userForm.UserId
                                                select d;

                                if (!existForm.Any())
                                {
                                    Data.mstUserForm newUserForm = new Data.mstUserForm();
                                    newUserForm.UserId = userForm.UserId;
                                    newUserForm.FormId = userForm.FormId;
                                    newUserForm.CanPerformActions = userForm.CanPerformActions;
                                    db.mstUserForms.InsertOnSubmit(newUserForm);
                                    db.SubmitChanges();

                                    return Request.CreateResponse(HttpStatusCode.OK);
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The form already exist.");
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

        // update user form
        [Authorize]
        [HttpPut]
        [Route("api/userForm/update/{id}")]
        public HttpResponseMessage updateUserForm(String id, Models.MstUserForm userForm)
        {
            try
            {
                var users = from d in db.mstUsers where d.Id == userForm.UserId select d;
                if (users.Any())
                {
                    if (!users.FirstOrDefault().IsLocked)
                    {
                        var userForms = from d in db.mstUserForms where d.Id == Convert.ToInt32(id) select d;
                        if (userForms.Any())
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
                                    var updateUserForm = userForms.FirstOrDefault();
                                    updateUserForm.UserId = userForm.UserId;
                                    updateUserForm.FormId = userForm.FormId;
                                    updateUserForm.CanPerformActions = userForm.CanPerformActions;

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

        // delete user form
        [Authorize]
        [HttpDelete]
        [Route("api/userForm/delete/{id}")]
        public HttpResponseMessage deleteUserForm(String id)
        {
            try
            {
                var userForms = from d in db.mstUserForms where d.Id == Convert.ToInt32(id) select d;
                if (userForms.Any())
                {
                    var users = from d in db.mstUsers where d.Id == userForms.FirstOrDefault().UserId select d;
                    if (!users.FirstOrDefault().IsLocked)
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
                                db.mstUserForms.DeleteOnSubmit(userForms.First());
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

        // copy rights user form
        [Authorize]
        [HttpPost]
        [Route("api/userForm/copyRights/{urlUserId}/{copryRightsUserId}")]
        public HttpResponseMessage copyRightssUserForm(String urlUserId, String copryRightsUserId)
        {
            try
            {
                var users = from d in db.mstUsers where d.Id == Convert.ToInt32(urlUserId) select d;
                if (users.Any())
                {
                    if (!users.FirstOrDefault().IsLocked)
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
                                // forms
                                var userForms = from d in db.mstUserForms
                                                where d.UserId == Convert.ToInt32(copryRightsUserId)
                                                select new Models.MstUserForm
                                                {
                                                    UserId = d.UserId,
                                                    FormId = d.FormId,
                                                    CanPerformActions = d.CanPerformActions
                                                };

                                if (userForms.Any())
                                {
                                    foreach (var userForm in userForms)
                                    {
                                        // query to check if the form id already exist in the table
                                        var existForm = from d in db.mstUserForms
                                                        where d.FormId == userForm.FormId
                                                        && d.UserId == Convert.ToInt32(urlUserId)
                                                        select d;

                                        if (!existForm.Any())
                                        {
                                            Data.mstUserForm newMstUserForm = new Data.mstUserForm();
                                            newMstUserForm.UserId = Convert.ToInt32(urlUserId);
                                            newMstUserForm.FormId = userForm.FormId;
                                            newMstUserForm.CanPerformActions = userForm.CanPerformActions;
                                            db.mstUserForms.InsertOnSubmit(newMstUserForm);
                                            db.SubmitChanges();
                                        }
                                    }
                                }

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
