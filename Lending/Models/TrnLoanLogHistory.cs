using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnLoanLogHistory
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 LoanId { get; set; }
        public String LoanNumber { get; set; }
        public String Applicant { get; set; }
        public String Area { get; set; }
        public String CollectionDate { get; set; }
        public Decimal NetAmount { get; set; }
        public Decimal CollectibleAmount { get; set; }
        public Decimal PenaltyAmount { get; set; }
        public Decimal PaidAmount { get; set; }
        public Decimal PreviousBalanceAmount { get; set; }
        public Decimal CurrentBalanceAmount { get; set; }
        public Boolean IsCleared { get; set; }
        public Boolean IsPenalty { get; set; }
        public Boolean IsOverdue { get; set; }
        public Boolean IsFullyPaid { get; set; }
        public String LoanApplicationMaturityDate { get; set; }
    }
}