using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnExpenses
    {
        [Key]
        public Int32 Id { get; set; }
        public String ExpenseNumber { get; set; }
        public String ExpenseDate { get; set; }
        public Int32 AccountId { get; set; }
        public String Account { get; set; }
        public Int32 CollectorId { get; set; }
        public String Collector { get; set; }
        public String CollectorAreaAssigned { get; set; }
        public Int32 ExpenseTypeId { get; set; }
        public String ExpenseType { get; set; }
        public String Particulars { get; set; }
        public Decimal ExpenseAmount { get; set; }
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