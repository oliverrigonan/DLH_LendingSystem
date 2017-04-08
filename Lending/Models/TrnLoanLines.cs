using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnLoanLines
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 LoanId { get; set; }
        public String DayReference { get; set; }
        public String CollectibleDate { get; set; }
        public Decimal CollectibleAmount { get; set; }
        public Decimal PaidAmount { get; set; }
        public Decimal PenaltyAmount { get; set; }
        public Decimal BalanceAmount { get; set; }
        public Boolean IsCleared { get; set; }
        public String Applicant { get; set; }
        public Boolean IsReconstruct { get; set; }
        public Boolean IsRenew { get; set; }
        public Boolean IsLoanApplication { get; set; }
        public Boolean IsLoanReconstruct { get; set; }
        public Boolean IsLoanRenew { get; set; }
        public String DueDate { get; set; }
        public Boolean? ForOverdue { get; set; }
        public Decimal TermNoOfDays { get; set; }
    }
}