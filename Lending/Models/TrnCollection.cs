using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnCollection
    {
        [Key]
        public Int32 Id { get; set; }
        public String CollectionNumber { get; set; }
        public String CollectionDate { get; set; }
        public Int32 ApplicantId { get; set; }
        public String Applicant { get; set; }
        public Int32 LoanId { get; set; }
        public String LoanNumberDetail { get; set; }
        public String Particulars { get; set; }
        public Decimal TotalPaidAmount { get; set; }
        public Decimal TotalPenaltyAmount { get; set; }
        public Int32 CollectorStaffId { get; set; }
        public String CollectorStaff { get; set; }
        public Int32 PreparedByUserId { get; set; }
        public String PreparedByUser { get; set; }
        public Boolean IsLocked { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUser { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedByUserId { get; set; }
        public String UpdatedByUser { get; set; }
        public String UpdatedDateTime { get; set; }
        public Boolean IsReconstructed { get; set; }
        public Boolean IsRenewed { get; set; }
        public Boolean IsLoanApplication { get; set; }
        public Boolean IsLoanReconstruct { get; set; }
        public Boolean IsLoanRenew { get; set; }
        public Int32 AreaId { get; set; }
        public String Area { get; set; }
        public Decimal Active { get; set; }
        public Decimal Overdue { get; set; }
        public Decimal TotalCollection { get; set; }
        public Decimal TotalGrossCollection { get; set; }
        public Int32 CollectionStatusId { get; set; }
        public String CollectionStatus { get; set; }
    }
}