using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnLoanApplicationCollateral
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 LoanId { get; set; }
        public String Type { get; set; }
        public String Brand { get; set; }
        public String ModelNumber { get; set; }
        public String SerialNumber { get; set; }
    }
}