using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnLoanHistory
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 ApplicantId { get; set; }
        public Int32 LoanId { get; set; }
        public String LoanNumber { get; set; }
        public String LoanNumberDetail { get; set; }
        public String LoanDate { get; set; }
        public String Applicant { get; set; }
        public String Area { get; set; }
        public String Particulars { get; set; }
        public Int32 PreparedByUserId { get; set; }
        public String PreparedByUser { get; set; }
        public Int32 TermId { get; set; }
        public String Term { get; set; }
        public Decimal TermNoOfDays { get; set; }
        public String MaturityDate { get; set; }
        public Decimal PrincipalAmount { get; set; }
        public Boolean IsAdvanceInterest { get; set; }
        public Int32 InterestId { get; set; }
        public String Interest { get; set; }
        public Decimal InterestRate { get; set; }
        public Decimal InterestAmount { get; set; }
        public Decimal PreviousBalanceAmount { get; set; }
        public Decimal PreviousPenaltyAmount { get; set; }
        public Decimal DeductionAmount { get; set; }
        public Decimal NetAmount { get; set; }
        public Decimal NetCollectionAmount { get; set; }
        public Decimal CollectibleAmount { get; set; }
        public Decimal TotalPaidAmount { get; set; }
        public Decimal TotalPenaltyAmount { get; set; }
        public Decimal TotalBalanceAmount { get; set; }
        public Boolean IsLoanApplication { get; set; }
        public Boolean IsLoanReconstruct { get; set; }
        public Boolean IsLoanRenew { get; set; }
        public Boolean IsReconstructed { get; set; }
        public Boolean IsRenewed { get; set; }
        public String ReconstructedDocNumber { get; set; }
        public String RenewedDocNumber { get; set; }
        public Boolean IsLocked { get; set; }
    }
}