using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnLoanApplication
    {
        [Key]
        public Int32 Id { get; set; }
        public String LoanNumber { get; set; }
        public String LoanDate { get; set; }
        public String MaturityDate { get; set; }
        public Int32 AccountId { get; set; }
        public String Account { get; set; }
        public Int32 ApplicantId { get; set; }
        public String Applicant { get; set; }
        public String Area { get; set; }
        public String Particulars { get; set; }
        public Int32 LoanTypeId { get; set; }
        public String LoanType { get; set; }
        public Int32 TermId { get; set; }
        public String Term { get; set; }
        public Int32 InterestId { get; set; }
        public String Interest { get; set; }
        public Decimal InterestRate { get; set; }
        public Int32 PenaltyId { get; set; }
        public String Penalty { get; set; }
        public Int32 PreparedByUserId { get; set; }
        public String PreparedByUser { get; set; }
        public Int32 AssignedCollectorId { get; set; }
        public String AssignedCollector { get; set; }
        public Int32 CurrentCollectorId { get; set; }
        public String CurrentCollector { get; set; }
        public String CollectorAreaAssigned { get; set; }
        public Decimal PrincipalAmount { get; set; }
        public Decimal ProcessingFeeAmountDeduction { get; set; }
        public Decimal PassbookAmountDeduction { get; set; }
        public Decimal BalanceAmountDeduction { get; set; }
        public Decimal PenaltyAmountDeduction { get; set; }
        public Decimal LateIntAmountDeduction { get; set; }
        public Decimal AdvanceAmountDeduction { get; set; }
        public Decimal RequirementsAmountDeduction { get; set; }
        public Decimal InsuranceIPIorPPIAmountDeduction { get; set; }
        public Decimal NetAmount { get; set; }
        public Boolean IsLocked { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUser { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedByUserId { get; set; }
        public String UpdatedByUser { get; set; }
        public String UpdatedDateTime { get; set; }
    }
}