using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnLoanRenew
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 LoanId { get; set; }
        public String LoanNumber { get; set; }
        public Int32 ApplicantId { get; set; }
        public Int32 RenewLoanId { get; set; }
        public String RenewLoanNumber { get; set; }
        public Decimal RenewPrincipalAmount { get; set; }
        public Decimal RenewLoanTotalBalanceAmount { get; set; }
        public Decimal RenewLoanTotalPenaltyAmount { get; set; }
    }
}