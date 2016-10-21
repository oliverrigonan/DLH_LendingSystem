using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Diagnostics;

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
                                       Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                       Area = d.mstApplicant.mstArea.Area,
                                       Particulars = d.Particulars,
                                       PreparedByUserId = d.PreparedByUserId,
                                       PreparedByUser = d.mstUser.FullName,
                                       CollectorId = d.CollectorId,
                                       Collector = d.mstCollector.Collector,
                                       CollectorAreaAssigned = d.mstCollector.mstArea.Area,
                                       PrincipalAmount = d.PrincipalAmount,
                                       ProcessingFeeAmount = d.ProcessingFeeAmount,
                                       PassbookAmount = d.PassbookAmount,
                                       BalanceAmount = d.BalanceAmount,
                                       PenaltyAmount = d.PenaltyAmount,
                                       LateIntAmount = d.LateIntAmount,
                                       AdvanceAmount = d.AdvanceAmount,
                                       RequirementsAmount = d.RequirementsAmount,
                                       InsuranceIPIorPPIAmount = d.InsuranceIPIorPPIAmount,
                                       NetAmount = d.NetAmount,
                                       IsFullyPaid = d.IsFullyPaid,
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
                                      Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                      Area = d.mstApplicant.mstArea.Area,
                                      Particulars = d.Particulars,
                                      PreparedByUserId = d.PreparedByUserId,
                                      PreparedByUser = d.mstUser.FullName,
                                      CollectorId = d.CollectorId,
                                      Collector = d.mstCollector.Collector,
                                      CollectorAreaAssigned = d.mstCollector.mstArea.Area,
                                      PrincipalAmount = d.PrincipalAmount,
                                      ProcessingFeeAmount = d.ProcessingFeeAmount,
                                      PassbookAmount = d.PassbookAmount,
                                      BalanceAmount = d.BalanceAmount,
                                      PenaltyAmount = d.PenaltyAmount,
                                      LateIntAmount = d.LateIntAmount,
                                      AdvanceAmount = d.AdvanceAmount,
                                      RequirementsAmount = d.RequirementsAmount,
                                      InsuranceIPIorPPIAmount = d.InsuranceIPIorPPIAmount,
                                      NetAmount = d.NetAmount,
                                      IsLocked = d.IsLocked,
                                      IsFullyPaid = d.IsFullyPaid,
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
                                      Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                      Area = d.mstApplicant.mstArea.Area,
                                      Particulars = d.Particulars,
                                      PreparedByUserId = d.PreparedByUserId,
                                      PreparedByUser = d.mstUser.FullName,
                                      CollectorId = d.CollectorId,
                                      Collector = d.mstCollector.Collector,
                                      CollectorAreaAssigned = d.mstCollector.mstArea.Area,
                                      PrincipalAmount = d.PrincipalAmount,
                                      ProcessingFeeAmount = d.ProcessingFeeAmount,
                                      PassbookAmount = d.PassbookAmount,
                                      BalanceAmount = d.BalanceAmount,
                                      PenaltyAmount = d.PenaltyAmount,
                                      LateIntAmount = d.LateIntAmount,
                                      AdvanceAmount = d.AdvanceAmount,
                                      RequirementsAmount = d.RequirementsAmount,
                                      InsuranceIPIorPPIAmount = d.InsuranceIPIorPPIAmount,
                                      NetAmount = d.NetAmount,
                                      IsFullyPaid = d.IsFullyPaid,
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
                newLoanApplication.MaturityDate = DateTime.Today.AddMonths(2);
                newLoanApplication.AccountId = (from d in db.mstAccounts.OrderByDescending(d => d.Id) where d.AccountTransactionTypeId == 1 select d.Id).FirstOrDefault();
                newLoanApplication.ApplicantId = (from d in db.mstApplicants.OrderByDescending(d => d.Id) select d.Id).FirstOrDefault();
                newLoanApplication.Particulars = "NA";
                newLoanApplication.PreparedByUserId = userId;
                newLoanApplication.CollectorId = (from d in db.mstCollectors.OrderByDescending(d => d.Id) select d.Id).FirstOrDefault();
                newLoanApplication.PrincipalAmount = 0;
                newLoanApplication.ProcessingFeeAmount = 0;
                newLoanApplication.PassbookAmount = 0;
                newLoanApplication.BalanceAmount = 0;
                newLoanApplication.PenaltyAmount = 0;
                newLoanApplication.LateIntAmount = 0;
                newLoanApplication.AdvanceAmount = 0;
                newLoanApplication.RequirementsAmount = 0;
                newLoanApplication.InsuranceIPIorPPIAmount = 0;
                newLoanApplication.NetAmount = 0;
                newLoanApplication.IsFullyPaid = false;
                newLoanApplication.IsLocked = false;
                newLoanApplication.CreatedByUserId = userId;
                newLoanApplication.CreatedDateTime = DateTime.Now;
                newLoanApplication.UpdatedByUserId = userId;
                newLoanApplication.UpdatedDateTime = DateTime.Now;
                db.trnLoanApplications.InsertOnSubmit(newLoanApplication);
                db.SubmitChanges();

                return newLoanApplication.Id;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
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
                        if (Convert.ToDateTime(loanApplication.LoanDate) > Convert.ToDateTime(loanApplication.MaturityDate))
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                        else
                        {
                            var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                            var lockLoanApplication = loanApplications.FirstOrDefault();
                            lockLoanApplication.LoanDate = Convert.ToDateTime(loanApplication.LoanDate);
                            lockLoanApplication.MaturityDate = Convert.ToDateTime(loanApplication.MaturityDate);
                            lockLoanApplication.AccountId = loanApplication.AccountId;
                            lockLoanApplication.ApplicantId = loanApplication.ApplicantId;
                            lockLoanApplication.Particulars = loanApplication.Particulars;
                            lockLoanApplication.PreparedByUserId = loanApplication.PreparedByUserId;
                            lockLoanApplication.CollectorId = loanApplication.CollectorId;
                            lockLoanApplication.PrincipalAmount = loanApplication.PrincipalAmount;
                            lockLoanApplication.ProcessingFeeAmount = loanApplication.ProcessingFeeAmount;
                            lockLoanApplication.PassbookAmount = loanApplication.PassbookAmount;
                            lockLoanApplication.BalanceAmount = loanApplication.BalanceAmount;
                            lockLoanApplication.PenaltyAmount = loanApplication.PenaltyAmount;
                            lockLoanApplication.LateIntAmount = loanApplication.LateIntAmount;
                            lockLoanApplication.AdvanceAmount = loanApplication.AdvanceAmount;
                            lockLoanApplication.RequirementsAmount = loanApplication.RequirementsAmount;
                            lockLoanApplication.InsuranceIPIorPPIAmount = loanApplication.InsuranceIPIorPPIAmount;
                            lockLoanApplication.NetAmount = loanApplication.NetAmount;
                            lockLoanApplication.IsLocked = true;
                            lockLoanApplication.UpdatedByUserId = userId;
                            lockLoanApplication.UpdatedDateTime = DateTime.Now;
                            db.SubmitChanges();

                            Business.LoanLogHistory loanLogHistory = new Business.LoanLogHistory();
                            loanLogHistory.postLoanLogHistory(Convert.ToInt32(id));

                            Business.Journal journal = new Business.Journal();
                            journal.postLoanJournal(Convert.ToInt32(id));

                            return Request.CreateResponse(HttpStatusCode.OK);
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
                        var collectionLogHistory = from d in db.trnCollectionLogHistories where d.trnLoanLogHistory.LoanId == Convert.ToInt32(id) select d;
                        if (!collectionLogHistory.Any())
                        {
                            var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                            var unlockLoanApplication = loanApplications.FirstOrDefault();
                            unlockLoanApplication.IsLocked = false;
                            unlockLoanApplication.UpdatedByUserId = userId;
                            unlockLoanApplication.UpdatedDateTime = DateTime.Now;
                            db.SubmitChanges();

                            Business.LoanLogHistory loanLogHistory = new Business.LoanLogHistory();
                            loanLogHistory.deleteLoanLogHistory(Convert.ToInt32(id));

                            Business.Journal journal = new Business.Journal();
                            journal.deleteLoanJournal(Convert.ToInt32(id));

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot unlock the loan application because the collection has already been started.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot unlock the loan application because it is not yet locked.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server to unlock this loan application.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something went wrong from the server. Please contact the administrator.");
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
