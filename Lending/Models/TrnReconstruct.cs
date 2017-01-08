using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnReconstruct
    {
        [Key]
        public Int32 Id { get; set; }
        public String ReconstructNumber { get; set; }
        public String ReconstructDate { get; set; }
        public String MaturityDate { get; set; }
        public String Particulars { get; set; }
        public Int32 LoanId { get; set; }
        public Decimal LoanBalanceAmount { get; set; }
        public String PreviousLoanEndDate { get; set; }
        public Int32 InterestId { get; set; }
        public Decimal InterestRate { get; set; }
        public Decimal InterestAmount { get; set; }
        public String ReconstructAmount { get; set; }
        public Int32 PreparedByUserId { get; set; }
        public String PreparedByUser { get; set; }
        public Boolean IsLocked { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUser { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedByUserId { get; set; }
        public String UpdatedByUser { get; set; }
        public String UpdatedDateTime { get; set; }
    }
}