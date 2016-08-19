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
            var users = from d in db.mstUsers.OrderByDescending(d => d.Id)
                              select new Models.MstUser
                              {
                                  Id = d.Id,
                                  AspUserId = d.AspUserId,
                                  Username = d.Username,
                                  Password = d.Password,
                                  FullName = d.FullName,
                                  CreatedDate = d.CreatedDate.ToString(),
                                  UpdatedDate = d.UpdatedDate.ToString()
                              };

            return users.ToList();
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
