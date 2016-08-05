using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class tblApplicantApplianceOwned
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 ApplicantId { get; set; }
        public String Applicant { get; set; }
        public String ApplianceBrand { get; set; }
        public String PresentValue { get; set; }
    }
}