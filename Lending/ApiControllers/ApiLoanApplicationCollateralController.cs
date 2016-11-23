using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiLoanApplicationCollateralController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan application collateral list
        [Authorize]
        [HttpGet]
        [Route("api/loanApplicationCollateral/listByLoanId/{loanId}")]
        public List<Models.TrnLoanApplicationCollateral> listLoanApplicationCollateralByLoanId(String loanId)
        {
            var loanApplicationCollaterals = from d in db.trnLoanApplicationCollaterals
                                             where d.LoanId == Convert.ToInt32(loanId)
                                             select new Models.TrnLoanApplicationCollateral
                                             {
                                                 Id = d.Id,
                                                 LoanId = d.LoanId,
                                                 Type = d.Type,
                                                 Brand = d.Brand,
                                                 ModelNumber = d.ModelNumber,
                                                 SerialNumber = d.SerialNumber
                                             };

            return loanApplicationCollaterals.ToList();
        }

        // add loan application collateral
        [Authorize]
        [HttpPost]
        [Route("api/loanApplicationCollateral/add")]
        public HttpResponseMessage addLoanApplicationCollateral(Models.TrnLoanApplicationCollateral loanApplicationCollateral)
        {
            try
            {
                var loanApplications = from d in db.trnLoanApplications where d.Id == loanApplicationCollateral.LoanId select d;
                if (loanApplications.Any())
                {
                    if (!loanApplications.FirstOrDefault().IsLocked)
                    {
                        Data.trnLoanApplicationCollateral newLoanApplicationCollateral = new Data.trnLoanApplicationCollateral();
                        newLoanApplicationCollateral.LoanId = loanApplicationCollateral.LoanId;
                        newLoanApplicationCollateral.Type = loanApplicationCollateral.Type;
                        newLoanApplicationCollateral.Brand = loanApplicationCollateral.Brand;
                        newLoanApplicationCollateral.ModelNumber = loanApplicationCollateral.ModelNumber;
                        newLoanApplicationCollateral.SerialNumber = loanApplicationCollateral.SerialNumber;
                        db.trnLoanApplicationCollaterals.InsertOnSubmit(newLoanApplicationCollateral);
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
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // update loan application collateral
        [Authorize]
        [HttpPut]
        [Route("api/loanApplicationCollateral/update/{id}")]
        public HttpResponseMessage updateLoanApplicationCollateral(String id, Models.TrnLoanApplicationCollateral loanApplicationCollateral)
        {
            try
            {
                var loanApplications = from d in db.trnLoanApplications where d.Id == loanApplicationCollateral.LoanId select d;
                if (loanApplications.Any())
                {
                    if (!loanApplications.FirstOrDefault().IsLocked)
                    {
                        var loanApplicationCollaterals = from d in db.trnLoanApplicationCollaterals where d.Id == Convert.ToInt32(id) select d;
                        if (loanApplicationCollaterals.Any())
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

        // delete loan application collateral
        [Authorize]
        [HttpDelete]
        [Route("api/loanApplicationCollateral/delete/{id}")]
        public HttpResponseMessage deleteLoanApplicationCollateral(String id)
        {
            try
            {
                var loanApplicationCollaterals = from d in db.trnLoanApplicationCollaterals where d.Id == Convert.ToInt32(id) select d;
                if (loanApplicationCollaterals.Any())
                {
                    var loanApplications = from d in db.trnLoanApplications where d.Id == loanApplicationCollaterals.FirstOrDefault().LoanId select d;
                    if (loanApplications.Any())
                    {
                        if (!loanApplications.FirstOrDefault().IsLocked)
                        {
                            db.trnLoanApplicationCollaterals.DeleteOnSubmit(loanApplicationCollaterals.First());
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
