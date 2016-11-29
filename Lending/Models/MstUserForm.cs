using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class MstUserForm
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 UserId { get; set; }
        public String User { get; set; }
        public Int32 FormId { get; set; }
        public String Form { get; set; }
        public Boolean IsViewOnly { get; set; }
    }
}