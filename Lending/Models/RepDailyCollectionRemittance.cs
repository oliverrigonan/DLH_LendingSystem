using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class RepDailyCollectionRemittance
    {
        public String Area { get; set; }
        public Decimal GrossCollection { get; set; }
        public Decimal Expenses { get; set; }
        public Decimal NetRemitted { get; set; }
    }
}