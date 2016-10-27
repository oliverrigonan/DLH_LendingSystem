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
        public Int32 LoanId { get; set; }
        public String LoanNumber { get; set; }
        public Int32 ApplicantId { get; set; }
        public String Applicant { get; set; }
        public String Area { get; set; }
        public Boolean IsFullyPaid { get; set; }
        public Int32 AccountId { get; set; }
        public String Account { get; set; }
        public String CollectionDate { get; set; }
        public Decimal NetAmount { get; set; }
        public Decimal CollectibleAmount { get; set; }
        public Decimal PenaltyAmount { get; set; }
        public Decimal PaidAmount { get; set; }
        public Decimal PreviousBalanceAmount { get; set; }
        public Decimal CurrentBalanceAmount { get; set; }
        public Boolean IsCleared { get; set; }
        public Boolean IsAbsent { get; set; }
        public Boolean IsPartialPayment { get; set; }
        public Boolean IsAdvancedPayment { get; set; }
        public Boolean IsDueDate { get; set; }
        public Boolean IsOverdue { get; set; }
        public Boolean IsExtended { get; set; }
        public Boolean IsCurrentCollection { get; set; }
        public Boolean IsProcessed { get; set; }
        public Boolean IsAction { get; set; }
        public Int32 AssignedCollectorId { get; set; }
        public String AssignedCollector { get; set; }
        public String AssignedCollectorArea { get; set; }
        public Int32 CurrentCollectorId { get; set; }
        public String CurrentCollector { get; set; }
        public String CurrentCollectorArea { get; set; }
    }
}