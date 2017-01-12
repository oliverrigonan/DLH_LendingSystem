using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiLoanDeductionsController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan deductions list
        [Authorize]
        [HttpGet]
        [Route("api/loanDeductions/listByLoanId/{loanId}")]
        public List<Models.TrnLoanDeductions> listLoanDeductionsByLoanId(String loanId)
        {
            var loanDeductions = from d in db.trnLoanDeductions
                                  where d.LoanId == Convert.ToInt32(loanId)
                                  select new Models.TrnLoanDeductions
                                  {
                                      Id = d.Id,
                                      LoanId = d.LoanId,
                                      DeductionId = d.DeductionId,
                                      Deduction = d.mstDeduction.Deduction,
                                      IsPercentage = d.mstDeduction.IsPercentage,
                                      PercentageRate = d.mstDeduction.PercentageRate,
                                      DeductionAmount = d.DeductionAmount
                                  };

            return loanDeductions.ToList();
        }

        // add loan deductions
        [Authorize]
        [HttpPost]
        [Route("api/loanDeductions/add")]
        public HttpResponseMessage addLoanDeductions(Models.TrnLoanDeductions loanApplicationDeductions)
        {
            try
            {
                var loan = from d in db.trnLoans where d.Id == loanApplicationDeductions.LoanId select d;
                if (loan.Any())
                {
                    if (!loan.FirstOrDefault().IsLocked)
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
                            String matchPageString = "LoanApplicationDetail";
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
                                Data.trnLoanDeduction newLoanApplicationDeductions = new Data.trnLoanDeduction();
                                newLoanApplicationDeductions.LoanId = loanApplicationDeductions.LoanId;
                                newLoanApplicationDeductions.DeductionId = loanApplicationDeductions.DeductionId;
                                newLoanApplicationDeductions.DeductionAmount = loanApplicationDeductions.DeductionAmount;
                                db.trnLoanDeductions.InsertOnSubmit(newLoanApplicationDeductions);
                                db.SubmitChanges();

                                Business.UpdateLoan updateLoan = new Business.UpdateLoan();
                                updateLoan.updateLoan(loanApplicationDeductions.LoanId);

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

        // update loan deductions
        [Authorize]
        [HttpPut]
        [Route("api/loanDeductions/update/{id}")]
        public HttpResponseMessage updateLoanDeductions(String id, Models.TrnLoanDeductions loanApplicationDeductions)
        {
            try
            {
                var loan = from d in db.trnLoans where d.Id == loanApplicationDeductions.LoanId select d;
                if (loan.Any())
                {
                    if (!loan.FirstOrDefault().IsLocked)
                    {
                        var loanApplicationDeductionss = from d in db.trnLoanDeductions where d.Id == Convert.ToInt32(id) select d;
                        if (loanApplicationDeductionss.Any())
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
                                String matchPageString = "LoanApplicationDetail";
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
                                    var updateLoanApplicationDeductions = loanApplicationDeductionss.FirstOrDefault();
                                    updateLoanApplicationDeductions.LoanId = loanApplicationDeductions.LoanId;
                                    updateLoanApplicationDeductions.DeductionId = loanApplicationDeductions.DeductionId;
                                    updateLoanApplicationDeductions.DeductionAmount = loanApplicationDeductions.DeductionAmount;
                                    db.SubmitChanges();

                                    Business.UpdateLoan updateLoan = new Business.UpdateLoan();
                                    updateLoan.updateLoan(loanApplicationDeductions.LoanId);

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
                            return Request.CreateResponse(HttpStatusCode.NotFound);
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

        // delete loan deductions
        [Authorize]
        [HttpDelete]
        [Route("api/loanDeductions/delete/{id}")]
        public HttpResponseMessage deleteLoanDeductions(String id)
        {
            try
            {
                var loanApplicationDeductionss = from d in db.trnLoanDeductions where d.Id == Convert.ToInt32(id) select d;
                if (loanApplicationDeductionss.Any())
                {
                    var loan = from d in db.trnLoans where d.Id == loanApplicationDeductionss.FirstOrDefault().LoanId select d;
                    if (loan.Any())
                    {
                        if (!loan.FirstOrDefault().IsLocked)
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
                                String matchPageString = "LoanApplicationDetail";
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
                                    var loanId = Convert.ToInt32(id);

                                    db.trnLoanDeductions.DeleteOnSubmit(loanApplicationDeductionss.First());
                                    db.SubmitChanges();

                                    Business.UpdateLoan updateLoan = new Business.UpdateLoan();
                                    updateLoan.updateLoan(loanId);

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
