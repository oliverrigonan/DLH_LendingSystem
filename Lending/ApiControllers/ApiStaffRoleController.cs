using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiStaffRoleController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // staff role list
        [Authorize]
        [HttpGet]
        [Route("api/staffRole/list")]
        public List<Models.SysStaffRole> listStaffRole()
        {
            var staffRoles = from d in db.sysStaffRoles.OrderByDescending(d => d.Id)
                               select new Models.SysStaffRole
                               {
                                   Id = d.Id,
                                   StaffRole = d.StaffRole,
                               };

            return staffRoles.ToList();
        }
    }
}
