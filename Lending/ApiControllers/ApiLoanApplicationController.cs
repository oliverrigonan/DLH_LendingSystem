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
                                       BranchId = d.BranchId,
                                       Branch = d.mstBranch.Branch,
                                       AccountId = d.AccountId,
                                       Account = d.mstAccount.Account,
                                       ApplicantId = d.ApplicantId,
                                       Applicant = d.mstApplicant.ApplicantFullName,
                                       AreaId = d.AreaId,
                                       Area = d.mstArea.Area,
                                       Promises = d.Promises,
                                       Particulars = d.Particulars,
                                       LoanAmount = d.LoanAmount,
                                       PaidAmount = d.PaidAmount,
                                       BalanceAmount = d.BalanceAmount,
                                       CollectorId = d.CollectorId,
                                       Collector = d.mstCollector.Collector,
                                       PreparedByUserId = d.PreparedByUserId,
                                       PreparedByUser = d.mstUser.FullName,
                                       VerifiedByUserId = d.VerifiedByUserId,
                                       VerifiedByUser = d.mstUser1.FullName,
                                       IsLocked = d.IsLocked,
                                       CreatedByUserId = d.CreatedByUserId,
                                       CreatedByUser = d.mstUser2.FullName,
                                       CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                       UpdatedByUserId = d.UpdatedByUserId,
                                       UpdatedByUser = d.mstUser3.FullName,
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
                                      BranchId = d.BranchId,
                                      Branch = d.mstBranch.Branch,
                                      AccountId = d.AccountId,
                                      Account = d.mstAccount.Account,
                                      ApplicantId = d.ApplicantId,
                                      Applicant = d.mstApplicant.ApplicantFullName,
                                      AreaId = d.AreaId,
                                      Area = d.mstArea.Area,
                                      Promises = d.Promises,
                                      Particulars = d.Particulars,
                                      LoanAmount = d.LoanAmount,
                                      PaidAmount = d.PaidAmount,
                                      BalanceAmount = d.BalanceAmount,
                                      CollectorId = d.CollectorId,
                                      Collector = d.mstCollector.Collector,
                                      PreparedByUserId = d.PreparedByUserId,
                                      PreparedByUser = d.mstUser.FullName,
                                      VerifiedByUserId = d.VerifiedByUserId,
                                      VerifiedByUser = d.mstUser1.FullName,
                                      IsLocked = d.IsLocked,
                                      CreatedByUserId = d.CreatedByUserId,
                                      CreatedByUser = d.mstUser2.FullName,
                                      CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                      UpdatedByUserId = d.UpdatedByUserId,
                                      UpdatedByUser = d.mstUser3.FullName,
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
                                  && d.IsLocked == true
                                  && d.BalanceAmount > 0
                                  select new Models.TrnLoanApplication
                                  {
                                      Id = d.Id,
                                      LoanNumber = d.LoanNumber,
                                      LoanDate = d.LoanDate.ToShortDateString(),
                                      MaturityDate = d.MaturityDate.ToShortDateString(),
                                      BranchId = d.BranchId,
                                      Branch = d.mstBranch.Branch,
                                      AccountId = d.AccountId,
                                      Account = d.mstAccount.Account,
                                      ApplicantId = d.ApplicantId,
                                      Applicant = d.mstApplicant.ApplicantFullName,
                                      AreaId = d.AreaId,
                                      Area = d.mstArea.Area,
                                      Promises = d.Promises,
                                      Particulars = d.Particulars,
                                      LoanAmount = d.LoanAmount,
                                      PaidAmount = d.PaidAmount,
                                      BalanceAmount = d.BalanceAmount,
                                      CollectorId = d.CollectorId,
                                      Collector = d.mstCollector.Collector,
                                      PreparedByUserId = d.PreparedByUserId,
                                      PreparedByUser = d.mstUser.FullName,
                                      VerifiedByUserId = d.VerifiedByUserId,
                                      VerifiedByUser = d.mstUser1.FullName,
                                      IsLocked = d.IsLocked,
                                      CreatedByUserId = d.CreatedByUserId,
                                      CreatedByUser = d.mstUser2.FullName,
                                      CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                      UpdatedByUserId = d.UpdatedByUserId,
                                      UpdatedByUser = d.mstUser3.FullName,
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
                newLoanApplication.BranchId = (from d in db.mstBranches select d.Id).FirstOrDefault();
                newLoanApplication.AccountId = (from d in db.mstAccounts select d.Id).FirstOrDefault();
                newLoanApplication.ApplicantId = (from d in db.mstApplicants select d.Id).FirstOrDefault();
                newLoanApplication.AreaId = (from d in db.mstAreas select d.Id).FirstOrDefault();
                newLoanApplication.Promises = "NA";
                newLoanApplication.Particulars = "NA";
                newLoanApplication.LoanAmount = 0;
                newLoanApplication.PaidAmount = 0;
                newLoanApplication.BalanceAmount = 0;
                newLoanApplication.CollectorId = (from d in db.mstCollectors select d.Id).FirstOrDefault();
                newLoanApplication.PreparedByUserId = userId;
                newLoanApplication.VerifiedByUserId = userId;
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
                        lockLoanApplication.BranchId = loanApplication.BranchId;
                        lockLoanApplication.AccountId = loanApplication.AccountId;
                        lockLoanApplication.ApplicantId = loanApplication.ApplicantId;
                        lockLoanApplication.AreaId = loanApplication.AreaId;
                        lockLoanApplication.Promises = loanApplication.Promises;
                        lockLoanApplication.Particulars = loanApplication.Particulars;
                        lockLoanApplication.LoanAmount = loanApplication.LoanAmount;
                        lockLoanApplication.PaidAmount = totalCollectionLinesPaidAmount;
                        lockLoanApplication.BalanceAmount = loanApplication.LoanAmount - totalCollectionLinesPaidAmount;
                        lockLoanApplication.CollectorId = loanApplication.CollectorId;
                        lockLoanApplication.PreparedByUserId = loanApplication.PreparedByUserId;
                        lockLoanApplication.VerifiedByUserId = loanApplication.VerifiedByUserId;
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
