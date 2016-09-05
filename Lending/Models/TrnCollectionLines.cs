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
        public Int32 AccountId { get; set; }
        public String Account { get; set; }
        public Int32 LoanId { get; set; }
        public String LoanNumber { get; set; }
        public String LoanDate { get; set; }
        public Int32 PaytypeId { get; set; }
        public String Paytype { get; set; }
        public String CheckNumber { get; set; }
        public String CheckDate { get; set; }
        public String CheckBank { get; set; }
        public String Particulars { get; set; }
        public Decimal Amount { get; set; }
        public Decimal LoanAmount { get; set; }
        public Decimal PaidAmount { get; set; }
        public Decimal Balance { get; set; }
        public Int32 CollectedByCollectorId { get; set; }
        public String CollectedByCollector { get; set; }
    }
}