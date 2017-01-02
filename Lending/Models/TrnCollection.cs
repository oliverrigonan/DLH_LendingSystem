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
        public String LoanDate { get; set; }

        public String MaturityDate { get; set; }
        public Decimal PrincipalAmount { get; set; }
        public String Interest { get; set; }
        public Decimal InterestRate { get;set;}
        public Decimal InterestAmount { get;set;}
        public Decimal TotalDeductionAmount { get; set; }
        public Decimal NetAmount { get; set; }

        public String Applicant { get; set; }
        public String Area { get; set; }
        public Int32 TermId { get; set; }
        public String Term { get; set; }
        public Decimal TermNoOfDays { get; set; }
        public Decimal TermNoOfAllowanceDays { get; set; }
        public Boolean IsFullyPaid { get; set; }
        public Boolean IsOverdue { get; set; }
    }
}