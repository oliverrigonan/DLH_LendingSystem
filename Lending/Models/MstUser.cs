using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class MstUser
    {
        [Key]
        public Int32 Id { get; set; }
        public String AspUserId { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String FullName { get; set; }
        public Int32 CompanyId { get; set; }
        public String Company { get; set; }
        public Boolean IsLocked { get; set; }
        public String CreatedDate { get; set; }
        public String UpdatedDate { get; set; }
    }
}