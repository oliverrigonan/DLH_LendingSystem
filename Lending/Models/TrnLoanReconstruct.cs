using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnLoanReconstruct
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 LoanId { get; set; }
        public String LoanNumber { get; set; }
        public Int32 ReconstructLoanId { get; set; }
        public String ReconstructLoanNumber { get; set; }
        public Decimal ReconstuctLoanTotalBalanceAmount { get; set; }
    }
}