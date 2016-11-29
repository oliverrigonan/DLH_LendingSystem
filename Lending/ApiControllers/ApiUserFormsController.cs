using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
            var userForms = from d in db.mstUserForms
                            where d.UserId == Convert.ToInt32(userId)
                            select new Models.MstUserForm
                            {
                                Id = d.Id,
                                UserId = d.UserId,
                                User = d.mstUser.FullName,
                                FormId = d.FormId,
                                Form = d.sysForm.Form,
                                IsViewOnly = d.IsViewOnly,
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
                    Data.mstUserForm newUserForm = new Data.mstUserForm();
                    newUserForm.UserId = userForm.UserId;
                    newUserForm.FormId = userForm.FormId;
                    newUserForm.IsViewOnly = userForm.IsViewOnly;
                    db.mstUserForms.InsertOnSubmit(newUserForm);
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
                    var userForms = from d in db.mstUserForms where d.Id == Convert.ToInt32(id) select d;
                    if (userForms.Any())
                    {
                        var updateUserForm = userForms.FirstOrDefault();
                        updateUserForm.UserId = userForm.UserId;
                        updateUserForm.FormId = userForm.FormId;
                        updateUserForm.IsViewOnly = userForm.IsViewOnly;

                        db.SubmitChanges();

                        return Request.CreateResponse(HttpStatusCode.OK);
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
                    db.mstUserForms.DeleteOnSubmit(userForms.First());
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
