using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiLoanApplicationLinesController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan application lines list
        [Authorize]
        [HttpGet]
        [Route("api/loanApplicationLines/listByLoanId/{loanId}")]
        public List<Models.TrnLoanApplicationLines> listLoanApplicationLinesByLoanId(String loanId)
        {
            var loanApplicationLines = from d in db.trnLoanApplicationLines.OrderByDescending(d => d.Id)
                                       where d.LoanId == Convert.ToInt32(loanId)
                                       select new Models.TrnLoanApplicationLines
                                       {
                                           Id = d.Id,
                                           LoanId = d.LoanId,
                                           Principal = d.Principal,
                                           ProcessingFee = d.ProcessingFee,
                                           Passbook = d.Passbook,
                                           Balance = d.Balance,
                                           Penalty = d.Penalty,
                                           LateInt = d.LateInt,
                                           Advance = d.Advance,
                                           Requirements = d.Requirements,
                                           InsuranceIPIorPPI = d.InsuranceIPIorPPI,
                                           NetAmount = d.NetAmount,
                                           AccountId = d.AccountId,
                                           Account = d.mstAccount.Account,
                                           Particulars = d.Particulars
                                       };

            return loanApplicationLines.ToList();
        }

        // add loan application lines
        [Authorize]
        [HttpPost]
        [Route("api/loanApplicationLines/add")]
        public HttpResponseMessage addLoanApplicationLines(Models.TrnLoanApplicationLines loanApplicationLine)
        {
            try
            {
                var loanApplication = from d in db.trnLoanApplications where d.Id == loanApplicationLine.LoanId select d;
                if (loanApplication.Any())
                {
                    if (!loanApplication.FirstOrDefault().IsLocked)
                    {
                        Data.trnLoanApplicationLine newLoanApplicationLine = new Data.trnLoanApplicationLine();
                        newLoanApplicationLine.LoanId = loanApplicationLine.LoanId;
                        newLoanApplicationLine.Principal = loanApplicationLine.Principal;
                        newLoanApplicationLine.ProcessingFee = loanApplicationLine.ProcessingFee;
                        newLoanApplicationLine.Passbook = loanApplicationLine.Passbook;
                        newLoanApplicationLine.Balance = loanApplicationLine.Balance;
                        newLoanApplicationLine.Penalty = loanApplicationLine.Penalty;
                        newLoanApplicationLine.LateInt = loanApplicationLine.LateInt;
                        newLoanApplicationLine.Advance = loanApplicationLine.Advance;
                        newLoanApplicationLine.Requirements = loanApplicationLine.Requirements;
                        newLoanApplicationLine.InsuranceIPIorPPI = loanApplicationLine.InsuranceIPIorPPI;
                        newLoanApplicationLine.NetAmount = loanApplicationLine.NetAmount;
                        newLoanApplicationLine.AccountId = loanApplicationLine.AccountId;
                        newLoanApplicationLine.Particulars = loanApplicationLine.Particulars;
                        db.trnLoanApplicationLines.InsertOnSubmit(newLoanApplicationLine);
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

        // update loan application lines
        [Authorize]
        [HttpPut]
        [Route("api/loanApplicationLines/update/{id}")]
        public HttpResponseMessage updateLoanApplicationLines(String id, Models.TrnLoanApplicationLines loanApplicationLine)
        {
            try
            {
                var loanApplication = from d in db.trnLoanApplications where d.Id == loanApplicationLine.LoanId select d;
                if (loanApplication.Any())
                {
                    if (!loanApplication.FirstOrDefault().IsLocked)
                    {
                        var loanApplicationLines = from d in db.trnLoanApplicationLines where d.Id == Convert.ToInt32(id) select d;
                        if (loanApplicationLines.Any())
                        {
                            var updateLoanApplicationLine = loanApplicationLines.FirstOrDefault();
                            updateLoanApplicationLine.LoanId = loanApplicationLine.LoanId;
                            updateLoanApplicationLine.Principal = loanApplicationLine.Principal;
                            updateLoanApplicationLine.ProcessingFee = loanApplicationLine.ProcessingFee;
                            updateLoanApplicationLine.Passbook = loanApplicationLine.Passbook;
                            updateLoanApplicationLine.Balance = loanApplicationLine.Balance;
                            updateLoanApplicationLine.Penalty = loanApplicationLine.Penalty;
                            updateLoanApplicationLine.LateInt = loanApplicationLine.LateInt;
                            updateLoanApplicationLine.Advance = loanApplicationLine.Advance;
                            updateLoanApplicationLine.Requirements = loanApplicationLine.Requirements;
                            updateLoanApplicationLine.InsuranceIPIorPPI = loanApplicationLine.InsuranceIPIorPPI;
                            updateLoanApplicationLine.NetAmount = loanApplicationLine.NetAmount;
                            updateLoanApplicationLine.AccountId = loanApplicationLine.AccountId;
                            updateLoanApplicationLine.Particulars = loanApplicationLine.Particulars;
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

        // delete loan application lines
        [Authorize]
        [HttpDelete]
        [Route("api/loanApplicationLines/delete/{id}/{loanId}")]
        public HttpResponseMessage deleteLoanApplicationLines(String id, String loanId)
        {
            try
            {
                var loanApplication = from d in db.trnLoanApplications where d.Id == Convert.ToInt32(loanId) select d;
                if (loanApplication.Any())
                {
                    if (!loanApplication.FirstOrDefault().IsLocked)
                    {
                        var loanApplicationLines = from d in db.trnLoanApplicationLines where d.Id == Convert.ToInt32(id) select d;
                        if (loanApplicationLines.Any())
                        {
                            db.trnLoanApplicationLines.DeleteOnSubmit(loanApplicationLines.First());
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
