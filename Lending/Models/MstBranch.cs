using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class MstBranch
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 CompanyId { get; set; }
        public String Company { get; set; }
        public String Branch { get; set; }
        public String Address { get; set; }
        public String ContactNumber { get; set; }
    }
}