using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lending.Reports
{
    public class RepDailyAreaCollectionsPDFController : Controller
    {
        // GET: RepDailyAreaCollectionsPDF
        public ActionResult dailyAreaCollectionsPDF(String date, String areaId)
        {
            if (date != null && areaId != null)
            {

            }
            else
            {

            }

            return View();
        }
    }
}