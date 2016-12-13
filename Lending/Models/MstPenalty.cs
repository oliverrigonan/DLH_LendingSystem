using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class MstPenalty
    {
        [Key]
        public Int32 Id { get; set; }
        public String Penalty { get; set; }
        public String Description { get; set; }
        public Decimal PenaltyAmount { get; set; }
        public Decimal NoOfAbsentLimit { get; set; }
        public Decimal PenaltyAmountOverNoOfAbsentLimit { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUser { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedByUserId { get; set; }
        public String UpdatedByUser { get; set; }
        public String UpdatedDateTime { get; set; }
    }
}