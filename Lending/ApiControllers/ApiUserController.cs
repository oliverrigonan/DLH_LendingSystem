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
        public Models.tblUser getCurrentLoggedInUser()
        {
            var currentUser = from d in db.tblUsers
                              where d.AspUserId == User.Identity.GetUserId()
                              select new Models.tblUser
                              {
                                  Id = d.Id,
                                  AspUserId = d.AspUserId,
                                  Username = d.Username,
                                  Password = d.Password,
                                  FirstName = d.FirstName,
                                  MiddleName = d.MiddleName,
                                  LastName = d.LastName,
                                  BirthDate = d.BirthDate.ToString(),
                                  JobTitle = d.JobTitle,
                                  AboutMe = d.AboutMe,
                                  AddressStreet = d.AddressStreet,
                                  AddressCity = d.AddressCity,
                                  AddressZip = d.AddressZip,
                                  AddressCountry = d.AddressCountry,
                                  ContactNumber = d.ContactNumber,
                                  EmailAddress = d.EmailAddress,
                                  CreatedDate = d.CreatedDate.ToString(),
                                  UpdatedDate = d.UpdatedDate.ToString()
                              };

            return (Models.tblUser)currentUser.FirstOrDefault();
        }

        // update user
        [Authorize]
        [HttpPut]
        [Route("api/user/update/currentLoggedInUser")]
        public HttpResponseMessage updateCurrentLoggedInUser(Models.tblUser user)
        {
            try
            {
                var currentUser = from d in db.tblUsers  where d.AspUserId == User.Identity.GetUserId() select d;
                if (currentUser.Any())
                {
                    var updateUser = currentUser.FirstOrDefault();
                    updateUser.FirstName = user.FirstName;
                    updateUser.MiddleName = user.MiddleName;
                    updateUser.LastName = user.LastName;
                    updateUser.BirthDate = null;
                    updateUser.JobTitle = user.JobTitle;
                    updateUser.AboutMe = user.AboutMe;
                    updateUser.AddressStreet = user.AddressStreet;
                    updateUser.AddressCity = user.AddressCity;
                    updateUser.AddressZip = user.AddressZip;
                    updateUser.AddressCountry = user.AddressCountry;
                    updateUser.ContactNumber = user.ContactNumber;
                    updateUser.EmailAddress = user.EmailAddress;
                    updateUser.UpdatedDate = DateTime.Now;
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
