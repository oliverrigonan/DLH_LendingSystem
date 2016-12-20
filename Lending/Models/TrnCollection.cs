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
        public Int32 LoanId { get; set; }
        public String LoanNumber { get; set; }
        public Decimal NetAmount { get; set; }
        public String Applicant { get; set; }
        public String Area { get; set; }
        public Int32 TermId { get; set; }
        public Decimal TermNoOfDays { get; set; }
        public Decimal TermNoOfAllowanceDays { get; set; }
        public Boolean IsFullyPaid { get; set; }
        public Boolean IsOverdue { get; set; }
        public Boolean IsLocked { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUser { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedByUserId { get; set; }
        public String UpdatedByUser { get; set; }
        public String UpdatedDateTime { get; set; }
    }
}