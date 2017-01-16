using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class MstAreaStaff
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 AreaId { get; set; }
        public Int32 StaffId { get; set; }
        public String Staff { get; set; }
    }
}