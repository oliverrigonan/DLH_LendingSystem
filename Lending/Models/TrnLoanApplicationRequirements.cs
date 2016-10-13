using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnLoanApplicationRequirements
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 LoanId { get; set; }
        public Int32 RequirementId { get; set; }
        public String Requirement { get; set; }
        public String Note { get; set; }
    }
}