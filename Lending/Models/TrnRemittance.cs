using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnRemittance
    {
        public Int32 Id { get; set; }
        public String RemittanceNumber { get; set; }
        public String RemittanceDate { get; set; }
        public Int32 AreaId { get; set; }
        public String Area { get; set; }
        public Int32 StaffId { get; set; }
        public String Staff { get; set; }
        public String Particulars { get; set; }
        public Int32 PreparedByUserId { get; set; }
        public String PreparedByUser { get; set; }
        public Decimal RemitAmount { get; set; }
        public Boolean IsLocked { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUser { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedByUserId { get; set; }
        public String UpdatedByUser { get; set; }
        public String UpdatedDateTime { get; set; }
    }
}