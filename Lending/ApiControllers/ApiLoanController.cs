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

        // loan applicants
        [Authorize]
        [HttpGet]
        [Route("api/loan/list/loanApplicants")]
        public List<Models.TrnLoan> listLoanApplicants()
        {
            var loanApplicants = from d in db.trnLoans.OrderBy(d => d.mstApplicant.ApplicantLastName)
                                 where d.IsLocked == true
                                 && d.TotalBalanceAmount > 0
                                 group d by new
                                 {
                                     ApplicantId = d.ApplicantId,
                                     Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " ")
                                 } into g
                                 select new Models.TrnLoan
                                 {
                                     ApplicantId = g.Key.ApplicantId,
                                     Applicant = g.Key.Applicant
                                 };

            return loanApplicants.ToList();
        }

        // loan list by applicantId
        [Authorize]
        [HttpGet]
        [Route("api/loan/list/byApplicantId/{applicantId}")]
        public List<Models.TrnLoan> listLoanByApplicantId(String applicantId)
        {
            var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                   where d.ApplicantId == Convert.ToInt32(applicantId)
                                   && d.IsLocked == true
                                   && d.TotalBalanceAmount > 0
                                   select new Models.TrnLoan
                                   {
                                       Id = d.Id,
                                       LoanNumberDetail = d.IsLoanApplication == true ? d.IsReconstruct == true ? "LN - " + d.LoanNumber + " (Reconstructed)" : "LN - " + d.LoanNumber : d.IsRenew == true ? "LN - " + d.LoanNumber + " (Renewed)" : d.IsLoanReconstruct == true ? d.IsReconstruct == true ? "RC - " + d.LoanNumber + " (Reconstructed)" : "RC - " + d.LoanNumber : d.IsRenew == true ? "RC - " + d.LoanNumber + " (Renewed)" : d.IsLoanRenew == true ? d.IsReconstruct == true ? "RN - " + d.LoanNumber + " (Reconstructed)" : "RN - " + d.LoanNumber : d.IsRenew == true ? "RN - " + d.LoanNumber + " (Renewed)" : d.LoanNumber,
                                       TotalBalanceAmount = d.TotalBalanceAmount,
                                       TotalPenaltyAmount = d.TotalPenaltyAmount
                                   };

            return loanApplications.ToList();
        }

        // loan applicants
        [Authorize]
        [HttpGet]
        [Route("api/loan/list/loanApplicants/InReconstruct/InRenewal")]
        public List<Models.TrnLoan> listLoanApplicantsInReconstructInRenewal()
        {
            var loanApplicants = from d in db.trnLoans.OrderBy(d => d.mstApplicant.ApplicantLastName)
                                 where d.IsLocked == true
                                 && d.TotalBalanceAmount > 0
                                 && d.IsReconstruct == false
                                 && d.IsRenew == false
                                 group d by new
                                 {
                                     ApplicantId = d.ApplicantId,
                                     Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " ")
                                 } into g
                                 select new Models.TrnLoan
                                 {
                                     ApplicantId = g.Key.ApplicantId,
                                     Applicant = g.Key.Applicant
                                 };

            return loanApplicants.ToList();
        }

        // loan list by applicantId
        [Authorize]
        [HttpGet]
        [Route("api/loan/list/byApplicantId/InReconstruct/InRenewal/{applicantId}")]
        public List<Models.TrnLoan> listLoanByApplicantIdInReconstructInRenewal(String applicantId)
        {
            var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                   where d.ApplicantId == Convert.ToInt32(applicantId)
                                   && d.IsLocked == true
                                   && d.TotalBalanceAmount > 0
                                   && d.IsReconstruct == false
                                   && d.IsRenew == false
                                   select new Models.TrnLoan
                                   {
                                       Id = d.Id,
                                       LoanNumberDetail = d.IsLoanApplication == true ? d.IsReconstruct == true ? "LN - " + d.LoanNumber + " (Reconstructed)" : "LN - " + d.LoanNumber : d.IsRenew == true ? "LN - " + d.LoanNumber + " (Renewed)" : d.IsLoanReconstruct == true ? d.IsReconstruct == true ? "RC - " + d.LoanNumber + " (Reconstructed)" : "RC - " + d.LoanNumber : d.IsRenew == true ? "RC - " + d.LoanNumber + " (Renewed)" : d.IsLoanRenew == true ? d.IsReconstruct == true ? "RN - " + d.LoanNumber + " (Reconstructed)" : "RN - " + d.LoanNumber : d.IsRenew == true ? "RN - " + d.LoanNumber + " (Renewed)" : d.LoanNumber,
                                       TotalBalanceAmount = d.TotalBalanceAmount,
                                       TotalPenaltyAmount = d.TotalPenaltyAmount
                                   };

            return loanApplications.ToList();
        }

        // loan list by applicantId
        [Authorize]
        [HttpGet]
        [Route("api/loan/list/byApplicantId/forViewCollection/{applicantId}")]
        public List<Models.TrnLoan> listLoanByApplicantIdForViewCollection(String applicantId)
        {
            var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                   where d.ApplicantId == Convert.ToInt32(applicantId)
                                   && d.IsLocked == true
                                   select new Models.TrnLoan
                                   {
                                       Id = d.Id,
                                       LoanNumberDetail = d.IsLoanApplication == true ? d.IsReconstruct == true ? "LN - " + d.LoanNumber + " (Reconstructed)" : "LN - " + d.LoanNumber : d.IsRenew == true ? "LN - " + d.LoanNumber + " (Renewed)" : d.IsLoanReconstruct == true ? d.IsReconstruct == true ? "RC - " + d.LoanNumber + " (Reconstructed)" : "RC - " + d.LoanNumber : d.IsRenew == true ? "RC - " + d.LoanNumber + " (Renewed)" : d.IsLoanRenew == true ? d.IsReconstruct == true ? "RN - " + d.LoanNumber + " (Reconstructed)" : "RN - " + d.LoanNumber : d.IsRenew == true ? "RN - " + d.LoanNumber + " (Renewed)" : d.LoanNumber,
                                       LoanDate = d.LoanDate.ToShortDateString()
                                   };

            return loanApplications.ToList();
        }

        // loan list by loan date
        [Authorize]
        [HttpGet]
        [Route("api/loan/listByLoanDate/{startLoanDate}/{endLoanDate}")]
        public List<Models.TrnLoan> listLoanByLoanDate(String startLoanDate, String endLoanDate)
        {
            var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                   where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                   && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                   && d.IsLoanApplication == true
                                   select new Models.TrnLoan
                                   {
                                       Id = d.Id,
                                       LoanNumber = d.LoanNumber,
                                       LoanDate = d.LoanDate.ToShortDateString(),
                                       ApplicantId = d.ApplicantId,
                                       Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                       Area = d.mstApplicant.mstArea.Area,
                                       Particulars = d.Particulars,
                                       PreparedByUserId = d.PreparedByUserId,
                                       PreparedByUser = d.mstUser.FullName,
                                       TermId = d.TermId,
                                       Term = d.mstTerm.Term,
                                       TermNoOfDays = d.TermNoOfDays,
                                       TermPaymentNoOfDays = d.TermPaymentNoOfDays,
                                       MaturityDate = d.MaturityDate.ToShortDateString(),
                                       PrincipalAmount = d.PrincipalAmount,
                                       InterestId = d.InterestId,
                                       Interest = d.mstInterest.Interest,
                                       InterestRate = d.InterestRate,
                                       InterestAmount = d.InterestAmount,
                                       PreviousBalanceAmount = d.PreviousBalanceAmount,
                                       PreviousPenaltyAmount = d.PreviousPenaltyAmount,
                                       DeductionAmount = d.DeductionAmount,
                                       NetAmount = d.NetAmount,
                                       NetCollectionAmount = d.NetCollectionAmount,
                                       TotalPaidAmount = d.TotalPaidAmount,
                                       TotalPenaltyAmount = d.TotalPenaltyAmount,
                                       TotalBalanceAmount = d.TotalBalanceAmount,
                                       IsReconstruct = d.IsReconstruct,
                                       IsRenew = d.IsRenew,
                                       IsLoanApplication = d.IsLoanApplication,
                                       IsLoanReconstruct = d.IsLoanReconstruct,
                                       IsLoanRenew = d.IsLoanRenew,
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
                       && d.IsLoanApplication == true
                       select new Models.TrnLoan
                       {
                           Id = d.Id,
                           LoanNumber = d.LoanNumber,
                           LoanDate = d.LoanDate.ToShortDateString(),
                           ApplicantId = d.ApplicantId,
                           Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                           Area = d.mstApplicant.mstArea.Area,
                           Particulars = d.Particulars,
                           PreparedByUserId = d.PreparedByUserId,
                           PreparedByUser = d.mstUser.FullName,
                           TermId = d.TermId,
                           Term = d.mstTerm.Term,
                           TermNoOfDays = d.TermNoOfDays,
                           TermPaymentNoOfDays = d.TermPaymentNoOfDays,
                           MaturityDate = d.MaturityDate.ToShortDateString(),
                           PrincipalAmount = d.PrincipalAmount,
                           InterestId = d.InterestId,
                           Interest = d.mstInterest.Interest,
                           InterestRate = d.InterestRate,
                           InterestAmount = d.InterestAmount,
                           PreviousBalanceAmount = d.PreviousBalanceAmount,
                           PreviousPenaltyAmount = d.PreviousPenaltyAmount,
                           DeductionAmount = d.DeductionAmount,
                           NetAmount = d.NetAmount,
                           NetCollectionAmount = d.NetCollectionAmount,
                           TotalPaidAmount = d.TotalPaidAmount,
                           TotalPenaltyAmount = d.TotalPenaltyAmount,
                           TotalBalanceAmount = d.TotalBalanceAmount,
                           IsReconstruct = d.IsReconstruct,
                           IsRenew = d.IsRenew,
                           IsLoanApplication = d.IsLoanApplication,
                           IsLoanReconstruct = d.IsLoanReconstruct,
                           IsLoanRenew = d.IsLoanRenew,
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
                        var loan = from d in db.trnLoans.OrderByDescending(d => d.Id) where d.IsLoanApplication == true select d;
                        if (loan.Any())
                        {
                            var newLoanNumber = Convert.ToInt32(loan.FirstOrDefault().LoanNumber) + 0000000001;
                            loanNumber = newLoanNumber.ToString();
                        }

                        var applicant = from d in db.mstApplicants select d;
                        if (applicant.Any())
                        {
                            var term = from d in db.mstTerms select d;
                            if (term.Any())
                            {
                                var interest = from d in db.mstInterests select d;
                                if (interest.Any())
                                {
                                    Data.trnLoan newLoan = new Data.trnLoan();
                                    newLoan.LoanNumber = zeroFill(Convert.ToInt32(loanNumber), 10);
                                    newLoan.LoanDate = DateTime.Today;
                                    newLoan.ApplicantId = applicant.FirstOrDefault().Id;
                                    newLoan.Particulars = "NA";
                                    newLoan.PreparedByUserId = userId;
                                    newLoan.TermId = term.FirstOrDefault().Id;
                                    newLoan.TermNoOfDays = term.FirstOrDefault().NoOfDays;
                                    newLoan.TermPaymentNoOfDays = term.FirstOrDefault().PaymentNoOfDays;
                                    newLoan.MaturityDate = DateTime.Today;
                                    newLoan.PrincipalAmount = 0;
                                    newLoan.InterestId = interest.FirstOrDefault().Id;
                                    newLoan.InterestRate = interest.FirstOrDefault().Rate;
                                    newLoan.InterestAmount = 0;
                                    newLoan.PreviousBalanceAmount = 0;
                                    newLoan.PreviousPenaltyAmount = 0;
                                    newLoan.DeductionAmount = 0;
                                    newLoan.NetAmount = 0;
                                    newLoan.NetCollectionAmount = 0;
                                    newLoan.TotalPaidAmount = 0;
                                    newLoan.TotalPenaltyAmount = 0;
                                    newLoan.TotalBalanceAmount = 0;
                                    newLoan.IsReconstruct = false;
                                    newLoan.IsRenew = false;
                                    newLoan.IsLoanApplication = true;
                                    newLoan.IsLoanReconstruct = false;
                                    newLoan.IsLoanRenew = false;
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
                                if (loan.NetCollectionAmount != 0)
                                {
                                    var loanDeduction = from d in db.trnLoanDeductions
                                                        where d.LoanId == Convert.ToInt32(id)
                                                        select d;

                                    Decimal deductionAmount = 0;
                                    if (loanDeduction.Any())
                                    {
                                        deductionAmount = loanDeduction.Sum(d => d.DeductionAmount);
                                    }

                                    var lockLoan = loans.FirstOrDefault();
                                    lockLoan.LoanDate = Convert.ToDateTime(loan.LoanDate);
                                    lockLoan.ApplicantId = loan.ApplicantId;
                                    lockLoan.Particulars = loan.Particulars;
                                    lockLoan.PreparedByUserId = loan.PreparedByUserId;
                                    lockLoan.TermId = loan.TermId;
                                    lockLoan.TermNoOfDays = loan.TermNoOfDays;
                                    lockLoan.TermPaymentNoOfDays = loan.TermPaymentNoOfDays;
                                    lockLoan.MaturityDate = DateTime.Today;
                                    lockLoan.PrincipalAmount = loan.PrincipalAmount;
                                    lockLoan.InterestId = loan.InterestId;
                                    lockLoan.InterestRate = loan.InterestRate;
                                    lockLoan.InterestAmount = loan.InterestAmount;
                                    lockLoan.PreviousBalanceAmount = loan.PreviousBalanceAmount;
                                    lockLoan.PreviousPenaltyAmount = loan.PreviousPenaltyAmount;
                                    lockLoan.DeductionAmount = deductionAmount;
                                    lockLoan.NetAmount = loan.NetAmount;
                                    lockLoan.NetCollectionAmount = loan.NetCollectionAmount;
                                    lockLoan.TotalBalanceAmount = loan.NetCollectionAmount;
                                    lockLoan.IsFullyPaid = false;
                                    lockLoan.IsLocked = true;
                                    lockLoan.UpdatedByUserId = userId;
                                    lockLoan.UpdatedDateTime = DateTime.Now;
                                    db.SubmitChanges();

                                    if (loans.FirstOrDefault().IsLoanReconstruct)
                                    {
                                        var loanReconstructs = from d in db.trnLoanReconstructs
                                                               where d.LoanId == Convert.ToInt32(id)
                                                               select new Models.TrnLoanReconstruct
                                                               {
                                                                   Id = d.Id,
                                                                   LoanId = d.LoanId,
                                                                   ReconstructLoanId = d.ReconstructLoanId
                                                               };

                                        if (loanReconstructs.Any())
                                        {
                                            foreach (var loanReconstruct in loanReconstructs)
                                            {
                                                var loanReconUpdate = from d in db.trnLoans
                                                                      where d.Id == loanReconstruct.ReconstructLoanId
                                                                      select d;

                                                if (loanReconUpdate.Any())
                                                {
                                                    var updateLoanRecon = loanReconUpdate.FirstOrDefault();
                                                    updateLoanRecon.IsReconstruct = true;
                                                    db.SubmitChanges();
                                                }
                                            }
                                        }
                                    }

                                    if (loans.FirstOrDefault().IsLoanRenew)
                                    {
                                        var loanRenews = from d in db.trnLoanRenews
                                                         where d.LoanId == Convert.ToInt32(id)
                                                         select new Models.TrnLoanRenew
                                                         {
                                                             Id = d.Id,
                                                             LoanId = d.LoanId,
                                                             RenewLoanId = d.RenewLoanId
                                                         };

                                        if (loanRenews.Any())
                                        {
                                            foreach (var loanRenew in loanRenews)
                                            {
                                                var loanRenewUpdate = from d in db.trnLoans
                                                                      where d.Id == loanRenew.RenewLoanId
                                                                      select d;

                                                if (loanRenewUpdate.Any())
                                                {
                                                    var updateLoanRenew = loanRenewUpdate.FirstOrDefault();
                                                    updateLoanRenew.IsRenew = true;
                                                    db.SubmitChanges();
                                                }
                                            }
                                        }
                                    }

                                    Decimal collectibleAmount = loan.NetCollectionAmount / loan.TermNoOfDays;
                                    Decimal ceilCollectibleAmount = Math.Ceiling(collectibleAmount / 5) * 5;
                                    Decimal loanNetCollectionAmount = loan.NetCollectionAmount;

                                    var dayCount = 0;
                                    for (var i = 1; i <= loan.TermNoOfDays; i++)
                                    {
                                        if (i % loan.TermPaymentNoOfDays == 0)
                                        {
                                            Decimal finalCollectibleAmount = ceilCollectibleAmount * loan.TermPaymentNoOfDays;

                                            dayCount += 1;

                                            if (loanNetCollectionAmount < finalCollectibleAmount)
                                            {
                                                finalCollectibleAmount = loanNetCollectionAmount;
                                            }

                                            if (finalCollectibleAmount != 0)
                                            {
                                                Data.trnLoanLine newLoanLine = new Data.trnLoanLine();
                                                newLoanLine.LoanId = Convert.ToInt32(id);

                                                if (loans.FirstOrDefault().IsLoanApplication)
                                                {
                                                    newLoanLine.DayReference = "LN-" + loans.FirstOrDefault().LoanNumber + "-" + this.zeroFill(dayCount, 3) + " (" + Convert.ToDateTime(loan.LoanDate).AddDays(i).ToString("MMM dd, yyyy") + ") - " + Convert.ToDateTime(loan.LoanDate).AddDays(i).DayOfWeek.ToString();
                                                }
                                                else
                                                {
                                                    if (loans.FirstOrDefault().IsLoanReconstruct)
                                                    {
                                                        newLoanLine.DayReference = "RC-" + loans.FirstOrDefault().LoanNumber + "-" + this.zeroFill(dayCount, 3) + " (" + Convert.ToDateTime(loan.LoanDate).AddDays(i).ToString("MMM dd, yyyy") + ") - " + Convert.ToDateTime(loan.LoanDate).AddDays(i).DayOfWeek.ToString();
                                                    }
                                                    else
                                                    {
                                                        if (loans.FirstOrDefault().IsLoanRenew)
                                                        {
                                                            newLoanLine.DayReference = "RN-" + loans.FirstOrDefault().LoanNumber + "-" + this.zeroFill(dayCount, 3) + " (" + Convert.ToDateTime(loan.LoanDate).AddDays(i).ToString("MMM dd, yyyy") + ") - " + Convert.ToDateTime(loan.LoanDate).AddDays(i).DayOfWeek.ToString();
                                                        }
                                                    }
                                                }

                                                newLoanLine.CollectibleDate = Convert.ToDateTime(loan.LoanDate).AddDays(i);
                                                newLoanLine.CollectibleAmount = finalCollectibleAmount;
                                                newLoanLine.PaidAmount = 0;
                                                newLoanLine.PenaltyAmount = 0;
                                                db.trnLoanLines.InsertOnSubmit(newLoanLine);
                                                db.SubmitChanges();

                                                loanNetCollectionAmount -= finalCollectibleAmount;
                                            }
                                        }

                                        if (i == loan.TermNoOfDays)
                                        {
                                            var loanLines = from d in db.trnLoanLines.OrderByDescending(d => d.Id)
                                                            where d.LoanId == Convert.ToInt32(id)
                                                            select d;

                                            if (loanLines.Any())
                                            {
                                                lockLoan.MaturityDate = loanLines.FirstOrDefault().CollectibleDate;
                                                db.SubmitChanges();
                                            }
                                        }
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
                                         && d.IsLocked == true
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
                                    var existLoanReconstruct = from d in db.trnLoanReconstructs
                                                               where d.ReconstructLoanId == Convert.ToInt32(id)
                                                               select d;

                                    if (!existLoanReconstruct.Any())
                                    {
                                        var existLoanRenew = from d in db.trnLoanRenews
                                                             where d.RenewLoanId == Convert.ToInt32(id)
                                                             select d;

                                        if (!existLoanRenew.Any())
                                        {
                                            if (loans.FirstOrDefault().IsLoanReconstruct)
                                            {
                                                var loanReconstructs = from d in db.trnLoanReconstructs
                                                                       where d.LoanId == Convert.ToInt32(id)
                                                                       select new Models.TrnLoanReconstruct
                                                                       {
                                                                           Id = d.Id,
                                                                           LoanId = d.LoanId,
                                                                           ReconstructLoanId = d.ReconstructLoanId
                                                                       };

                                                if (loanReconstructs.Any())
                                                {
                                                    foreach (var loanReconstruct in loanReconstructs)
                                                    {
                                                        var loanReconUpdate = from d in db.trnLoans
                                                                              where d.Id == loanReconstruct.ReconstructLoanId
                                                                              select d;

                                                        if (loanReconUpdate.Any())
                                                        {
                                                            var updateLoanRecon = loanReconUpdate.FirstOrDefault();
                                                            updateLoanRecon.IsReconstruct = false;
                                                            db.SubmitChanges();
                                                        }
                                                    }
                                                }
                                            }

                                            if (loans.FirstOrDefault().IsLoanRenew)
                                            {
                                                var loanRenews = from d in db.trnLoanRenews
                                                                 where d.LoanId == Convert.ToInt32(id)
                                                                 select new Models.TrnLoanRenew
                                                                 {
                                                                     Id = d.Id,
                                                                     LoanId = d.LoanId,
                                                                     RenewLoanId = d.RenewLoanId
                                                                 };

                                                if (loanRenews.Any())
                                                {
                                                    foreach (var loanRenew in loanRenews)
                                                    {
                                                        var loanRenewUpdate = from d in db.trnLoans
                                                                              where d.Id == loanRenew.RenewLoanId
                                                                              select d;

                                                        if (loanRenewUpdate.Any())
                                                        {
                                                            var updateLoanRenew = loanRenewUpdate.FirstOrDefault();
                                                            updateLoanRenew.IsRenew = false;
                                                            db.SubmitChanges();
                                                        }
                                                    }
                                                }
                                            }

                                            var loanLines = from d in db.trnLoanLines
                                                            where d.LoanId == Convert.ToInt32(id)
                                                            select d;

                                            if (loanLines.Any())
                                            {
                                                db.trnLoanLines.DeleteAllOnSubmit(loanLines);
                                                db.SubmitChanges();
                                            }

                                            var unlockLoan = loans.FirstOrDefault();
                                            unlockLoan.IsLocked = false;
                                            unlockLoan.UpdatedByUserId = userId;
                                            unlockLoan.UpdatedDateTime = DateTime.Now;
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
                                         && d.IsLocked == true
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
                                    // loan reconstructs 
                                    var existLoanReconstruct = from d in db.trnLoanReconstructs
                                                               where d.ReconstructLoanId == Convert.ToInt32(id)
                                                               select d;

                                    if (!existLoanReconstruct.Any())
                                    {
                                        var loanReconstruct = from d in db.trnLoanReconstructs
                                                              where d.LoanId == Convert.ToInt32(id)
                                                              select d;

                                        if (loanReconstruct.Any())
                                        {
                                            var existLoan = from d in db.trnLoans
                                                            where d.Id == loanReconstruct.FirstOrDefault().ReconstructLoanId
                                                            select d;

                                            if (existLoan.Any())
                                            {
                                                var updateLoan = existLoan.FirstOrDefault();
                                                updateLoan.IsReconstruct = false;
                                                db.SubmitChanges();

                                                db.trnLoanReconstructs.DeleteAllOnSubmit(loanReconstruct);
                                                db.SubmitChanges();
                                            }
                                        }
                                    }

                                    // loan renews
                                    var existLoanRenew = from d in db.trnLoanRenews
                                                         where d.RenewLoanId == Convert.ToInt32(id)
                                                         select d;

                                    if (!existLoanRenew.Any())
                                    {
                                        var loanRenew = from d in db.trnLoanRenews
                                                        where d.LoanId == Convert.ToInt32(id)
                                                        select d;

                                        if (loanRenew.Any())
                                        {
                                            var existLoan = from d in db.trnLoans
                                                            where d.Id == loanRenew.FirstOrDefault().RenewLoanId
                                                            select d;

                                            if (existLoan.Any())
                                            {
                                                var updateLoan = existLoan.FirstOrDefault();
                                                updateLoan.IsRenew = false;
                                                db.SubmitChanges();

                                                db.trnLoanRenews.DeleteAllOnSubmit(loanRenew);
                                                db.SubmitChanges();
                                            }
                                        }
                                    }

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

        // loan list by applicant and transaction type
        [Authorize]
        [HttpGet]
        [Route("api/loan/listByApplicantAndByTtransactionType/{applicantId}/{transactionType}")]
        public List<Models.TrnLoan> listLoanByApplicantAndByTtransactionType(String applicantId, String transactionType)
        {
            if (transactionType.Equals("Loans"))
            {
                var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                       where d.IsLoanApplication == true
                                       && d.ApplicantId == Convert.ToInt32(applicantId)
                                       && d.IsLocked == true
                                       select new Models.TrnLoan
                                       {
                                           Id = d.Id,
                                           LoanNumber = d.LoanNumber,
                                           LoanDate = d.LoanDate.ToShortDateString(),
                                           ApplicantId = d.ApplicantId,
                                           Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                           Area = d.mstApplicant.mstArea.Area,
                                           Particulars = d.Particulars,
                                           PreparedByUserId = d.PreparedByUserId,
                                           PreparedByUser = d.mstUser.FullName,
                                           TermId = d.TermId,
                                           Term = d.mstTerm.Term,
                                           TermNoOfDays = d.TermNoOfDays,
                                           TermPaymentNoOfDays = d.TermPaymentNoOfDays,
                                           MaturityDate = d.MaturityDate.ToShortDateString(),
                                           PrincipalAmount = d.PrincipalAmount,
                                           InterestId = d.InterestId,
                                           Interest = d.mstInterest.Interest,
                                           InterestRate = d.InterestRate,
                                           InterestAmount = d.InterestAmount,
                                           PreviousBalanceAmount = d.PreviousBalanceAmount,
                                           PreviousPenaltyAmount = d.PreviousPenaltyAmount,
                                           DeductionAmount = d.DeductionAmount,
                                           NetAmount = d.NetAmount,
                                           NetCollectionAmount = d.NetCollectionAmount,
                                           TotalPaidAmount = d.TotalPaidAmount,
                                           TotalPenaltyAmount = d.TotalPenaltyAmount,
                                           TotalBalanceAmount = d.TotalBalanceAmount,
                                           IsReconstruct = d.IsReconstruct,
                                           IsRenew = d.IsRenew,
                                           IsLoanApplication = d.IsLoanApplication,
                                           IsLoanReconstruct = d.IsLoanReconstruct,
                                           IsLoanRenew = d.IsLoanRenew,
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
            else
            {
                if (transactionType.Equals("ReconstructedLoans"))
                {
                    var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                           where d.IsLoanReconstruct == true
                                           && d.ApplicantId == Convert.ToInt32(applicantId)
                                           && d.IsLocked == true
                                           select new Models.TrnLoan
                                           {
                                               Id = d.Id,
                                               LoanNumber = d.LoanNumber,
                                               LoanDate = d.LoanDate.ToShortDateString(),
                                               ApplicantId = d.ApplicantId,
                                               Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                               Area = d.mstApplicant.mstArea.Area,
                                               Particulars = d.Particulars,
                                               PreparedByUserId = d.PreparedByUserId,
                                               PreparedByUser = d.mstUser.FullName,
                                               TermId = d.TermId,
                                               Term = d.mstTerm.Term,
                                               TermNoOfDays = d.TermNoOfDays,
                                               TermPaymentNoOfDays = d.TermPaymentNoOfDays,
                                               MaturityDate = d.MaturityDate.ToShortDateString(),
                                               PrincipalAmount = d.PrincipalAmount,
                                               InterestId = d.InterestId,
                                               Interest = d.mstInterest.Interest,
                                               InterestRate = d.InterestRate,
                                               InterestAmount = d.InterestAmount,
                                               PreviousBalanceAmount = d.PreviousBalanceAmount,
                                               PreviousPenaltyAmount = d.PreviousPenaltyAmount,
                                               DeductionAmount = d.DeductionAmount,
                                               NetAmount = d.NetAmount,
                                               NetCollectionAmount = d.NetCollectionAmount,
                                               TotalPaidAmount = d.TotalPaidAmount,
                                               TotalPenaltyAmount = d.TotalPenaltyAmount,
                                               TotalBalanceAmount = d.TotalBalanceAmount,
                                               IsReconstruct = d.IsReconstruct,
                                               IsRenew = d.IsRenew,
                                               IsLoanApplication = d.IsLoanApplication,
                                               IsLoanReconstruct = d.IsLoanReconstruct,
                                               IsLoanRenew = d.IsLoanRenew,
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
                else
                {
                    if (transactionType.Equals("Renews"))
                    {
                        var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                               where d.IsLoanRenew == true
                                               && d.ApplicantId == Convert.ToInt32(applicantId)
                                               && d.IsLocked == true
                                               select new Models.TrnLoan
                                               {
                                                   Id = d.Id,
                                                   LoanNumber = d.LoanNumber,
                                                   LoanDate = d.LoanDate.ToShortDateString(),
                                                   ApplicantId = d.ApplicantId,
                                                   Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                                   Area = d.mstApplicant.mstArea.Area,
                                                   Particulars = d.Particulars,
                                                   PreparedByUserId = d.PreparedByUserId,
                                                   PreparedByUser = d.mstUser.FullName,
                                                   TermId = d.TermId,
                                                   Term = d.mstTerm.Term,
                                                   TermNoOfDays = d.TermNoOfDays,
                                                   TermPaymentNoOfDays = d.TermPaymentNoOfDays,
                                                   MaturityDate = d.MaturityDate.ToShortDateString(),
                                                   PrincipalAmount = d.PrincipalAmount,
                                                   InterestId = d.InterestId,
                                                   Interest = d.mstInterest.Interest,
                                                   InterestRate = d.InterestRate,
                                                   InterestAmount = d.InterestAmount,
                                                   PreviousBalanceAmount = d.PreviousBalanceAmount,
                                                   PreviousPenaltyAmount = d.PreviousPenaltyAmount,
                                                   DeductionAmount = d.DeductionAmount,
                                                   NetAmount = d.NetAmount,
                                                   NetCollectionAmount = d.NetCollectionAmount,
                                                   TotalPaidAmount = d.TotalPaidAmount,
                                                   TotalPenaltyAmount = d.TotalPenaltyAmount,
                                                   TotalBalanceAmount = d.TotalBalanceAmount,
                                                   IsReconstruct = d.IsReconstruct,
                                                   IsRenew = d.IsRenew,
                                                   IsLoanApplication = d.IsLoanApplication,
                                                   IsLoanReconstruct = d.IsLoanReconstruct,
                                                   IsLoanRenew = d.IsLoanRenew,
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
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
