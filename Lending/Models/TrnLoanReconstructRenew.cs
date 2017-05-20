using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnLoanReconstructRenew
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 LoanId { get; set; }
        public Int32 ReconstructRenewLoanId { get; set; }
        public String ReconstructRenewLoanNumber { get; set; }
        public Decimal BalanceAmount { get; set; }

        public String LoanDate { get; set; }
        public Int32 ApplicantId { get; set; }
        public String Particulars { get; set; }

        public Decimal RenewPrincipalAmount { get; set; }
    }
}