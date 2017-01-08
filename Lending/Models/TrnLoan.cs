using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnLoan
    {
        [Key]
        public Int32 Id { get; set; }
        public String LoanNumber { get; set; }
        public String LoanDate { get; set; }
        public String MaturityDate { get; set; }
        public Int32 ApplicantId { get; set; }
        public String Applicant { get; set; }
        public String Area { get; set; }
        public String Particulars { get; set; }
        public Int32 TermId { get; set; }
        public String Term { get; set; }
        public Decimal TermNoOfDays { get; set; }
        public Decimal TermNoOfAllowanceDays { get; set; }
        public String LoanEndDate { get; set; }
        public Int32 PenaltyId { get; set; }
        public String Penalty { get; set; }
        public Decimal NoOfAbsent { get; set; }
        public Int32 PreparedByUserId { get; set; }
        public String PreparedByUser { get; set; }
        public Decimal PrincipalAmount { get; set; }
        public Int32 InterestId { get; set; }
        public String Interest { get; set; }
        public Decimal InterestRate { get; set; }
        public Decimal InterestAmount { get; set; }
        public Decimal DeductionAmount { get; set; }
        public Decimal NetAmount { get; set; }
        public Decimal DailyCollectibleAmount { get; set; }
        public Decimal CurrentCollectibeAmount { get; set; }
        public Decimal PaidAmount { get; set; }
        public String NextPaidDate { get; set; }
        public Decimal PenaltyAmount { get; set; }
        public Boolean IsReconstruct { get; set; }
        public Decimal ReconstructInterestAmount { get; set; }
        public Decimal BalanceAmount { get; set; }
        public Boolean IsFullyPaid { get; set; }
        public Boolean IsLocked { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUser { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedByUserId { get; set; }
        public String UpdatedByUser { get; set; }
        public String UpdatedDateTime { get; set; }
    }
}