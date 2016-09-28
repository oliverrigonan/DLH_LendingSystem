using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiLoanApplicationController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan application list
        [Authorize]
        [HttpGet]
        [Route("api/loanApplication/listByLoanDate/{loanDate}")]
        public List<Models.TrnLoanApplication> listLoanApplicationByLoanDate(String loanDate)
        {
            var loanApplications = from d in db.trnLoanApplications.OrderByDescending(d => d.Id)
                                   where d.LoanDate == Convert.ToDateTime(loanDate)
                                   select new Models.TrnLoanApplication
                                   {
                                       Id = d.Id,
                                       LoanNumber = d.LoanNumber,
                                       LoanDate = d.LoanDate.ToShortDateString(),
                                       MaturityDate = d.MaturityDate.ToShortDateString(),
                                       AccountId = d.AccountId,
                                       Account = d.mstAccount.Account,
                                       ApplicantId = d.ApplicantId,
                                       Applicant = d.mstApplicant.ApplicantFullName,
                                       AreaId = d.AreaId,
                                       Area = d.mstArea.Area,
                                       Particulars = d.Particulars,
                                       PrincipalAmount = d.PrincipalAmount,
                                       ProcessingFee = d.ProcessingFee,
                                       Passbook = d.Passbook,
                                       Penalty = d.Penalty,
                                       LateInt = d.LateInt,
                                       Advance = d.Advance,
                                       Requirements = d.Requirements,
                                       InsuranceIPI = d.InsuranceIPI,
                                       InsurancePPI = d.InsurancePPI,
                                       LoanAmount = d.LoanAmount,
                                       PaidAmount = d.PaidAmount,
                                       BalanceAmount = d.BalanceAmount,
                                       CollectorId = d.CollectorId,
                                       Collector = d.mstCollector.Collector,
                                       PreparedByUserId = d.PreparedByUserId,
                                       PreparedByUser = d.mstUser.FullName,
                                       IsLocked = d.IsLocked,
                                       CreatedByUserId = d.CreatedByUserId,
                                       CreatedByUser = d.mstUser1.FullName,
                                       CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                       UpdatedByUserId = d.UpdatedByUserId,
                                       UpdatedByUser = d.mstUser2.FullName,
                                       UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                                   };

            return loanApplications.ToList();
        }

        // loan application get by id
        [Authorize]
        [HttpGet]
        [Route("api/loanApplication/getById/{id}")]
        public Models.TrnLoanApplication getLoanApplicationById(String id)
        {
            var loanApplication = from d in db.trnLoanApplications
                                  where d.Id == Convert.ToInt32(id)
                                  select new Models.TrnLoanApplication
                                  {
                                      Id = d.Id,
                                      LoanNumber = d.LoanNumber,
                                      LoanDate = d.LoanDate.ToShortDateString(),
                                      MaturityDate = d.MaturityDate.ToShortDateString(),
                                      AccountId = d.AccountId,
                                      Account = d.mstAccount.Account,
                                      ApplicantId = d.ApplicantId,
                                      Applicant = d.mstApplicant.ApplicantFullName,
                                      AreaId = d.AreaId,
                                      Area = d.mstArea.Area,
                                      Particulars = d.Particulars,
                                      PrincipalAmount = d.PrincipalAmount,
                                      ProcessingFee = d.ProcessingFee,
                                      Passbook = d.Passbook,
                                      Penalty = d.Penalty,
                                      LateInt = d.LateInt,
                                      Advance = d.Advance,
                                      Requirements = d.Requirements,
                                      InsuranceIPI = d.InsuranceIPI,
                                      InsurancePPI = d.InsurancePPI,
                                      LoanAmount = d.LoanAmount,
                                      PaidAmount = d.PaidAmount,
                                      BalanceAmount = d.BalanceAmount,
                                      CollectorId = d.CollectorId,
                                      Collector = d.mstCollector.Collector,
                                      PreparedByUserId = d.PreparedByUserId,
                                      PreparedByUser = d.mstUser.FullName,
                                      IsLocked = d.IsLocked,
                                      CreatedByUserId = d.CreatedByUserId,
                                      CreatedByUser = d.mstUser1.FullName,
                                      CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                      UpdatedByUserId = d.UpdatedByUserId,
                                      UpdatedByUser = d.mstUser2.FullName,
                                      UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                                  };

            return (Models.TrnLoanApplication)loanApplication.FirstOrDefault();
        }

        // loan application list by applicantId
        [Authorize]
        [HttpGet]
        [Route("api/loanApplication/listByApplicantId/{applicantId}")]
        public List<Models.TrnLoanApplication> listLoanApplicationByApplicantId(String applicantId)
        {
            var loanApplication = from d in db.trnLoanApplications
                                  where d.ApplicantId == Convert.ToInt32(applicantId)
                                  select new Models.TrnLoanApplication
                                  {
                                      Id = d.Id,
                                      LoanNumber = d.LoanNumber,
                                      LoanDate = d.LoanDate.ToShortDateString(),
                                      MaturityDate = d.MaturityDate.ToShortDateString(),
                                      AccountId = d.AccountId,
                                      Account = d.mstAccount.Account,
                                      ApplicantId = d.ApplicantId,
                                      Applicant = d.mstApplicant.ApplicantFullName,
                                      AreaId = d.AreaId,
                                      Area = d.mstArea.Area,
                                      Particulars = d.Particulars,
                                      PrincipalAmount = d.PrincipalAmount,
                                      ProcessingFee = d.ProcessingFee,
                                      Passbook = d.Passbook,
                                      Penalty = d.Penalty,
                                      LateInt = d.LateInt,
                                      Advance = d.Advance,
                                      Requirements = d.Requirements,
                                      InsuranceIPI = d.InsuranceIPI,
                                      InsurancePPI = d.InsurancePPI,
                                      LoanAmount = d.LoanAmount,
                                      PaidAmount = d.PaidAmount,
                                      BalanceAmount = d.BalanceAmount,
                                      CollectorId = d.CollectorId,
                                      Collector = d.mstCollector.Collector,
                                      PreparedByUserId = d.PreparedByUserId,
                                      PreparedByUser = d.mstUser.FullName,
                                      IsLocked = d.IsLocked,
                                      CreatedByUserId = d.CreatedByUserId,
                                      CreatedByUser = d.mstUser1.FullName,
                                      CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                      UpdatedByUserId = d.UpdatedByUserId,
                                      UpdatedByUser = d.mstUser2.FullName,
                                      UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                                  };

            return loanApplication.ToList();
        }


        // zero fill
        public String zeroFill(Int32 number, Int32 length)
        {
            var result = number.ToString();
            var pad = length - result.Length; 
            while (pad > 0) {
                result = "0" + result;
                pad--;
            }

            return result;
        }

        // add loan application
        [Authorize]
        [HttpPost]
        [Route("api/loanApplication/add")]
        public Int32 addLoanApplication()
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                String loanNumber = "0000000001";
                var loanApplication = from d in db.trnLoanApplications.OrderByDescending(d => d.Id) select d;
                if(loanApplication.Any()) {
                    var newLoanNumber = Convert.ToInt32(loanApplication.FirstOrDefault().LoanNumber) + 0000000001;
                    loanNumber = newLoanNumber.ToString();
                }

                Data.trnLoanApplication newLoanApplication = new Data.trnLoanApplication();
                newLoanApplication.LoanNumber = zeroFill(Convert.ToInt32(loanNumber), 10);
                newLoanApplication.LoanDate = DateTime.Today;
                newLoanApplication.MaturityDate = DateTime.Today;
                newLoanApplication.AccountId = (from d in db.mstAccounts.OrderByDescending(d => d.Id) where d.AccountTransactionTypeId == 1 select d.Id).FirstOrDefault();
                newLoanApplication.ApplicantId = (from d in db.mstApplicants.OrderByDescending(d => d.Id) select d.Id).FirstOrDefault();
                newLoanApplication.AreaId = (from d in db.mstAreas.OrderByDescending(d => d.Id) select d.Id).FirstOrDefault();
                newLoanApplication.Particulars = "NA";
                newLoanApplication.PrincipalAmount = 0;
                newLoanApplication.ProcessingFee = 0;
                newLoanApplication.Passbook = 0;
                newLoanApplication.Penalty = 0;
                newLoanApplication.LateInt = 0;
                newLoanApplication.Advance = 0;
                newLoanApplication.Requirements = 0;
                newLoanApplication.InsuranceIPI = 0;
                newLoanApplication.InsurancePPI = 0;
                newLoanApplication.LoanAmount = 0;
                newLoanApplication.PaidAmount = 0;
                newLoanApplication.BalanceAmount = 0;
                newLoanApplication.CollectorId = (from d in db.mstCollectors select d.Id).FirstOrDefault();
                newLoanApplication.PreparedByUserId = userId;
                newLoanApplication.IsLocked = false;
                newLoanApplication.CreatedByUserId = userId;
                newLoanApplication.CreatedDateTime = DateTime.Now;
                newLoanApplication.UpdatedByUserId = userId;
                newLoanApplication.UpdatedDateTime = DateTime.Now;

                db.trnLoanApplications.InsertOnSubmit(newLoanApplication);
                db.SubmitChanges();

                return newLoanApplication.Id;
            }
            catch
            {
                return 0;
            }
        }

        // lock loan application
        [Authorize]
        [HttpPut]
        [Route("api/loanApplication/lock/{id}")]
        public HttpResponseMessage lockLoanApplication(String id, Models.TrnLoanApplication loanApplication)
        {
            try
            {
                var loanApplications = from d in db.trnLoanApplications where d.Id == Convert.ToInt32(id) select d;
                if (loanApplications.Any())
                {
                    if (!loanApplications.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                        Decimal totalCollectionLinesPaidAmount = 0;
                        var collectionLines = from d in db.trnCollectionLines where d.LoanId == Convert.ToInt32(id) where d.trnCollection.IsLocked == true select d;
                        if (collectionLines.Any())
                        {
                            totalCollectionLinesPaidAmount = collectionLines.Sum(d => d.Amount);
                        }

                        var lockLoanApplication = loanApplications.FirstOrDefault();
                        lockLoanApplication.LoanDate = Convert.ToDateTime(loanApplication.LoanDate);
                        lockLoanApplication.MaturityDate = Convert.ToDateTime(loanApplication.MaturityDate);
                        lockLoanApplication.AccountId = loanApplication.AccountId;
                        lockLoanApplication.ApplicantId = loanApplication.ApplicantId;
                        lockLoanApplication.AreaId = loanApplication.AreaId;
                        lockLoanApplication.Particulars = loanApplication.Particulars;
                        lockLoanApplication.PrincipalAmount = loanApplication.PrincipalAmount;
                        lockLoanApplication.ProcessingFee = loanApplication.PrincipalAmount * Convert.ToDecimal(0.03);
                        lockLoanApplication.Passbook = loanApplication.Passbook;
                        lockLoanApplication.Penalty = loanApplication.Penalty;
                        lockLoanApplication.LateInt = loanApplication.LateInt;
                        lockLoanApplication.Advance = loanApplication.Advance;
                        lockLoanApplication.Requirements = loanApplication.Requirements;
                        lockLoanApplication.InsuranceIPI = loanApplication.InsuranceIPI;
                        lockLoanApplication.InsurancePPI = loanApplication.InsurancePPI;
                        lockLoanApplication.LoanAmount = loanApplication.PrincipalAmount - (loanApplication.PrincipalAmount * Convert.ToDecimal(0.03)) - loanApplication.Passbook - loanApplication.Penalty - loanApplication.LateInt - loanApplication.Advance - loanApplication.Requirements - loanApplication.InsuranceIPI - loanApplication.InsurancePPI;
                        lockLoanApplication.PaidAmount = totalCollectionLinesPaidAmount;
                        lockLoanApplication.BalanceAmount = loanApplication.LoanAmount - totalCollectionLinesPaidAmount;
                        lockLoanApplication.CollectorId = loanApplication.CollectorId;
                        lockLoanApplication.PreparedByUserId = loanApplication.PreparedByUserId;
                        lockLoanApplication.IsLocked = true;
                        lockLoanApplication.UpdatedByUserId = userId;
                        lockLoanApplication.UpdatedDateTime = DateTime.Now;
                        db.SubmitChanges();

                        Business.Journal journal = new Business.Journal();
                        journal.postLoanJournal(Convert.ToInt32(id));

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

        // unlock loan application
        [Authorize]
        [HttpPut]
        [Route("api/loanApplication/unlock/{id}")]
        public HttpResponseMessage unlockLoanApplication(String id)
        {
            try
            {
                var loanApplications = from d in db.trnLoanApplications where d.Id == Convert.ToInt32(id) select d;
                if (loanApplications.Any())
                {
                    if (loanApplications.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                        
                        var unlockLoanApplication = loanApplications.FirstOrDefault();
                        unlockLoanApplication.IsLocked = false;
                        unlockLoanApplication.UpdatedByUserId = userId;
                        unlockLoanApplication.UpdatedDateTime = DateTime.Now;
                        db.SubmitChanges();

                        Business.Journal journal = new Business.Journal();
                        journal.deleteLoanJournal(Convert.ToInt32(id));

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

        // delete loan application
        [Authorize]
        [HttpDelete]
        [Route("api/loanApplication/delete/{id}")]
        public HttpResponseMessage deleteLoanApplication(String id)
        {
            try
            {
                var loanApplications = from d in db.trnLoanApplications where d.Id == Convert.ToInt32(id) select d;
                if (loanApplications.Any())
                {
                    if (!loanApplications.FirstOrDefault().IsLocked)
                    {
                        db.trnLoanApplications.DeleteOnSubmit(loanApplications.First());
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
    }
}
