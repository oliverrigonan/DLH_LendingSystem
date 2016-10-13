using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiLoanApplicationRequirementsController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan Requirements list
        [Authorize]
        [HttpGet]
        [Route("api/loanApplicationRequirements/listByLoanId/{loanId}")]
        public List<Models.TrnLoanApplicationRequirements> listLoanRequirementsByLoanId(String loanId)
        {
            var loanRequirements = from d in db.trnLoanApplicationRequirements.OrderByDescending(d => d.Id)
                                   where d.LoanId == Convert.ToInt32(loanId)
                                   select new Models.TrnLoanApplicationRequirements
                                   {
                                       Id = d.Id,
                                       LoanId = d.LoanId,
                                       RequirementId = d.RequirementId,
                                       Requirement = d.mstRequirement.Requirement,
                                       Note = d.Note,
                                   };

            return loanRequirements.ToList();
        }

        // add loan Requirements
        [Authorize]
        [HttpPost]
        [Route("api/loanApplicationRequirements/add")]
        public HttpResponseMessage addLoanRequirements(Models.TrnLoanApplicationRequirements loanRequirement)
        {
            try
            {
                var loanApplications = from d in db.trnLoanApplications where d.Id == loanRequirement.LoanId select d;
                if (loanApplications.Any())
                {
                    if (!loanApplications.FirstOrDefault().IsLocked)
                    {
                        Data.trnLoanApplicationRequirement newLoanRequirement = new Data.trnLoanApplicationRequirement();
                        newLoanRequirement.LoanId = loanRequirement.LoanId;
                        newLoanRequirement.RequirementId = loanRequirement.RequirementId;
                        newLoanRequirement.Note = loanRequirement.Note;
                        db.trnLoanApplicationRequirements.InsertOnSubmit(newLoanRequirement);
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

        // update loan Requirements
        [Authorize]
        [HttpPut]
        [Route("api/loanApplicationRequirements/update/{id}")]
        public HttpResponseMessage updateLoanRequirements(String id, Models.TrnLoanApplicationRequirements loanRequirement)
        {
            try
            {
                var loanApplications = from d in db.trnLoanApplications where d.Id == loanRequirement.LoanId select d;
                if (loanApplications.Any())
                {
                    if(!loanApplications.FirstOrDefault().IsLocked) 
                    {
                        var loanRequirements = from d in db.trnLoanApplicationRequirements where d.Id == Convert.ToInt32(id) select d;
                        if (loanRequirements.Any())
                        {
                            var updateLoanRequirement = loanRequirements.FirstOrDefault();
                            updateLoanRequirement.LoanId = loanRequirement.LoanId;
                            updateLoanRequirement.RequirementId = loanRequirement.RequirementId;
                            updateLoanRequirement.Note = loanRequirement.Note;
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

        // delete loan Requirements
        [Authorize]
        [HttpDelete]
        [Route("api/loanApplicationRequirements/delete/{id}")]
        public HttpResponseMessage deleteLoanRequirements(String id)
        {
            try
            {
                var loanRequirements = from d in db.trnLoanApplicationRequirements where d.Id == Convert.ToInt32(id) select d;
                if (loanRequirements.Any())
                {
                    var loanApplications = from d in db.trnLoanApplications where d.Id == loanRequirements.FirstOrDefault().LoanId select d;
                    if (loanApplications.Any())
                    {
                        if(!loanApplications.FirstOrDefault().IsLocked)
                        {
                            db.trnLoanApplicationRequirements.DeleteOnSubmit(loanRequirements.First());
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
