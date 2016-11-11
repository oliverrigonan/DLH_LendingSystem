using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lending.Controllers
{
    public class SoftwareController : UserController
    {
        // GET: Software
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Forbidden()
        {
            return View();
        }

        [Authorize]
        public ActionResult NotFound()
        {
            return View();
        }

        [Authorize]
        public ActionResult ApplicantList()
        {
            return View();
        }

        [Authorize]
        public ActionResult ApplicantDetail(Int32? id)
        {
            if(id == null) {
                return RedirectToAction("NotFound", "Software");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult LoanApplicationList()
        {
            return View();
        }

        [Authorize]
        public ActionResult LoanApplicationDetail(Int32? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Software");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult CompanyList()
        {
            return View();
        }

        [Authorize]
        public ActionResult CompanyDetail(Int32? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Software");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult SystemTables()
        {
            return View();
        }

        [Authorize]
        public ActionResult CollectionList()
        {
            return View();
        }

        [Authorize]
        public ActionResult CollectionDetail(Int32? applicantId, Int32? loanId)
        {
            if (applicantId == null || loanId == null)
            {
                return RedirectToAction("NotFound", "Software");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult ExpensesList()
        {
            return View();
        }

        [Authorize]
        public ActionResult ExpenseDetail(Int32? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Software");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult UserList()
        {
            return View();
        }

        [Authorize]
        public ActionResult UserDetail(Int32? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Software");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult AreaList()
        {
            return View();
        }

        [Authorize]
        public ActionResult AreaDetail(Int32? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Software");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult StaffList()
        {
            return View();
        }

        [Authorize]
        public ActionResult StaffDetail(Int32? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Software");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult LoanReport()
        {
            return View();
        }

        [Authorize]
        public ActionResult CollectionReport()
        {
            return View();
        }

        [Authorize]
        public ActionResult ExpensesReport()
        {
            return View();
        }
    }
}