﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiApplicantCivilStatusController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // residence type list
        [Authorize]
        [HttpGet]
        [Route("api/civilStatus/list")]
        public List<Models.tblApplicantCivilStatus> listApplicantCivilStatus()
        {
            var civilStatus = from d in db.tblApplicantCivilStatus
                                 select new Models.tblApplicantCivilStatus
                                 {
                                     Id = d.Id,
                                     CivilStatus = d.CivilStatus,
                                     CreatedByUserId = d.CreatedByUserId,
                                     CreatedByUser = d.tblUser.FirstName + " " + d.tblUser.MiddleName + " " + d.tblUser.LastName,
                                     CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                     UpdatedByUserId = d.UpdatedByUserId,
                                     UpdatedByUser = d.tblUser1.FirstName + " " + d.tblUser1.MiddleName + " " + d.tblUser1.LastName,
                                     UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                                 };

            return civilStatus.ToList();
        }
    }
}