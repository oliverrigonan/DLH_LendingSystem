using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class TrnVault
    {
        public Int32 Id { get; set; }
        public String VaultNumber { get; set; }
        public String VaultDate { get; set; }
        public Int32 StaffId { get; set; }
        public String Staff { get; set; }
        public String Particulars { get; set; }
        public Int32 PreparedByUserId { get; set; }
        public String PreparedByUser { get; set; }
        public Decimal Amount { get; set; }
        public Boolean IsLocked { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUser { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedByUserId { get; set; }
        public String UpdatedByUser { get; set; }
        public String UpdatedDateTime { get; set; }
    }
}