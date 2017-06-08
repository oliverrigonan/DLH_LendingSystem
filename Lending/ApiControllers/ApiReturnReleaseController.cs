using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class TrnReturnReleaseReturnReleaseController : ApiController
    {
        private Data.LendingDataContext db = new Data.LendingDataContext();

        [Authorize]
        [HttpGet]
        [Route("api/returnRelease/list/{startDate}/{endDate}")]
        public List<Models.TrnReturnRelease> listReturnRelease(String startDate, String endDate)
        {
            var remmitance = from d in db.trnReturnReleases.OrderByDescending(d => d.Id)
                             where d.ReturnReleaseDate >= Convert.ToDateTime(startDate)
                             && d.ReturnReleaseDate <= Convert.ToDateTime(endDate)
                             select new Models.TrnReturnRelease
                             {
                                 Id = d.Id,
                                 ReturnReleaseNumber = d.ReturnReleaseNumber,
                                 ReturnReleaseDate = d.ReturnReleaseDate.ToShortDateString(),
                                 Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                 LoanId = d.LoanId,
                                 LoanNumber = d.trnLoan.IsLoanApplication == true ? "LN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanRenew == true ? "RN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanReconstruct == true ? "RC-" + d.trnLoan.LoanNumber : " ",
                                 Particulars = d.Particulars,
                                 CollectorStaffId = d.CollectorStaffId,
                                 CollectorStaff = d.mstStaff.Staff,
                                 PreparedByUserId = d.PreparedByUserId,
                                 PreparedByUser = d.mstUser.FullName,
                                 ReturnAmount = d.ReturnAmount,
                                 IsLocked = d.IsLocked,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.mstUser1.FullName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.mstUser2.FullName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            return remmitance.ToList();
        }

        [Authorize]
        [HttpGet]
        [Route("api/returnRelease/get/{id}")]
        public Models.TrnReturnRelease listReturnRelease(String id)
        {
            var remmitance = from d in db.trnReturnReleases
                             where d.Id == Convert.ToInt32(id)
                             select new Models.TrnReturnRelease
                             {
                                 Id = d.Id,
                                 ReturnReleaseNumber = d.ReturnReleaseNumber,
                                 ReturnReleaseDate = d.ReturnReleaseDate.ToShortDateString(),
                                 Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                 LoanId = d.LoanId,
                                 LoanNumber = d.trnLoan.IsLoanApplication == true ? "LN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanRenew == true ? "RN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanReconstruct == true ? "RC-" + d.trnLoan.LoanNumber : " ",
                                 Particulars = d.Particulars,
                                 CollectorStaffId = d.CollectorStaffId,
                                 CollectorStaff = d.mstStaff.Staff,
                                 PreparedByUserId = d.PreparedByUserId,
                                 PreparedByUser = d.mstUser.FullName,
                                 ReturnAmount = d.ReturnAmount,
                                 IsLocked = d.IsLocked,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.mstUser1.FullName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.mstUser2.FullName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            return (Models.TrnReturnRelease)remmitance.FirstOrDefault();
        }


        // zero fill
        public String zeroFill(Int32 number, Int32 length)
        {
            var result = number.ToString();
            var pad = length - result.Length;
            while (pad > 0)
            {
                result = "0" + result;
                pad--;
            }

            return result;
        }

        // add returnRelease
        [Authorize]
        [HttpPost]
        [Route("api/returnRelease/add")]
        public Int32 addLockReturnRelease()
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                var mstUserForms = from d in db.mstUserForms
                                   where d.UserId == userId
                                   select new Models.MstUserForm
                                   {
                                       Id = d.Id,
                                       Form = d.sysForm.Form,
                                       CanPerformActions = d.CanPerformActions
                                   };

                if (mstUserForms.Any())
                {
                    String matchPageString = "ReturnReleaseList";
                    Boolean canPerformActions = false;

                    foreach (var mstUserForm in mstUserForms)
                    {
                        if (mstUserForm.Form.Equals(matchPageString))
                        {
                            if (mstUserForm.CanPerformActions)
                            {
                                canPerformActions = true;
                            }

                            break;
                        }
                    }

                    if (canPerformActions)
                    {
                        String returnReleaseNumber = "0000000001";
                        var returnReleases = from d in db.trnReturnReleases.OrderByDescending(d => d.Id) select d;
                        if (returnReleases.Any())
                        {
                            var newReturnReleaseNumber = Convert.ToInt32(returnReleases.FirstOrDefault().ReturnReleaseNumber) + 0000000001;
                            returnReleaseNumber = newReturnReleaseNumber.ToString();
                        }

                        var loan = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                   where d.IsLocked == true
                                   && d.IsLoanReconstruct == false
                                   select d;

                        if (loan.Any())
                        {
                            var staffs = from d in db.mstStaffs.OrderByDescending(d => d.Id)
                                         select d;

                            if (staffs.Any())
                            {
                                Data.trnReturnRelease newReturnRelease = new Data.trnReturnRelease();
                                newReturnRelease.ReturnReleaseNumber = zeroFill(Convert.ToInt32(returnReleaseNumber), 10);
                                newReturnRelease.ReturnReleaseDate = DateTime.Today;
                                newReturnRelease.LoanId = loan.FirstOrDefault().Id;
                                newReturnRelease.Particulars = "NA";
                                newReturnRelease.PreparedByUserId = userId;
                                newReturnRelease.CollectorStaffId = staffs.FirstOrDefault().Id;
                                newReturnRelease.ReturnAmount = loan.FirstOrDefault().NetAmount;
                                newReturnRelease.IsLocked = false;
                                newReturnRelease.CreatedByUserId = userId;
                                newReturnRelease.CreatedDateTime = DateTime.Now;
                                newReturnRelease.UpdatedByUserId = userId;
                                newReturnRelease.UpdatedDateTime = DateTime.Now;

                                db.trnReturnReleases.InsertOnSubmit(newReturnRelease);
                                db.SubmitChanges();

                                return newReturnRelease.Id;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                        else
                        {
                            return 0;
                        }

                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        // lock returnRelease
        [Authorize]
        [HttpPut]
        [Route("api/returnRelease/lock/{id}")]
        public HttpResponseMessage lockReturnRelease(String id, Models.TrnReturnRelease returnRelease)
        {
            try
            {
                var returnReleases = from d in db.trnReturnReleases where d.Id == Convert.ToInt32(id) select d;
                if (returnReleases.Any())
                {
                    if (!returnReleases.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                        var mstUserForms = from d in db.mstUserForms
                                           where d.UserId == userId
                                           select new Models.MstUserForm
                                           {
                                               Id = d.Id,
                                               Form = d.sysForm.Form,
                                               CanPerformActions = d.CanPerformActions
                                           };

                        if (mstUserForms.Any())
                        {
                            String matchPageString = "ReturnReleaseDetail";
                            Boolean canPerformActions = false;

                            foreach (var mstUserForm in mstUserForms)
                            {
                                if (mstUserForm.Form.Equals(matchPageString))
                                {
                                    if (mstUserForm.CanPerformActions)
                                    {
                                        canPerformActions = true;
                                    }

                                    break;
                                }
                            }

                            if (canPerformActions)
                            {
                                var lockReturnRelease = returnReleases.FirstOrDefault();
                                lockReturnRelease.ReturnReleaseDate = Convert.ToDateTime(returnRelease.ReturnReleaseDate);
                                lockReturnRelease.LoanId = returnRelease.LoanId;
                                lockReturnRelease.Particulars = returnRelease.Particulars;
                                lockReturnRelease.CollectorStaffId = returnRelease.CollectorStaffId;
                                lockReturnRelease.ReturnAmount = returnRelease.ReturnAmount;
                                lockReturnRelease.IsLocked = true;
                                lockReturnRelease.UpdatedByUserId = userId;
                                lockReturnRelease.UpdatedDateTime = DateTime.Now;

                                db.SubmitChanges();

                                var loans = from d in db.trnLoans
                                            where d.Id == returnRelease.LoanId
                                            select d;

                                if (loans.Any())
                                {
                                    var updateIsReturnRelease = loans.FirstOrDefault();
                                    updateIsReturnRelease.IsReturnRelease = true;
                                    db.SubmitChanges();
                                }

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // unlock returnRelease
        [Authorize]
        [HttpPut]
        [Route("api/returnRelease/unlock/{id}")]
        public HttpResponseMessage unlockReturnRelease(String id, Models.TrnReturnRelease returnRelease)
        {
            try
            {
                var returnReleases = from d in db.trnReturnReleases where d.Id == Convert.ToInt32(id) select d;
                if (returnReleases.Any())
                {
                    if (returnReleases.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                        var mstUserForms = from d in db.mstUserForms
                                           where d.UserId == userId
                                           select new Models.MstUserForm
                                           {
                                               Id = d.Id,
                                               Form = d.sysForm.Form,
                                               CanPerformActions = d.CanPerformActions
                                           };

                        if (mstUserForms.Any())
                        {
                            String matchPageString = "ReturnReleaseDetail";
                            Boolean canPerformActions = false;

                            foreach (var mstUserForm in mstUserForms)
                            {
                                if (mstUserForm.Form.Equals(matchPageString))
                                {
                                    if (mstUserForm.CanPerformActions)
                                    {
                                        canPerformActions = true;
                                    }

                                    break;
                                }
                            }

                            if (canPerformActions)
                            {
                                var unlockReturnRelease = returnReleases.FirstOrDefault();
                                unlockReturnRelease.IsLocked = false;
                                unlockReturnRelease.UpdatedByUserId = userId;
                                unlockReturnRelease.UpdatedDateTime = DateTime.Now;
                                db.SubmitChanges();

                                var loans = from d in db.trnLoans
                                            where d.Id == returnReleases.FirstOrDefault().LoanId
                                            select d;

                                if (loans.Any())
                                {
                                    var updateIsReturnRelease = loans.FirstOrDefault();
                                    updateIsReturnRelease.IsReturnRelease = false;
                                    db.SubmitChanges();
                                }

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // delete returnRelease
        [Authorize]
        [HttpDelete]
        [Route("api/returnRelease/delete/{id}")]
        public HttpResponseMessage deleteReturnRelease(String id)
        {
            try
            {
                var returnReleases = from d in db.trnReturnReleases where d.Id == Convert.ToInt32(id) select d;
                if (returnReleases.Any())
                {
                    if (!returnReleases.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                        var mstUserForms = from d in db.mstUserForms
                                           where d.UserId == userId
                                           select new Models.MstUserForm
                                           {
                                               Id = d.Id,
                                               Form = d.sysForm.Form,
                                               CanPerformActions = d.CanPerformActions
                                           };

                        if (mstUserForms.Any())
                        {
                            String matchPageString = "ReturnReleaseList";
                            Boolean canPerformActions = false;

                            foreach (var mstUserForm in mstUserForms)
                            {
                                if (mstUserForm.Form.Equals(matchPageString))
                                {
                                    if (mstUserForm.CanPerformActions)
                                    {
                                        canPerformActions = true;
                                    }

                                    break;
                                }
                            }

                            if (canPerformActions)
                            {
                                db.trnReturnReleases.DeleteOnSubmit(returnReleases.First());
                                db.SubmitChanges();

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
