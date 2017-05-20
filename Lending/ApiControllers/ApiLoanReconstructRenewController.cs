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
    public class ApiLoanReconstructRenewController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan recontructs and renews list
        [Authorize]
        [HttpGet]
        [Route("api/loanReconstructRenews/list/ByLoanId/{loanId}")]
        public List<Models.TrnLoanReconstructRenew> listReconstructRenews(String loanId)
        {
            var reconstructs = from d in db.trnLoanReconstructRenews.OrderByDescending(d => d.Id)
                               where d.LoanId == Convert.ToInt32(loanId)
                               select new Models.TrnLoanReconstructRenew
                               {
                                   Id = d.Id,
                                   LoanId = d.LoanId,
                                   ReconstructRenewLoanId = d.ReconstructRenewLoanId,
                                   ReconstructRenewLoanNumber = d.trnLoan1.IsLoanApplication == true ? "LN-" + d.trnLoan1.LoanNumber : d.trnLoan1.IsLoanReconstruct == true ? "RC-" + d.trnLoan1.LoanNumber : d.trnLoan1.IsLoanRenew == true ? "RN-" + d.trnLoan1.LoanNumber : " ",
                                   BalanceAmount = d.trnLoan1.TotalBalanceAmount,
                               };

            return reconstructs.ToList();
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

        // add loan reconstruct
        [Authorize]
        [HttpPost]
        [Route("api/loanReconstruct/add")]
        public Int32 addLoanReconstruct(Models.TrnLoanReconstructRenew loanReconstructRenew)
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
                    String matchPageString = "ReconstructList";
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
                        if (loanReconstructRenew.BalanceAmount > 0)
                        {
                            String loanNumber = "0000000001";
                            var loan = from d in db.trnLoans.OrderByDescending(d => d.Id) where d.IsLoanReconstruct == true select d;
                            if (loan.Any())
                            {
                                var newLoanNumber = Convert.ToInt32(loan.FirstOrDefault().LoanNumber) + 0000000001;
                                loanNumber = newLoanNumber.ToString();
                            }

                            var term = from d in db.mstTerms.OrderByDescending(d => d.Id) select d;
                            if (term.Any())
                            {
                                var interest = from d in db.mstInterests.OrderByDescending(d => d.Id) select d;
                                if (interest.Any())
                                {
                                    var existLoan = from d in db.trnLoans
                                                    where d.Id == loanReconstructRenew.ReconstructRenewLoanId
                                                    where d.IsLocked == true
                                                    select d;

                                    if (existLoan.Any())
                                    {
                                        Data.trnLoan newLoan = new Data.trnLoan();
                                        newLoan.LoanNumber = zeroFill(Convert.ToInt32(loanNumber), 10);
                                        newLoan.LoanDate = DateTime.Today;
                                        newLoan.ApplicantId = existLoan.FirstOrDefault().ApplicantId;
                                        newLoan.Particulars = "NA";
                                        newLoan.PreparedByUserId = userId;
                                        newLoan.TermId = term.FirstOrDefault().Id;
                                        newLoan.TermNoOfDays = term.FirstOrDefault().NoOfDays;
                                        newLoan.MaturityDate = DateTime.Today;
                                        newLoan.PrincipalAmount = existLoan.FirstOrDefault().TotalBalanceAmount;
                                        newLoan.InterestId = interest.FirstOrDefault().Id;
                                        newLoan.InterestRate = interest.FirstOrDefault().Rate;
                                        Decimal interestAmount = (loanReconstructRenew.BalanceAmount / 100) * interest.FirstOrDefault().Rate;
                                        newLoan.InterestAmount = interestAmount;
                                        newLoan.PreviousBalanceAmount = 0;
                                        newLoan.DeductionAmount = 0;
                                        newLoan.NetAmount = 0;
                                        newLoan.NetCollectionAmount = loanReconstructRenew.BalanceAmount + interestAmount;
                                        newLoan.CollectibleAmount = 0;
                                        newLoan.TotalPaidAmount = 0;
                                        newLoan.TotalPenaltyAmount = 0;
                                        newLoan.TotalBalanceAmount = 0;
                                        newLoan.IsReconstructed = false;
                                        newLoan.IsRenewed = false;
                                        newLoan.IsLoanApplication = false;
                                        newLoan.IsLoanReconstruct = true;
                                        newLoan.IsLoanRenew = false;
                                        newLoan.IsLocked = false;
                                        newLoan.CreatedByUserId = userId;
                                        newLoan.CreatedDateTime = DateTime.Now;
                                        newLoan.UpdatedByUserId = userId;
                                        newLoan.UpdatedDateTime = DateTime.Now;
                                        db.trnLoans.InsertOnSubmit(newLoan);
                                        db.SubmitChanges();

                                        Data.trnLoanReconstructRenew newLoanReconstruct = new Data.trnLoanReconstructRenew();
                                        newLoanReconstruct.LoanId = newLoan.Id;
                                        newLoanReconstruct.ReconstructRenewLoanId = loanReconstructRenew.ReconstructRenewLoanId;
                                        db.trnLoanReconstructRenews.InsertOnSubmit(newLoanReconstruct);
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

        // add loan reconstruct overdue
        [Authorize]
        [HttpPost]
        [Route("api/loanReconstruct/overdue/add")]
        public Int32 addLoanReconstructOverdue(Models.TrnLoanReconstructRenew loanReconstruct)
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
                    String matchPageString = "ReconstructList";
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
                        var loan = from d in db.trnLoans.OrderByDescending(d => d.Id) where d.IsLoanReconstruct == true select d;
                        if (loan.Any())
                        {
                            var newLoanNumber = Convert.ToInt32(loan.FirstOrDefault().LoanNumber) + 0000000001;
                            loanNumber = newLoanNumber.ToString();
                        }

                        var term = from d in db.mstTerms.OrderByDescending(d => d.Id) select d;
                        if (term.Any())
                        {
                            var interest = from d in db.mstInterests.OrderByDescending(d => d.Id) select d;
                            if (interest.Any())
                            {
                                if (loanReconstruct.BalanceAmount != 0)
                                {
                                    Data.trnLoan newLoan = new Data.trnLoan();
                                    newLoan.LoanNumber = zeroFill(Convert.ToInt32(loanNumber), 10);
                                    newLoan.LoanDate = Convert.ToDateTime(loanReconstruct.LoanDate);
                                    newLoan.ApplicantId = loanReconstruct.ApplicantId;
                                    newLoan.Particulars = loanReconstruct.Particulars;
                                    newLoan.PreparedByUserId = userId;
                                    newLoan.TermId = term.FirstOrDefault().Id;
                                    newLoan.TermNoOfDays = term.FirstOrDefault().NoOfDays;
                                    newLoan.MaturityDate = Convert.ToDateTime(loanReconstruct.LoanDate);
                                    newLoan.PrincipalAmount = loanReconstruct.BalanceAmount;
                                    newLoan.InterestId = interest.FirstOrDefault().Id;
                                    newLoan.InterestRate = interest.FirstOrDefault().Rate;
                                    Decimal interestAmount = (loanReconstruct.BalanceAmount / 100) * interest.FirstOrDefault().Rate;
                                    newLoan.InterestAmount = interestAmount;
                                    newLoan.PreviousBalanceAmount = 0;
                                    newLoan.DeductionAmount = 0;
                                    newLoan.NetAmount = 0;
                                    newLoan.NetCollectionAmount = loanReconstruct.BalanceAmount + interestAmount;
                                    newLoan.CollectibleAmount = 0;
                                    newLoan.TotalPaidAmount = 0;
                                    newLoan.TotalPenaltyAmount = 0;
                                    newLoan.TotalBalanceAmount = 0;
                                    newLoan.IsReconstructed = false;
                                    newLoan.IsRenewed = false;
                                    newLoan.IsLoanApplication = false;
                                    newLoan.IsLoanReconstruct = true;
                                    newLoan.IsLoanRenew = false;
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

        // add loan renew
        [Authorize]
        [HttpPost]
        [Route("api/loanRenew/add")]
        public Int32 addLoanRenew(Models.TrnLoanReconstructRenew loanRenew)
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
                    String matchPageString = "RenewList";
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
                        var loan = from d in db.trnLoans.OrderByDescending(d => d.Id) where d.IsLoanRenew == true select d;
                        if (loan.Any())
                        {
                            var newLoanNumber = Convert.ToInt32(loan.FirstOrDefault().LoanNumber) + 0000000001;
                            loanNumber = newLoanNumber.ToString();
                        }

                        var term = from d in db.mstTerms.OrderByDescending(d => d.Id) select d;
                        if (term.Any())
                        {
                            var interest = from d in db.mstInterests.OrderByDescending(d => d.Id) select d;
                            if (interest.Any())
                            {
                                var existLoan = from d in db.trnLoans
                                                where d.Id == loanRenew.ReconstructRenewLoanId
                                                where d.IsLocked == true
                                                select d;

                                if (existLoan.Any())
                                {
                                    if (loanRenew.RenewPrincipalAmount != 0)
                                    {
                                        if (loanRenew.RenewPrincipalAmount >= loanRenew.BalanceAmount)
                                        {
                                            Data.trnLoan newLoan = new Data.trnLoan();
                                            newLoan.LoanNumber = zeroFill(Convert.ToInt32(loanNumber), 10);
                                            newLoan.LoanDate = DateTime.Today;
                                            newLoan.ApplicantId = loanRenew.ApplicantId;
                                            newLoan.Particulars = "NA";
                                            newLoan.PreparedByUserId = userId;
                                            newLoan.TermId = term.FirstOrDefault().Id;
                                            newLoan.TermNoOfDays = term.FirstOrDefault().NoOfDays;
                                            newLoan.MaturityDate = DateTime.Today;
                                            newLoan.PrincipalAmount = loanRenew.RenewPrincipalAmount;
                                            newLoan.InterestId = interest.FirstOrDefault().Id;
                                            newLoan.InterestRate = interest.FirstOrDefault().Rate;
                                            Decimal interestAmount = (loanRenew.RenewPrincipalAmount / 100) * interest.FirstOrDefault().Rate;
                                            newLoan.InterestAmount = interestAmount;
                                            newLoan.PreviousBalanceAmount = loanRenew.BalanceAmount;
                                            newLoan.DeductionAmount = 0;
                                            newLoan.NetAmount = loanRenew.RenewPrincipalAmount - loanRenew.BalanceAmount;
                                            newLoan.NetCollectionAmount = loanRenew.RenewPrincipalAmount + interestAmount;
                                            newLoan.CollectibleAmount = 0;
                                            newLoan.TotalPaidAmount = 0;
                                            newLoan.TotalPenaltyAmount = 0;
                                            newLoan.TotalBalanceAmount = 0;
                                            newLoan.IsReconstructed = false;
                                            newLoan.IsRenewed = false;
                                            newLoan.IsLoanApplication = false;
                                            newLoan.IsLoanReconstruct = false;
                                            newLoan.IsLoanRenew = true;
                                            newLoan.IsLocked = false;
                                            newLoan.CreatedByUserId = userId;
                                            newLoan.CreatedDateTime = DateTime.Now;
                                            newLoan.UpdatedByUserId = userId;
                                            newLoan.UpdatedDateTime = DateTime.Now;
                                            db.trnLoans.InsertOnSubmit(newLoan);
                                            db.SubmitChanges();

                                            Data.trnLoanReconstructRenew newLoanRenew = new Data.trnLoanReconstructRenew();
                                            newLoanRenew.LoanId = newLoan.Id;
                                            newLoanRenew.ReconstructRenewLoanId = loanRenew.ReconstructRenewLoanId;
                                            db.trnLoanReconstructRenews.InsertOnSubmit(newLoanRenew);
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

        // renew list by loan date
        [Authorize]
        [HttpGet]
        [Route("api/renew/listByLoanDate/{startLoanDate}/{endLoanDate}")]
        public List<Models.TrnLoan> listRenewByLoanDate(String startLoanDate, String endLoanDate)
        {
            var renews = from d in db.trnLoans.OrderByDescending(d => d.Id)
                         join s in db.trnLoanReconstructRenews
                         on d.Id equals s.LoanId
                         into joinRenews
                         from listRenews in joinRenews.DefaultIfEmpty()
                         where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                         && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                         && d.IsLoanRenew == true
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
                             MaturityDate = d.MaturityDate.ToShortDateString(),
                             PrincipalAmount = d.PrincipalAmount,
                             InterestId = d.InterestId,
                             Interest = d.mstInterest.Interest,
                             InterestRate = d.InterestRate,
                             InterestAmount = d.InterestAmount,
                             PreviousBalanceAmount = d.PreviousBalanceAmount,
                             DeductionAmount = d.DeductionAmount,
                             NetAmount = d.NetAmount,
                             NetCollectionAmount = d.NetCollectionAmount,
                             CollectibleAmount = d.CollectibleAmount,
                             TotalPaidAmount = d.TotalPaidAmount,
                             TotalPenaltyAmount = d.TotalPenaltyAmount,
                             TotalBalanceAmount = d.TotalBalanceAmount,
                             IsLoanApplication = d.IsLoanApplication,
                             IsLoanReconstruct = d.IsLoanReconstruct,
                             IsLoanRenew = d.IsLoanRenew,
                             IsReconstructed = d.IsReconstructed,
                             IsRenewed = d.IsRenewed,
                             IsLocked = d.IsLocked,
                             CreatedByUserId = d.CreatedByUserId,
                             CreatedByUser = d.mstUser1.FullName,
                             CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                             UpdatedByUserId = d.UpdatedByUserId,
                             UpdatedByUser = d.mstUser2.FullName,
                             UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                             RenewedDocNumber = joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanApplication == true ? "LN-" + joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanReconstruct == true ? "RC-" + joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanRenew == true ? "RN-" + joinRenews.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : " "
                         };

            return renews.ToList();
        }

        // renew get by id
        [Authorize]
        [HttpGet]
        [Route("api/renew/getById/{id}")]
        public Models.TrnLoan getRenewById(String id)
        {
            var renew = from d in db.trnLoans
                        where d.Id == Convert.ToInt32(id)
                        && d.IsLoanRenew == true
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
                            MaturityDate = d.MaturityDate.ToShortDateString(),
                            PrincipalAmount = d.PrincipalAmount,
                            InterestId = d.InterestId,
                            Interest = d.mstInterest.Interest,
                            InterestRate = d.InterestRate,
                            InterestAmount = d.InterestAmount,
                            PreviousBalanceAmount = d.PreviousBalanceAmount,
                            DeductionAmount = d.DeductionAmount,
                            NetAmount = d.NetAmount,
                            NetCollectionAmount = d.NetCollectionAmount,
                            CollectibleAmount = d.CollectibleAmount,
                            TotalPaidAmount = d.TotalPaidAmount,
                            TotalPenaltyAmount = d.TotalPenaltyAmount,
                            TotalBalanceAmount = d.TotalBalanceAmount,
                            IsLoanApplication = d.IsLoanApplication,
                            IsLoanReconstruct = d.IsLoanReconstruct,
                            IsLoanRenew = d.IsLoanRenew,
                            IsReconstructed = d.IsReconstructed,
                            IsRenewed = d.IsRenewed,
                            IsLocked = d.IsLocked,
                            CreatedByUserId = d.CreatedByUserId,
                            CreatedByUser = d.mstUser1.FullName,
                            CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                            UpdatedByUserId = d.UpdatedByUserId,
                            UpdatedByUser = d.mstUser2.FullName,
                            UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                        };

            return (Models.TrnLoan)renew.FirstOrDefault();
        }

        // reconstruct list by loan date
        [Authorize]
        [HttpGet]
        [Route("api/reconstruct/listByLoanDate/{startLoanDate}/{endLoanDate}")]
        public List<Models.TrnLoan> listReconstructByLoanDate(String startLoanDate, String endLoanDate)
        {
            var renews = from d in db.trnLoans.OrderByDescending(d => d.Id)
                         join s in db.trnLoanReconstructRenews
                         on d.Id equals s.LoanId
                         into joinReconstructs
                         from listReconstruct in joinReconstructs.DefaultIfEmpty()
                         where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                         && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                         && d.IsLoanReconstruct == true
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
                             MaturityDate = d.MaturityDate.ToShortDateString(),
                             PrincipalAmount = d.PrincipalAmount,
                             InterestId = d.InterestId,
                             Interest = d.mstInterest.Interest,
                             InterestRate = d.InterestRate,
                             InterestAmount = d.InterestAmount,
                             PreviousBalanceAmount = d.PreviousBalanceAmount,
                             DeductionAmount = d.DeductionAmount,
                             NetAmount = d.NetAmount,
                             NetCollectionAmount = d.NetCollectionAmount,
                             CollectibleAmount = d.CollectibleAmount,
                             TotalPaidAmount = d.TotalPaidAmount,
                             TotalPenaltyAmount = d.TotalPenaltyAmount,
                             TotalBalanceAmount = d.TotalBalanceAmount,
                             IsLoanApplication = d.IsLoanApplication,
                             IsLoanReconstruct = d.IsLoanReconstruct,
                             IsLoanRenew = d.IsLoanRenew,
                             IsReconstructed = d.IsReconstructed,
                             IsRenewed = d.IsRenewed,
                             IsLocked = d.IsLocked,
                             CreatedByUserId = d.CreatedByUserId,
                             CreatedByUser = d.mstUser1.FullName,
                             CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                             UpdatedByUserId = d.UpdatedByUserId,
                             UpdatedByUser = d.mstUser2.FullName,
                             UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                             ReconstructedDocNumber = joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanApplication == true ? "LN-" + joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanReconstruct == true ? "RC-" + joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.IsLoanRenew == true ? "RN-" + joinReconstructs.Where(g => g.LoanId == d.Id).FirstOrDefault().trnLoan1.LoanNumber : " "
                         };

            return renews.ToList();
        }

        // reconstruct get by id
        [Authorize]
        [HttpGet]
        [Route("api/reconstruct/getById/{id}")]
        public Models.TrnLoan getReconstructById(String id)
        {
            var renew = from d in db.trnLoans
                        where d.Id == Convert.ToInt32(id)
                        && d.IsLoanReconstruct == true
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
                            MaturityDate = d.MaturityDate.ToShortDateString(),
                            PrincipalAmount = d.PrincipalAmount,
                            InterestId = d.InterestId,
                            Interest = d.mstInterest.Interest,
                            InterestRate = d.InterestRate,
                            InterestAmount = d.InterestAmount,
                            PreviousBalanceAmount = d.PreviousBalanceAmount,
                            DeductionAmount = d.DeductionAmount,
                            NetAmount = d.NetAmount,
                            NetCollectionAmount = d.NetCollectionAmount,
                            CollectibleAmount = d.CollectibleAmount,
                            TotalPaidAmount = d.TotalPaidAmount,
                            TotalPenaltyAmount = d.TotalPenaltyAmount,
                            TotalBalanceAmount = d.TotalBalanceAmount,
                            IsLoanApplication = d.IsLoanApplication,
                            IsLoanReconstruct = d.IsLoanReconstruct,
                            IsLoanRenew = d.IsLoanRenew,
                            IsReconstructed = d.IsReconstructed,
                            IsRenewed = d.IsRenewed,
                            IsLocked = d.IsLocked,
                            CreatedByUserId = d.CreatedByUserId,
                            CreatedByUser = d.mstUser1.FullName,
                            CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                            UpdatedByUserId = d.UpdatedByUserId,
                            UpdatedByUser = d.mstUser2.FullName,
                            UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                        };

            return (Models.TrnLoan)renew.FirstOrDefault();
        }
    }
}
