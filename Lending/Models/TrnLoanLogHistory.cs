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
        public String CollectibleDate { get; set; }
        public Decimal NetAmount { get; set; }
        public Decimal CollectibleAmount { get; set; }
        public Decimal Penalty { get; set; }
        public Boolean IsPenalty { get; set; }
        public Decimal PaidAmount { get; set; }
        public Decimal PreviousBalance { get; set; }
        public Decimal CurrentBalance { get; set; }
        public Decimal BalanceNetAmount { get; set; }
    }
}