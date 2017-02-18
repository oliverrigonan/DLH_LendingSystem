using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Lending.Controllers
{
    public class SoftwareController : UserController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        public String pageAccess(String page)
        {
            var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).FirstOrDefault();
            var userForms = from d in db.mstUserForms
                            where d.UserId == userId
                            select new Models.MstUserForm
                            {
                                Id = d.Id,
                                UserId = d.UserId,
                                User = d.mstUser.FullName,
                                FormId = d.FormId,
                                Form = d.sysForm.Form,
                                CanPerformActions = d.CanPerformActions,
                            };

            String pageName = page;
            String emptyPageName = "";

            foreach (var userForm in userForms)
            {
                if (pageName.Equals(userForm.Form))
                {
                    var CanPerformActions = 0;
                    if (userForm.CanPerformActions) 
                    {
                        CanPerformActions = 1;
                    }

                    ViewData.Add("CanPerformActions", CanPerformActions);

                    emptyPageName = userForm.Form;
                    break;
                }
            }

            return emptyPageName;
        }

        // GET: Software
        [Authorize]
        public ActionResult Index()
        {
            if (pageAccess("Software").Equals("Software"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
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
            if (pageAccess("ApplicantList").Equals("ApplicantList"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult ApplicantDetail(Int32? id)
        {
            if (pageAccess("ApplicantDetail").Equals("ApplicantDetail"))
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
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }

        }

        [Authorize]
        public ActionResult LoanApplicationList()
        {
            if (pageAccess("LoanApplicationList").Equals("LoanApplicationList"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult LoanApplicationDetail(Int32? id)
        {
            if (pageAccess("LoanApplicationDetail").Equals("LoanApplicationDetail"))
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
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult CompanyList()
        {
            if (pageAccess("CompanyList").Equals("CompanyList"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult CompanyDetail(Int32? id)
        {
            if (pageAccess("CompanyDetail").Equals("CompanyDetail"))
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
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult SystemTables()
        {
            if (pageAccess("SystemTables").Equals("SystemTables"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult CollectionList()
        {
            if (pageAccess("CollectionList").Equals("CollectionList"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult CollectionDetail(Int32? id)
        {
            if (pageAccess("CollectionDetail").Equals("CollectionDetail"))
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
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult ExpensesList()
        {
            if (pageAccess("ExpensesList").Equals("ExpensesList"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult ExpenseDetail(Int32? id)
        {
            if (pageAccess("ExpenseDetail").Equals("ExpenseDetail"))
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
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult UserList()
        {
            if (pageAccess("UserList").Equals("UserList"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult UserDetail(Int32? id)
        {
            if (pageAccess("UserDetail").Equals("UserDetail"))
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
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult AreaList()
        {
            if (pageAccess("AreaList").Equals("AreaList"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult AreaDetail(Int32? id)
        {
            if (pageAccess("AreaDetail").Equals("AreaDetail"))
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
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult StaffList()
        {
            if (pageAccess("StaffList").Equals("StaffList"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult StaffDetail(Int32? id)
        {
            if (pageAccess("StaffDetail").Equals("StaffDetail"))
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
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult Reports()
        {
            if (pageAccess("Reports").Equals("Reports"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult ReconstructList()
        {
            if (pageAccess("ReconstructList").Equals("ReconstructList"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult ReconstructDetail(Int32? id)
        {
            if (pageAccess("ReconstructDetail").Equals("ReconstructDetail"))
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
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult RenewList()
        {
            if (pageAccess("RenewList").Equals("RenewList"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

        [Authorize]
        public ActionResult RenewDetail(Int32? id)
        {
            if (pageAccess("RenewDetail").Equals("RenewDetail"))
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
            else
            {
                return RedirectToAction("Forbidden", "Software");
            }
        }

    }
}