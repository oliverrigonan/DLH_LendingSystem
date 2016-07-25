using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class tblUser
    {
        [Key]
        public Int32 Id { get; set; }
        public String AspUserId { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String FirstName { get; set; }
        public String MiddleName { get; set; }
        public String LastName { get; set; }
        public String BirthDate { get; set; }
        public String JobTitle { get; set; }
        public String AboutMe { get; set; }
        public String AddressStreet { get; set; }
        public String AddressCity { get; set; }
        public String AddressZip { get; set; }
        public String AddressCountry { get; set; }
        public String ContactNumber { get; set; }
        public String EmailAddress { get; set; }
        public String CreatedDate { get; set; }
        public String UpdatedDate { get; set; }
    }
}