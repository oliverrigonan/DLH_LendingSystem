using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class MstCoMakerStatementRealPropertiesOwned
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 CoMakerId { get; set; }
        public String CoMaker { get; set; }
        public String Real { get; set; }
        public String Location { get; set; }
        public String PresentValue { get; set; }
        public String EcumberedTo { get; set; }
    }
}