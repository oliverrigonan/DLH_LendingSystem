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
    public class ApiLoanController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan list by loan date
        [Authorize]
        [HttpGet]
        [Route("api/loan/listByLoanDate/{loanDate}")]
        public List<Models.TrnLoan> listLoanByLoanDate(String loanDate)
        {
            var loanApplications = from d in db.trnLoans
                                   where d.LoanDate == Convert.ToDateTime(loanDate)
                                   select new Models.TrnLoan
                                   {
                                       Id = d.Id,
                                       LoanNumber = d.LoanNumber,
                                       LoanDate = d.LoanDate.ToShortDateString(),
                                       MaturityDate = d.MaturityDate.ToShortDateString(),
                                       ApplicantId = d.ApplicantId,
                                       Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                       Area = d.mstApplicant.mstArea.Area,
                                       Particulars = d.Particulars,
                                       TermId = d.TermId,
                                       Term = d.mstTerm.Term,
                                       TermNoOfDays = d.TermNoOfDays,
                                       TermNoOfAllowanceDays = d.TermNoOfAllowanceDays,
                                       LoanEndDate = d.LoanEndDate.ToShortDateString(),
                                       PenaltyId = d.PenaltyId,
                                       Penalty = d.mstPenalty.Penalty,
                                       NoOfAbsent = d.NoOfAbsent,
                                       PreparedByUserId = d.PreparedByUserId,
                                       PreparedByUser = d.mstUser.FullName,
                                       PrincipalAmount = d.PrincipalAmount,
                                       InterestId = d.InterestId,
                                       Interest = d.mstInterest.Interest,
                                       InterestRate = d.InterestRate,
                                       InterestAmount = d.InterestAmount,
                                       DeductionAmount = d.DeductionAmount,
                                       NetAmount = d.NetAmount,
                                       DailyCollectibleAmount = d.DailyCollectibleAmount,
                                       CurrentCollectibeAmount = d.CurrentCollectibeAmount,
                                       PaidAmount = d.PaidAmount,
                                       NextPaidDate = d.NextPaidDate.ToShortDateString(),
                                       PenaltyAmount = d.PenaltyAmount,
                                       IsReconstruct = d.IsReconstruct,
                                       ReconstructInterestAmount = d.ReconstructInterestAmount,
                                       BalanceAmount = d.BalanceAmount,
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

        // loan get by id
        [Authorize]
        [HttpGet]
        [Route("api/loan/getById/{id}")]
        public Models.TrnLoan getLoanById(String id)
        {
            var loan = from d in db.trnLoans
                       where d.Id == Convert.ToInt32(id)
                       select new Models.TrnLoan
                       {
                           Id = d.Id,
                           LoanNumber = d.LoanNumber,
                           LoanDate = d.LoanDate.ToShortDateString(),
                           MaturityDate = d.MaturityDate.ToShortDateString(),
                           ApplicantId = d.ApplicantId,
                           Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                           Area = d.mstApplicant.mstArea.Area,
                           Particulars = d.Particulars,
                           TermId = d.TermId,
                           Term = d.mstTerm.Term,
                           TermNoOfDays = d.TermNoOfDays,
                           TermNoOfAllowanceDays = d.TermNoOfAllowanceDays,
                           LoanEndDate = d.LoanEndDate.ToShortDateString(),
                           PenaltyId = d.PenaltyId,
                           Penalty = d.mstPenalty.Penalty,
                           NoOfAbsent = d.NoOfAbsent,
                           PreparedByUserId = d.PreparedByUserId,
                           PreparedByUser = d.mstUser.FullName,
                           PrincipalAmount = d.PrincipalAmount,
                           InterestId = d.InterestId,
                           Interest = d.mstInterest.Interest,
                           InterestRate = d.InterestRate,
                           InterestAmount = d.InterestAmount,
                           DeductionAmount = d.DeductionAmount,
                           NetAmount = d.NetAmount,
                           DailyCollectibleAmount = d.DailyCollectibleAmount,
                           CurrentCollectibeAmount = d.CurrentCollectibeAmount,
                           PaidAmount = d.PaidAmount,
                           NextPaidDate = d.NextPaidDate.ToShortDateString(),
                           PenaltyAmount = d.PenaltyAmount,
                           IsReconstruct = d.IsReconstruct,
                           ReconstructInterestAmount = d.ReconstructInterestAmount,
                           BalanceAmount = d.BalanceAmount,
                           IsFullyPaid = d.IsFullyPaid,
                           IsLocked = d.IsLocked,
                           CreatedByUserId = d.CreatedByUserId,
                           CreatedByUser = d.mstUser1.FullName,
                           CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                           UpdatedByUserId = d.UpdatedByUserId,
                           UpdatedByUser = d.mstUser2.FullName,
                           UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                       };

            return (Models.TrnLoan)loan.FirstOrDefault();
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
        [Route("api/loan/add")]
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
                        var loan = from d in db.trnLoans.OrderByDescending(d => d.Id) select d;
                        if (loan.Any())
                        {
                            var newLoanNumber = Convert.ToInt32(loan.FirstOrDefault().LoanNumber) + 0000000001;
                            loanNumber = newLoanNumber.ToString();
                        }

                        var term = from d in db.mstTerms select d;
                        var interest = from d in db.mstInterests select d;

                        Data.trnLoan newLoan = new Data.trnLoan();
                        newLoan.LoanNumber = zeroFill(Convert.ToInt32(loanNumber), 10);
                        newLoan.LoanDate = DateTime.Today;
                        newLoan.MaturityDate = DateTime.Today.AddDays(60);
                        newLoan.ApplicantId = (from d in db.mstApplicants select d.Id).FirstOrDefault();
                        newLoan.Particulars = "NA";
                        newLoan.TermId = term.FirstOrDefault().Id;
                        newLoan.TermNoOfDays = term.FirstOrDefault().NoOfDays;
                        newLoan.TermNoOfAllowanceDays = term.FirstOrDefault().NoOfAllowanceDays;
                        newLoan.LoanEndDate = DateTime.Today.AddDays(60 + Convert.ToDouble(term.FirstOrDefault().NoOfAllowanceDays));
                        newLoan.PenaltyId = (from d in db.mstPenalties select d.Id).FirstOrDefault();
                        newLoan.NoOfAbsent = 0;
                        newLoan.PreparedByUserId = userId;
                        newLoan.PrincipalAmount = 0;
                        newLoan.InterestId = interest.FirstOrDefault().Id;
                        newLoan.InterestRate = interest.FirstOrDefault().Rate;
                        newLoan.InterestAmount = 0;
                        newLoan.DeductionAmount = 0;
                        newLoan.NetAmount = 0;
                        newLoan.DailyCollectibleAmount = 0;
                        newLoan.CurrentCollectibeAmount = 0;
                        newLoan.PaidAmount = 0;
                        newLoan.NextPaidDate = DateTime.Today.AddDays(Convert.ToDouble(term.FirstOrDefault().NoOfDays));
                        newLoan.PenaltyAmount = 0;
                        newLoan.IsReconstruct = false;
                        newLoan.ReconstructInterestAmount = 0;
                        newLoan.BalanceAmount = 0;
                        newLoan.IsFullyPaid = false;
                        newLoan.IsLocked = false;
                        newLoan.CreatedByUserId = userId;
                        newLoan.CreatedDateTime = DateTime.Now;
                        newLoan.UpdatedByUserId = userId;
                        newLoan.UpdatedDateTime = DateTime.Now;
                        db.trnLoans.InsertOnSubmit(newLoan);
                        db.SubmitChanges();

                        return newLoan.Id;
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

        // lock loan
        [Authorize]
        [HttpPut]
        [Route("api/loan/lock/{id}")]
        public HttpResponseMessage lockLoan(String id, Models.TrnLoan loan)
        {
            try
            {
                var loans = from d in db.trnLoans where d.Id == Convert.ToInt32(id) select d;
                if (loans.Any())
                {
                    if (!loans.FirstOrDefault().IsLocked)
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
                                if (Convert.ToDateTime(loan.LoanDate) > Convert.ToDateTime(loan.MaturityDate))
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                                }
                                else
                                {
                                    var term = from d in db.mstTerms where d.Id == loan.TermId select d;
                                    var interest = from d in db.mstInterests where d.Id == loan.InterestId select d;
                                    var loanNoOfDays = (Convert.ToDateTime(loan.MaturityDate) - Convert.ToDateTime(loan.LoanDate)).TotalDays;
                                    
                                    var loanDeduction = from d in db.trnLoanDeductions where d.LoanId == Convert.ToInt32(id) select d;
                                    Decimal deductionAmount = 0;
                                    if (loanDeduction.Any())
                                    {
                                        deductionAmount = loanDeduction.Sum(d => d.DeductionAmount);
                                    }

                                    Decimal principalAmount = loan.NetAmount;
                                    Decimal dailyCollectibleAmount = principalAmount / Convert.ToDecimal(loanNoOfDays);


                                    var lockLoan = loans.FirstOrDefault();
                                    lockLoan.LoanDate = Convert.ToDateTime(loan.LoanDate);
                                    lockLoan.MaturityDate = Convert.ToDateTime(loan.MaturityDate);
                                    lockLoan.ApplicantId = loan.ApplicantId;
                                    lockLoan.Particulars = loan.Particulars;
                                    lockLoan.TermId = term.FirstOrDefault().Id;
                                    lockLoan.TermNoOfDays = term.FirstOrDefault().NoOfDays;
                                    lockLoan.TermNoOfAllowanceDays = term.FirstOrDefault().NoOfAllowanceDays;
                                    lockLoan.LoanEndDate = DateTime.Today.AddDays(loanNoOfDays + Convert.ToDouble(term.FirstOrDefault().NoOfAllowanceDays));
                                    lockLoan.PenaltyId = loan.PenaltyId;
                                    lockLoan.PreparedByUserId = userId;
                                    lockLoan.PrincipalAmount = loan.PrincipalAmount;
                                    lockLoan.InterestId = interest.FirstOrDefault().Id;
                                    lockLoan.InterestRate = loan.InterestRate;
                                    lockLoan.InterestAmount = loan.InterestAmount;
                                    lockLoan.DeductionAmount = deductionAmount;
                                    lockLoan.NetAmount = loan.NetAmount;
                                    lockLoan.DailyCollectibleAmount = Math.Ceiling(dailyCollectibleAmount / 5) * 5;
                                    lockLoan.CurrentCollectibeAmount = Math.Ceiling(dailyCollectibleAmount / 5) * 5;
                                    lockLoan.NextPaidDate = Convert.ToDateTime(loan.LoanDate).AddDays(Convert.ToDouble(term.FirstOrDefault().NoOfDays));
                                    lockLoan.BalanceAmount = loan.NetAmount;
                                    lockLoan.IsLocked = true;
                                    lockLoan.UpdatedByUserId = userId;
                                    lockLoan.UpdatedDateTime = DateTime.Now;
                                    db.SubmitChanges();

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

        // unlock loan
        [Authorize]
        [HttpPut]
        [Route("api/loan/unlock/{id}")]
        public HttpResponseMessage unlockLoan(String id)
        {
            try
            {
                var loans = from d in db.trnLoans where d.Id == Convert.ToInt32(id) select d;
                if (loans.Any())
                {
                    if (loans.FirstOrDefault().IsLocked)
                    {
                        var collection = from d in db.trnCollections
                                         where d.LoanId == Convert.ToInt32(id)
                                         select d;

                        if (!collection.Any())
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
                                    var unlockLoan = loans.FirstOrDefault();
                                    unlockLoan.IsLocked = false;
                                    unlockLoan.UpdatedByUserId = userId;
                                    unlockLoan.UpdatedDateTime = DateTime.Now;
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
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // delete loan
        [Authorize]
        [HttpDelete]
        [Route("api/loan/delete/{id}")]
        public HttpResponseMessage deleteLoanApplication(String id)
        {
            try
            {
                var loans = from d in db.trnLoans where d.Id == Convert.ToInt32(id) select d;
                if (loans.Any())
                {
                    if (!loans.FirstOrDefault().IsLocked)
                    {
                        var collection = from d in db.trnCollections
                                         where d.LoanId == Convert.ToInt32(id)
                                         select d;

                        if (!collection.Any())
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
                                    db.trnLoans.DeleteOnSubmit(loans.First());
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
