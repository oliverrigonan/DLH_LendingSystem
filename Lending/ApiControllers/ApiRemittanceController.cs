using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
            var remmitance = from d in db.trnRemittances
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
                             where d.Id >= Convert.ToInt32(id)
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
    }
}
