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
        public String CollectionNumber { get; set; }
        public String PayDate { get; set; }
        public String Particulars { get; set; }
        public Int32 StatusId { get; set; }
        public String Status { get; set; }
        public Decimal PaidAmount { get; set; }
        public Decimal PenaltyAmount { get; set; }
        public Int32 LoanId { get; set; }
    }
}