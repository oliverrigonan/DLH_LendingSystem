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
        public Int32 CollectionId { get; set; }
        public String ReconstructNumber { get; set; }
        public String StartDate { get; set; }
        public String EndDate { get; set; }
        public Int32 TermId { get; set; }
        public String Term { get; set; }
        public Decimal TermNoOfDays { get; set; }
        public Decimal TermNoOfAllowanceDays { get; set; }
        public Int32 InterestId { get; set; }
        public Decimal InterestRate { get; set; }
        public Decimal InterestAmount { get; set; }
        public Int32 PenaltyId { get; set; }
        public String Penalty { get; set; }
        public Decimal CurrentBalanceAmount { get; set; }
        public Decimal BalanceAmount { get; set; }
    }
}