using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class MstApplicantCoMakerStatement
    {
        public Int32 Id { get; set; }
        public Int32 ApplicantId { get; set; }
        public Int32 CoMakerApplicantId { get; set; }
        public String CoMaker { get; set; }
        public String ContactNumber { get; set; }
    }
}