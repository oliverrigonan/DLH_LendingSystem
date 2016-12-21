using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnDailyCollection
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 CollectionId { get; set; }
        public String CollectionNumber { get; set; }
        public Int32 LoanId { get; set; }
        public Int32 AccountId { get; set; }
        public String Applicant { get; set; }
        public String DailyCollectionDate { get; set; }
        public Decimal NetAmount { get; set; }
        public Decimal CollectibleAmount { get; set; }
        public Decimal PenaltyAmount { get; set; }
        public Decimal PaidAmount { get; set; }
        public Decimal PreviousBalanceAmount { get; set; }
        public Decimal CurrentBalanceAmount { get; set; }
        public Boolean IsCurrentCollection { get; set; }
        public Boolean IsCleared { get; set; }
        public Boolean IsAbsent { get; set; }
        public Boolean IsPartiallyPaid { get; set; }
        public Boolean IsPaidInAdvanced { get; set; }
        public Boolean IsFullyPaid { get; set; }
        public Boolean IsProcessed { get; set; }
        public Boolean CanPerformAction { get; set; }
        public Boolean IsDueDate { get; set; }
        public Boolean IsAllowanceDay { get; set; }
        public Boolean IsLastDay { get; set; }
        public Int32? ReconstructId { get; set; }
        public Boolean IsReconstructed { get; set; }
        public String Status { get; set; }
        public Boolean IsOverdue { get; set; }
        public String Duedate { get; set; }
    }
}