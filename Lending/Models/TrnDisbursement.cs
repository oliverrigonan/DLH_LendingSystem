using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnDisbursement
    {
        [Key]
        public Int32 Id { get; set; }
        public String DisbursementNumber { get; set; }
        public String DisbursementDate { get; set; }
        public Int32 AccountId { get; set; }
        public String Account { get; set; }
        public String Payee { get; set; }
        public String Particulars { get; set; }
        public Decimal Amount { get; set; }
        public Int32 PreparedByUserId { get; set; }
        public String PreparedByUser { get; set; }
        public Boolean IsLocked { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUser { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedByUserId { get; set; }
        public String UpdatedByUser { get; set; }
        public String UpdatedDateTime { get; set; }
    }
}