using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class SysCollectionStatus
    {
        [Key]
        public Int32 Id { get; set; }
        public String Status { get; set; }
    }
}