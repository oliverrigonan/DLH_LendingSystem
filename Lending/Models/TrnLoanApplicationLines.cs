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
        public Decimal Principal { get; set; }
        public Decimal ProcessingFee { get; set; }
        public Decimal Passbook { get; set; }
        public Decimal Balance { get; set; }
        public Decimal Penalty { get; set; }
        public Decimal LateInt { get; set; }
        public Decimal Advance { get; set; }
        public Decimal Requirements { get; set; }
        public Decimal InsuranceIPIorPPI { get; set; }
        public Decimal NetAmount { get; set; }
        public Int32 AccountId { get; set; }
        public String Account { get; set; }
        public String Particulars { get; set; }
    }
}