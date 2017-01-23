using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnCollectionLines
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 CollectionId { get; set; }
        public String Collection { get; set; }
        public Int32 LoanId { get; set; }
        public Int32 LoanLinesId { get; set; }
        public String LoanLinesDayReference { get; set; }
        public String LoanLinesCollectibleDate { get; set; }
        public Int32 PenaltyId { get; set; }
        public String Penalty { get; set; }
        public Decimal PenaltyAmount { get; set; }
        public Decimal PaidAmount { get; set; }
        public Decimal BalanceAmount { get; set; }
        public Boolean IsReconstructed { get; set; }
        public String CollectedDate { get; set; }
    }
}