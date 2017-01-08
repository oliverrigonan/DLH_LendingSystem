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
        public String LoanDetail { get; set; }
        public Decimal CollectibleAmount { get; set; }
        public Boolean IsCleared { get; set; }
        public Boolean IsAbsent { get; set; }
        public Decimal PenaltyAmount { get; set; }
        public Boolean IsPartiallyPaid { get; set; }
        public Boolean IsPaidInAdvanced { get; set; }
        public String AdvancePaidDate { get; set; }
        public Boolean IsFullyPaid { get; set; }
        public Decimal PaidAmount { get; set; }
        public Decimal BalanceAmount { get; set; }
        public String Status { get; set; }
        public Boolean IsAllowanceDay { get; set; }
        public String Particulars { get; set; }
        public Boolean IsLocked { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUser { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedByUserId { get; set; }
        public String UpdatedByUser { get; set; }
        public String UpdatedDateTime { get; set; }
    }
}