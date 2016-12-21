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
            var loanApplications = from d in db.trnLoanApplications
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
                                       LoanTypeId = d.LoanTypeId,
                                       LoanType = d.mstLoanType.LoanType,
                                       PreparedByUserId = d.PreparedByUserId,
                                       TermId = d.TermId,
                                       Term = d.mstTerm.Term,
                                       InterestId = d.InterestId,
                                       Interest = d.mstInterest.Interest,
                                       InterestRate = d.InterestRate,
                                       PenaltyId = d.PenaltyId,
                                       Penalty = d.mstPenalty.Penalty,
                                       PreparedByUser = d.mstUser.FullName,
                                       PrincipalAmount = d.PrincipalAmount,
                                       ProcessingFeeAmountDeduction = d.ProcessingFeeAmountDeduction,
                                       PassbookAmountDeduction = d.PassbookAmountDeduction,
                                       BalanceAmountDeduction = d.BalanceAmountDeduction,
                                       PenaltyAmountDeduction = d.PenaltyAmountDeduction,
                                       LateIntAmountDeduction = d.LateIntAmountDeduction,
                                       AdvanceAmountDeduction = d.AdvanceAmountDeduction,
                                       RequirementsAmountDeduction = d.RequirementsAmountDeduction,
                                       InsuranceIPIorPPIAmountDeduction = d.InsuranceIPIorPPIAmountDeduction,
                                       NetAmount = d.NetAmount,
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
                                      LoanTypeId = d.LoanTypeId,
                                      LoanType = d.mstLoanType.LoanType,
                                      PreparedByUserId = d.PreparedByUserId,
                                      TermId = d.TermId,
                                      Term = d.mstTerm.Term,
                                      InterestId = d.InterestId,
                                      Interest = d.mstInterest.Interest,
                                      InterestRate = d.InterestRate,
                                      PenaltyId = d.PenaltyId,
                                      Penalty = d.mstPenalty.Penalty,
                                      PreparedByUser = d.mstUser.FullName,
                                      PrincipalAmount = d.PrincipalAmount,
                                      ProcessingFeeAmountDeduction = d.ProcessingFeeAmountDeduction,
                                      PassbookAmountDeduction = d.PassbookAmountDeduction,
                                      BalanceAmountDeduction = d.BalanceAmountDeduction,
                                      PenaltyAmountDeduction = d.PenaltyAmountDeduction,
                                      LateIntAmountDeduction = d.LateIntAmountDeduction,
                                      AdvanceAmountDeduction = d.AdvanceAmountDeduction,
                                      RequirementsAmountDeduction = d.RequirementsAmountDeduction,
                                      InsuranceIPIorPPIAmountDeduction = d.InsuranceIPIorPPIAmountDeduction,
                                      NetAmount = d.NetAmount,
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
                                      Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                      Area = d.mstApplicant.mstArea.Area,
                                      Particulars = d.Particulars,
                                      LoanTypeId = d.LoanTypeId,
                                      LoanType = d.mstLoanType.LoanType,
                                      PreparedByUserId = d.PreparedByUserId,
                                      TermId = d.TermId,
                                      Term = d.mstTerm.Term,
                                      InterestId = d.InterestId,
                                      Interest = d.mstInterest.Interest,
                                      InterestRate = d.InterestRate,
                                      PenaltyId = d.PenaltyId,
                                      Penalty = d.mstPenalty.Penalty,
                                      PreparedByUser = d.mstUser.FullName,
                                      PrincipalAmount = d.PrincipalAmount,
                                      ProcessingFeeAmountDeduction = d.ProcessingFeeAmountDeduction,
                                      PassbookAmountDeduction = d.PassbookAmountDeduction,
                                      BalanceAmountDeduction = d.BalanceAmountDeduction,
                                      PenaltyAmountDeduction = d.PenaltyAmountDeduction,
                                      LateIntAmountDeduction = d.LateIntAmountDeduction,
                                      AdvanceAmountDeduction = d.AdvanceAmountDeduction,
                                      RequirementsAmountDeduction = d.RequirementsAmountDeduction,
                                      InsuranceIPIorPPIAmountDeduction = d.InsuranceIPIorPPIAmountDeduction,
                                      NetAmount = d.NetAmount,
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
            while (pad > 0)
            {
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
                    String matchPageString = "LoanApplicationList";
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
                        String loanNumber = "0000000001";
                        var loanApplication = from d in db.trnLoanApplications.OrderByDescending(d => d.Id) select d;
                        if (loanApplication.Any())
                        {
                            var newLoanNumber = Convert.ToInt32(loanApplication.FirstOrDefault().LoanNumber) + 0000000001;
                            loanNumber = newLoanNumber.ToString();
                        }

                        var termId = (from d in db.mstTerms select d.Id).FirstOrDefault();
                        Data.trnLoanApplication newLoanApplication = new Data.trnLoanApplication();
                        newLoanApplication.LoanNumber = zeroFill(Convert.ToInt32(loanNumber), 10);
                        newLoanApplication.LoanDate = DateTime.Today;
                        newLoanApplication.MaturityDate = DateTime.Today.AddDays(60);
                        newLoanApplication.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 1 select d.Id).FirstOrDefault();
                        newLoanApplication.ApplicantId = (from d in db.mstApplicants select d.Id).FirstOrDefault();
                        newLoanApplication.Particulars = "NA";
                        newLoanApplication.LoanTypeId = (from d in db.mstLoanTypes select d.Id).FirstOrDefault();
                        newLoanApplication.PreparedByUserId = userId;
                        newLoanApplication.TermId = termId;
                        newLoanApplication.InterestId = (from d in db.mstInterests select d.Id).FirstOrDefault();
                        newLoanApplication.InterestRate = (from d in db.mstInterests select d.Rate).FirstOrDefault();
                        newLoanApplication.PenaltyId = (from d in db.mstPenalties select d.Id).FirstOrDefault();
                        newLoanApplication.PrincipalAmount = 0;
                        newLoanApplication.ProcessingFeeAmountDeduction = 0;
                        newLoanApplication.PassbookAmountDeduction = 0;
                        newLoanApplication.BalanceAmountDeduction = 0;
                        newLoanApplication.PenaltyAmountDeduction = 0;
                        newLoanApplication.LateIntAmountDeduction = 0;
                        newLoanApplication.AdvanceAmountDeduction = 0;
                        newLoanApplication.RequirementsAmountDeduction = 0;
                        newLoanApplication.InsuranceIPIorPPIAmountDeduction = 0;
                        newLoanApplication.NetAmount = 0;
                        newLoanApplication.IsLocked = false;
                        newLoanApplication.CreatedByUserId = userId;
                        newLoanApplication.CreatedDateTime = DateTime.Now;
                        newLoanApplication.UpdatedByUserId = userId;
                        newLoanApplication.UpdatedDateTime = DateTime.Now;
                        db.trnLoanApplications.InsertOnSubmit(newLoanApplication);
                        db.SubmitChanges();

                        return newLoanApplication.Id;
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
                                if (Convert.ToDateTime(loanApplication.LoanDate) > Convert.ToDateTime(loanApplication.MaturityDate))
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                                }
                                else
                                {
                                    var lockLoanApplication = loanApplications.FirstOrDefault();
                                    lockLoanApplication.LoanDate = Convert.ToDateTime(loanApplication.LoanDate);
                                    lockLoanApplication.MaturityDate = Convert.ToDateTime(loanApplication.MaturityDate);
                                    lockLoanApplication.AccountId = loanApplication.AccountId;
                                    lockLoanApplication.ApplicantId = loanApplication.ApplicantId;
                                    lockLoanApplication.Particulars = loanApplication.Particulars;
                                    lockLoanApplication.LoanTypeId = loanApplication.LoanTypeId;
                                    lockLoanApplication.PreparedByUserId = loanApplication.PreparedByUserId;
                                    lockLoanApplication.TermId = loanApplication.TermId;
                                    lockLoanApplication.InterestId = loanApplication.InterestId;
                                    lockLoanApplication.InterestRate = loanApplication.InterestRate;
                                    lockLoanApplication.PenaltyId = loanApplication.PenaltyId;
                                    lockLoanApplication.PrincipalAmount = loanApplication.PrincipalAmount;
                                    lockLoanApplication.ProcessingFeeAmountDeduction = loanApplication.ProcessingFeeAmountDeduction;
                                    lockLoanApplication.PassbookAmountDeduction = loanApplication.PassbookAmountDeduction;
                                    lockLoanApplication.BalanceAmountDeduction = loanApplication.BalanceAmountDeduction;
                                    lockLoanApplication.PenaltyAmountDeduction = loanApplication.PenaltyAmountDeduction;
                                    lockLoanApplication.LateIntAmountDeduction = loanApplication.LateIntAmountDeduction;
                                    lockLoanApplication.AdvanceAmountDeduction = loanApplication.AdvanceAmountDeduction;
                                    lockLoanApplication.RequirementsAmountDeduction = loanApplication.RequirementsAmountDeduction;
                                    lockLoanApplication.InsuranceIPIorPPIAmountDeduction = loanApplication.InsuranceIPIorPPIAmountDeduction;
                                    lockLoanApplication.NetAmount = loanApplication.NetAmount;
                                    lockLoanApplication.IsLocked = true;
                                    lockLoanApplication.UpdatedByUserId = userId;
                                    lockLoanApplication.UpdatedDateTime = DateTime.Now;
                                    db.SubmitChanges();

                                    Business.Collection collection = new Business.Collection();
                                    collection.postCollection(Convert.ToInt32(id), userId);

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
                        var dailyCollections = from d in db.trnDailyCollections
                                               where d.trnCollection.LoanId == Convert.ToInt32(id)
                                               && d.IsProcessed == true
                                               select d;

                        if (!dailyCollections.Any())
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
                                    var unlockLoanApplication = loanApplications.FirstOrDefault();
                                    unlockLoanApplication.IsLocked = false;
                                    unlockLoanApplication.UpdatedByUserId = userId;
                                    unlockLoanApplication.UpdatedDateTime = DateTime.Now;
                                    db.SubmitChanges();

                                    Business.Collection collection = new Business.Collection();
                                    collection.deleteCollection(Convert.ToInt32(id));

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
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
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
