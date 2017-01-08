using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiLoanCollateralController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan collateral list
        [Authorize]
        [HttpGet]
        [Route("api/loanCollateral/listByLoanId/{loanId}")]
        public List<Models.TrnLoanCollateral> listLoanCollateralByLoanId(String loanId)
        {
            var loanCollaterals = from d in db.trnLoanCollaterals
                                             where d.LoanId == Convert.ToInt32(loanId)
                                             select new Models.TrnLoanCollateral
                                             {
                                                 Id = d.Id,
                                                 LoanId = d.LoanId,
                                                 Type = d.Type,
                                                 Brand = d.Brand,
                                                 ModelNumber = d.ModelNumber,
                                                 SerialNumber = d.SerialNumber
                                             };

            return loanCollaterals.ToList();
        }

        // add loan collateral
        [Authorize]
        [HttpPost]
        [Route("api/loanCollateral/add")]
        public HttpResponseMessage addLoanCollateral(Models.TrnLoanCollateral loanApplicationCollateral)
        {
            try
            {
                var loanApplications = from d in db.trnLoans where d.Id == loanApplicationCollateral.LoanId select d;
                if (loanApplications.Any())
                {
                    if (!loanApplications.FirstOrDefault().IsLocked)
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
                                Data.trnLoanCollateral newLoanApplicationCollateral = new Data.trnLoanCollateral();
                                newLoanApplicationCollateral.LoanId = loanApplicationCollateral.LoanId;
                                newLoanApplicationCollateral.Type = loanApplicationCollateral.Type;
                                newLoanApplicationCollateral.Brand = loanApplicationCollateral.Brand;
                                newLoanApplicationCollateral.ModelNumber = loanApplicationCollateral.ModelNumber;
                                newLoanApplicationCollateral.SerialNumber = loanApplicationCollateral.SerialNumber;
                                db.trnLoanCollaterals.InsertOnSubmit(newLoanApplicationCollateral);
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

        // update loan collateral
        [Authorize]
        [HttpPut]
        [Route("api/loanCollateral/update/{id}")]
        public HttpResponseMessage updateLoanCollateral(String id, Models.TrnLoanCollateral loanApplicationCollateral)
        {
            try
            {
                var loanApplications = from d in db.trnLoans where d.Id == loanApplicationCollateral.LoanId select d;
                if (loanApplications.Any())
                {
                    if (!loanApplications.FirstOrDefault().IsLocked)
                    {
                        var loanApplicationCollaterals = from d in db.trnLoanCollaterals where d.Id == Convert.ToInt32(id) select d;
                        if (loanApplicationCollaterals.Any())
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
                                    var updateLoanApplicationCollateral = loanApplicationCollaterals.FirstOrDefault();
                                    updateLoanApplicationCollateral.LoanId = loanApplicationCollateral.LoanId;
                                    updateLoanApplicationCollateral.Type = loanApplicationCollateral.Type;
                                    updateLoanApplicationCollateral.Brand = loanApplicationCollateral.Brand;
                                    updateLoanApplicationCollateral.ModelNumber = loanApplicationCollateral.ModelNumber;
                                    updateLoanApplicationCollateral.SerialNumber = loanApplicationCollateral.SerialNumber;
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

        // delete loan collateral
        [Authorize]
        [HttpDelete]
        [Route("api/loanCollateral/delete/{id}")]
        public HttpResponseMessage deleteLoanCollateral(String id)
        {
            try
            {
                var loanApplicationCollaterals = from d in db.trnLoanCollaterals where d.Id == Convert.ToInt32(id) select d;
                if (loanApplicationCollaterals.Any())
                {
                    var loanApplications = from d in db.trnLoans where d.Id == loanApplicationCollaterals.FirstOrDefault().LoanId select d;
                    if (loanApplications.Any())
                    {
                        if (!loanApplications.FirstOrDefault().IsLocked)
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
                                    db.trnLoanCollaterals.DeleteOnSubmit(loanApplicationCollaterals.First());
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
