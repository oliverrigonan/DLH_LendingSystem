using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnLoanApplicationLines
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 LoanId { get; set; }
        public String Particulars { get; set; }
        public Decimal Amount { get; set; }
    }
}