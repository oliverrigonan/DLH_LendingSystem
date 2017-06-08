using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnReturnRelease
    {
        public Int32 Id { get; set; }
        public String ReturnReleaseNumber { get; set; }
        public String ReturnReleaseDate { get; set; }
        public String Applicant { get; set; }
        public Int32 LoanId { get; set; }
        public String LoanNumber { get; set; }
        public String Particulars { get; set; }
        public Int32 PreparedByUserId { get; set; }
        public String PreparedByUser { get; set; }
        public Int32 CollectorStaffId { get; set; }
        public String CollectorStaff { get; set; }
        public Decimal ReturnAmount { get; set; }
        public Boolean IsLocked { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUser { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedByUserId { get; set; }
        public String UpdatedByUser { get; set; }
        public String UpdatedDateTime { get; set; }
    }
}