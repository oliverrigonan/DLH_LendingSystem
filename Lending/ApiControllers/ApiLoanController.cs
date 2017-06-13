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

        // update loan lines
        public void updateLoan(Int32 loanId)
        {
            var collection = from d in db.trnCollections
                             where d.LoanId == loanId
                             && d.IsLocked == true
                             select d;

            Decimal TotalPaidAmount = 0;
            Decimal TotalPenaltyAmount = 0;
            if (collection.Any())
            {
                TotalPaidAmount = collection.Sum(d => d.TotalPaidAmount);
                TotalPenaltyAmount = collection.Sum(d => d.TotalPenaltyAmount);
            }

            var loan = from d in db.trnLoans where d.Id == loanId select d;
            if (loan.Any())
            {
                var updateLoan = loan.FirstOrDefault();
                updateLoan.TotalPaidAmount = TotalPaidAmount;
                updateLoan.TotalPenaltyAmount = TotalPenaltyAmount;
                updateLoan.TotalBalanceAmount = (loan.FirstOrDefault().NetCollectionAmount - TotalPaidAmount) + TotalPenaltyAmount;
                db.SubmitChanges();
            }
        }

        // loan applicants
        [Authorize]
        [HttpGet]
        [Route("api/loan/list/loanApplicants")]
        public List<Models.TrnLoan> listLoanApplicants()
        {
            var loanApplicants = from d in db.trnLoans.OrderBy(d => d.mstApplicant.ApplicantLastName)
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

            return loanApplicants.OrderBy(d => d.Applicant).ToList();
        }

        // loan list by applicantId
        [Authorize]
        [HttpGet]
        [Route("api/loan/list/byApplicantId/{applicantId}")]
        public List<Models.TrnLoan> listLoanByApplicantId(String applicantId)
        {
            var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                   where d.ApplicantId == Convert.ToInt32(applicantId)
                                   select new Models.TrnLoan
                                   {
                                       Id = d.Id,
                                       LoanNumberDetail = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
                                       CollectibleAmount = d.CollectibleAmount,
                                       TotalPaidAmount = d.TotalPaidAmount,
                                       TotalPenaltyAmount = d.TotalPenaltyAmount,
                                       TotalBalanceAmount = d.TotalBalanceAmount,
                                       NetAmount = d.NetAmount,
                                       IsLoanApplication = d.IsLoanApplication,
                                       IsLoanRenew = d.IsLoanRenew,
                                       IsLoanReconstruct = d.IsLoanReconstruct
                                   };

            return loanApplications.ToList();
        }


        // loan applicants
        [Authorize]
        [HttpGet]
        [Route("api/loan/list/loanApplicants/ForReturnRelease")]
        public List<Models.TrnLoan> listLoanApplicantsForReturnRelease()
        {
            var loanApplicants = from d in db.trnLoans.OrderBy(d => d.mstApplicant.ApplicantLastName)
                                 where d.IsLoanReconstruct == false
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

            return loanApplicants.OrderBy(d => d.Applicant).ToList();
        }


        // loan list by applicantId
        [Authorize]
        [HttpGet]
        [Route("api/loan/list/byApplicantId/ForReturnRelease/{applicantId}")]
        public List<Models.TrnLoan> listLoanByApplicantIdForReturnRelease(String applicantId)
        {
            var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                   where d.ApplicantId == Convert.ToInt32(applicantId)
                                   && d.IsLoanReconstruct == false
                                   select new Models.TrnLoan
                                   {
                                       Id = d.Id,
                                       LoanNumberDetail = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
                                       CollectibleAmount = d.CollectibleAmount,
                                       TotalPaidAmount = d.TotalPaidAmount,
                                       TotalPenaltyAmount = d.TotalPenaltyAmount,
                                       TotalBalanceAmount = d.TotalBalanceAmount,
                                       NetAmount = d.NetAmount,
                                       IsLoanApplication = d.IsLoanApplication,
                                       IsLoanRenew = d.IsLoanRenew,
                                       IsLoanReconstruct = d.IsLoanReconstruct
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

            return loanApplicants.OrderBy(d => d.Applicant).ToList();
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
                                   select new Models.TrnLoan
                                   {
                                       Id = d.Id,
                                       LoanNumberDetail = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
                                       TotalBalanceAmount = d.TotalBalanceAmount
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
                                       LoanNumberDetail = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
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
                                       IsReturnRelease = d.IsReturnRelease,
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

        // renew list by loan date
        [Authorize]
        [HttpGet]
        [Route("api/renew/listByLoanDate/{startLoanDate}/{endLoanDate}")]
        public List<Models.TrnLoan> listRenewByLoanDate(String startLoanDate, String endLoanDate)
        {
            var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
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
                                       IsReturnRelease = d.IsReturnRelease,
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

        // reconstruct list by loan date
        [Authorize]
        [HttpGet]
        [Route("api/reconstruct/listByLoanDate/{startLoanDate}/{endLoanDate}")]
        public List<Models.TrnLoan> listReconstructByLoanDate(String startLoanDate, String endLoanDate)
        {
            var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
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
                                       IsReturnRelease = d.IsReturnRelease,
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
        [Route("api/loan/getById/{id}/{loanType}")]
        public Models.TrnLoan getLoanById(String id, String loanType)
        {
            if (loanType.Equals("loan"))
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
                               IsReturnRelease = d.IsReturnRelease,
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
            else
            {
                if (loanType.Equals("renew"))
                {
                    var loan = from d in db.trnLoans
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
                                   IsReturnRelease = d.IsReturnRelease,
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
                else
                {
                    if (loanType.Equals("reconstruct"))
                    {
                        var loan = from d in db.trnLoans
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
                                       IsReturnRelease = d.IsReturnRelease,
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
                    else
                    {
                        return null;
                    }
                }
            }
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
        [Route("api/loan/add/{loanType}")]
        public Int32 addLoanApplication(String loanType)
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

                        var applicant = from d in db.mstApplicants.OrderByDescending(d => d.Id)
                                        where d.IsCoMaker != true
                                        select d;

                        if (applicant.Any())
                        {
                            var term = from d in db.mstTerms.OrderByDescending(d => d.Id) select d;
                            if (term.Any())
                            {
                                var interest = from d in db.mstInterests.OrderByDescending(d => d.Id) select d;
                                if (interest.Any())
                                {
                                    if (loanType.Equals("loan"))
                                    {
                                        var loan = from d in db.trnLoans.OrderByDescending(d => d.Id) where d.IsLoanApplication == true select d;
                                        if (loan.Any())
                                        {
                                            var newLoanNumber = Convert.ToInt32(loan.FirstOrDefault().LoanNumber) + 0000000001;
                                            loanNumber = newLoanNumber.ToString();
                                        }

                                        Data.trnLoan newLoan = new Data.trnLoan();
                                        newLoan.LoanNumber = zeroFill(Convert.ToInt32(loanNumber), 10);
                                        newLoan.LoanDate = DateTime.Today;
                                        newLoan.ApplicantId = applicant.FirstOrDefault().Id;
                                        newLoan.Particulars = "NA";
                                        newLoan.PreparedByUserId = userId;
                                        newLoan.TermId = term.FirstOrDefault().Id;
                                        newLoan.TermNoOfDays = term.FirstOrDefault().NoOfDays;
                                        newLoan.MaturityDate = DateTime.Today;
                                        newLoan.PrincipalAmount = 0;
                                        newLoan.InterestId = interest.FirstOrDefault().Id;
                                        newLoan.InterestRate = interest.FirstOrDefault().Rate;
                                        newLoan.InterestAmount = 0;
                                        newLoan.PreviousBalanceAmount = 0;
                                        newLoan.DeductionAmount = 0;
                                        newLoan.NetAmount = 0;
                                        newLoan.NetCollectionAmount = 0;
                                        newLoan.CollectibleAmount = 0;
                                        newLoan.TotalPaidAmount = 0;
                                        newLoan.TotalPenaltyAmount = 0;
                                        newLoan.TotalBalanceAmount = 0;
                                        newLoan.IsLoanApplication = true;
                                        newLoan.IsLoanReconstruct = false;
                                        newLoan.IsLoanRenew = false;
                                        newLoan.IsReturnRelease = false;
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
                                        if (loanType.Equals("renew"))
                                        {
                                            var loan = from d in db.trnLoans.OrderByDescending(d => d.Id) where d.IsLoanRenew == true select d;
                                            if (loan.Any())
                                            {
                                                var newLoanNumber = Convert.ToInt32(loan.FirstOrDefault().LoanNumber) + 0000000001;
                                                loanNumber = newLoanNumber.ToString();
                                            }

                                            Data.trnLoan newLoan = new Data.trnLoan();
                                            newLoan.LoanNumber = zeroFill(Convert.ToInt32(loanNumber), 10);
                                            newLoan.LoanDate = DateTime.Today;
                                            newLoan.ApplicantId = applicant.FirstOrDefault().Id;
                                            newLoan.Particulars = "NA";
                                            newLoan.PreparedByUserId = userId;
                                            newLoan.TermId = term.FirstOrDefault().Id;
                                            newLoan.TermNoOfDays = term.FirstOrDefault().NoOfDays;
                                            newLoan.MaturityDate = DateTime.Today;
                                            newLoan.PrincipalAmount = 0;
                                            newLoan.InterestId = interest.FirstOrDefault().Id;
                                            newLoan.InterestRate = interest.FirstOrDefault().Rate;
                                            newLoan.InterestAmount = 0;
                                            newLoan.PreviousBalanceAmount = 0;
                                            newLoan.DeductionAmount = 0;
                                            newLoan.NetAmount = 0;
                                            newLoan.NetCollectionAmount = 0;
                                            newLoan.CollectibleAmount = 0;
                                            newLoan.TotalPaidAmount = 0;
                                            newLoan.TotalPenaltyAmount = 0;
                                            newLoan.TotalBalanceAmount = 0;
                                            newLoan.IsLoanApplication = false;
                                            newLoan.IsLoanReconstruct = false;
                                            newLoan.IsLoanRenew = true;
                                            newLoan.IsReturnRelease = false;
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
                                            if (loanType.Equals("reconstruct"))
                                            {
                                                var loan = from d in db.trnLoans.OrderByDescending(d => d.Id) where d.IsLoanReconstruct == true select d;
                                                if (loan.Any())
                                                {
                                                    var newLoanNumber = Convert.ToInt32(loan.FirstOrDefault().LoanNumber) + 0000000001;
                                                    loanNumber = newLoanNumber.ToString();
                                                }

                                                Data.trnLoan newLoan = new Data.trnLoan();
                                                newLoan.LoanNumber = zeroFill(Convert.ToInt32(loanNumber), 10);
                                                newLoan.LoanDate = DateTime.Today;
                                                newLoan.ApplicantId = applicant.FirstOrDefault().Id;
                                                newLoan.Particulars = "NA";
                                                newLoan.PreparedByUserId = userId;
                                                newLoan.TermId = term.FirstOrDefault().Id;
                                                newLoan.TermNoOfDays = term.FirstOrDefault().NoOfDays;
                                                newLoan.MaturityDate = DateTime.Today;
                                                newLoan.PrincipalAmount = 0;
                                                newLoan.InterestId = interest.FirstOrDefault().Id;
                                                newLoan.InterestRate = interest.FirstOrDefault().Rate;
                                                newLoan.InterestAmount = 0;
                                                newLoan.PreviousBalanceAmount = 0;
                                                newLoan.DeductionAmount = 0;
                                                newLoan.NetAmount = 0;
                                                newLoan.NetCollectionAmount = 0;
                                                newLoan.CollectibleAmount = 0;
                                                newLoan.TotalPaidAmount = 0;
                                                newLoan.TotalPenaltyAmount = 0;
                                                newLoan.TotalBalanceAmount = 0;
                                                newLoan.IsLoanApplication = false;
                                                newLoan.IsLoanReconstruct = true;
                                                newLoan.IsLoanRenew = false;
                                                newLoan.IsReturnRelease = false;
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

                            if (loans.FirstOrDefault().IsLoanReconstruct)
                            {
                                matchPageString = "ReconstructDetail";
                            }
                            else
                            {
                                if (loans.FirstOrDefault().IsLoanRenew)
                                {
                                    matchPageString = "RenewDetail";
                                }
                            }

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
                                    lockLoan.MaturityDate = DateTime.Today;
                                    lockLoan.PrincipalAmount = loan.PrincipalAmount;
                                    lockLoan.InterestId = loan.InterestId;
                                    lockLoan.InterestRate = loan.InterestRate;
                                    lockLoan.InterestAmount = loan.InterestAmount;
                                    lockLoan.PreviousBalanceAmount = loan.PreviousBalanceAmount;
                                    lockLoan.DeductionAmount = deductionAmount;
                                    lockLoan.NetAmount = loan.NetAmount;
                                    lockLoan.NetCollectionAmount = loan.NetCollectionAmount;
                                    lockLoan.IsLocked = true;
                                    lockLoan.UpdatedByUserId = userId;
                                    lockLoan.UpdatedDateTime = DateTime.Now;
                                    db.SubmitChanges();

                                    if (loan.TermNoOfDays > 0)
                                    {
                                        Decimal collectibleAmount = loan.NetCollectionAmount / loan.TermNoOfDays;
                                        Decimal ceilCollectibleAmount = Math.Ceiling(collectibleAmount / 5) * 5;

                                        lockLoan.MaturityDate = Convert.ToDateTime(loan.LoanDate).AddDays(Convert.ToDouble(loan.TermNoOfDays));
                                        lockLoan.CollectibleAmount = ceilCollectibleAmount;
                                        lockLoan.TotalBalanceAmount = (loans.FirstOrDefault().NetCollectionAmount - loans.FirstOrDefault().TotalPaidAmount) + loan.TotalPenaltyAmount;
                                        db.SubmitChanges();
                                    }

                                    updateLoan(Convert.ToInt32(id));

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

                            if (loans.FirstOrDefault().IsLoanReconstruct)
                            {
                                matchPageString = "ReconstructDetail";
                            }
                            else
                            {
                                if (loans.FirstOrDefault().IsLoanRenew)
                                {
                                    matchPageString = "RenewDetail";
                                }
                            }

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

                                updateLoan(Convert.ToInt32(id));

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to unlock record.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to unlock record.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Already Unlocked.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry. Data not found.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong from the server.");
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

                            if (loans.FirstOrDefault().IsLoanReconstruct)
                            {
                                matchPageString = "ReconstructDetail";
                            }
                            else
                            {
                                if (loans.FirstOrDefault().IsLoanRenew)
                                {
                                    matchPageString = "RenewDetail";
                                }
                            }

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
                                var collection = from d in db.trnCollections
                                                 where d.LoanId == Convert.ToInt32(id)
                                                 && d.IsLocked == true
                                                 select d;

                                if (!collection.Any())
                                {
                                    db.trnLoans.DeleteOnSubmit(loans.First());
                                    db.SubmitChanges();

                                    return Request.CreateResponse(HttpStatusCode.OK);
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. Cannot delete if there are locked collections exist.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to delete record.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to delete record.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. Cannot delete locked record.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry. Data not found");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong from the server.");
            }
        }

        // loan list by areaid - active
        [Authorize]
        [HttpGet]
        [Route("api/loan/active/{areaId}/{date}")]
        public List<Models.TrnLoan> listActive(String areaId, String date)
        {
            if (areaId.Equals("0"))
            {
                var loanApplications = from d in db.trnLoans
                                       where d.IsLocked == true
                                       && d.TotalBalanceAmount > 0
                                       && d.IsReturnRelease == false
                                       select new Models.TrnLoan
                                       {
                                           ApplicantId = d.ApplicantId,
                                           Id = d.Id,
                                           Applicant = d.mstApplicant.ApplicantLastName + " " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                           LoanNumber = d.LoanNumber,
                                           DateTImeMaturityDate = d.MaturityDate,
                                           TotalBalanceAmount = d.TotalBalanceAmount,
                                           CollectibleAmount = d.CollectibleAmount,
                                           IsLoanRenew = d.IsLoanRenew,
                                           IsLoanReconstruct = d.IsLoanReconstruct,
                                           IsReturnRelease = d.IsReturnRelease
                                       };

                var grouploanApplications = from d in loanApplications.OrderByDescending(d => d.Id)
                                            group d by d.ApplicantId into g
                                            select new Models.TrnLoan
                                            {
                                                ApplicantId = g.FirstOrDefault().ApplicantId,
                                                Id = g.FirstOrDefault().Id,
                                                Applicant = g.FirstOrDefault().Applicant,
                                                LoanNumber = g.FirstOrDefault().LoanNumber,
                                                DateTImeMaturityDate = g.FirstOrDefault().DateTImeMaturityDate,
                                                TotalBalanceAmount = g.FirstOrDefault().TotalBalanceAmount,
                                                CollectibleAmount = g.FirstOrDefault().CollectibleAmount,
                                                IsLoanRenew = g.FirstOrDefault().IsLoanRenew,
                                                IsLoanReconstruct = g.FirstOrDefault().IsLoanReconstruct,
                                                IsReturnRelease = g.FirstOrDefault().IsReturnRelease,
                                            };

                var loanApplicationList = from d in grouploanApplications.OrderByDescending(d => d.Id)
                                          where d.IsLoanReconstruct == false
                                          select new Models.TrnLoan
                                          {
                                              ApplicantId = d.ApplicantId,
                                              Id = d.Id,
                                              Applicant = d.Applicant,
                                              LoanNumber = d.LoanNumber,
                                              MaturityDate = d.DateTImeMaturityDate.ToShortDateString(),
                                              TotalBalanceAmount = d.TotalBalanceAmount,
                                              CollectibleAmount = d.CollectibleAmount,
                                              IsLoanRenew = d.IsLoanRenew,
                                              IsLoanReconstruct = d.IsLoanReconstruct,
                                              IsReturnRelease = d.IsReturnRelease
                                          };

                return loanApplicationList.OrderBy(d => d.Applicant).ToList();
            }
            else
            {
                var loanApplications = from d in db.trnLoans
                                       where d.IsLocked == true
                                       && d.TotalBalanceAmount > 0
                                       && d.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                       && d.IsReturnRelease == false
                                       select new Models.TrnLoan
                                       {
                                           ApplicantId = d.ApplicantId,
                                           Id = d.Id,
                                           Applicant = d.mstApplicant.ApplicantLastName + " " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                           LoanNumber = d.LoanNumber,
                                           DateTImeMaturityDate = d.MaturityDate,
                                           TotalBalanceAmount = d.TotalBalanceAmount,
                                           CollectibleAmount = d.CollectibleAmount,
                                           IsLoanRenew = d.IsLoanRenew,
                                           IsLoanReconstruct = d.IsLoanReconstruct,
                                           IsReturnRelease = d.IsReturnRelease
                                       };

                var grouploanApplications = from d in loanApplications.OrderByDescending(d => d.Id)
                                            group d by d.ApplicantId into g
                                            select new Models.TrnLoan
                                            {
                                                ApplicantId = g.FirstOrDefault().ApplicantId,
                                                Id = g.FirstOrDefault().Id,
                                                Applicant = g.FirstOrDefault().Applicant,
                                                LoanNumber = g.FirstOrDefault().LoanNumber,
                                                DateTImeMaturityDate = g.FirstOrDefault().DateTImeMaturityDate,
                                                TotalBalanceAmount = g.FirstOrDefault().TotalBalanceAmount,
                                                CollectibleAmount = g.FirstOrDefault().CollectibleAmount,
                                                IsLoanRenew = g.FirstOrDefault().IsLoanRenew,
                                                IsLoanReconstruct = g.FirstOrDefault().IsLoanReconstruct,
                                                IsReturnRelease = g.FirstOrDefault().IsReturnRelease
                                            };

                var loanApplicationList = from d in grouploanApplications.OrderByDescending(d => d.Id)
                                          where d.IsLoanReconstruct == false
                                          select new Models.TrnLoan
                                          {
                                              ApplicantId = d.ApplicantId,
                                              Id = d.Id,
                                              Applicant = d.Applicant,
                                              LoanNumber = d.LoanNumber,
                                              MaturityDate = d.DateTImeMaturityDate.ToShortDateString(),
                                              TotalBalanceAmount = d.TotalBalanceAmount,
                                              CollectibleAmount = d.CollectibleAmount,
                                              IsLoanRenew = d.IsLoanRenew,
                                              IsReturnRelease = d.IsReturnRelease
                                          };

                return loanApplicationList.OrderBy(d => d.Applicant).ToList();
            }
        }

        // loan list by areaid - overdue
        [Authorize]
        [HttpGet]
        [Route("api/loan/overdue/{areaId}/{date}")]
        public List<Models.TrnLoan> listOverdue(String areaId, String date)
        {
            if (areaId.Equals("0"))
            {
                var loanApplications = from d in db.trnLoans
                                       where d.IsLocked == true
                                       && d.TotalBalanceAmount > 0
                                       && d.IsReturnRelease == false
                                       select new Models.TrnLoan
                                       {
                                           ApplicantId = d.ApplicantId,
                                           Id = d.Id,
                                           Applicant = d.mstApplicant.ApplicantLastName + " " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                           LoanNumber = d.LoanNumber,
                                           DateTImeLoanDate = d.LoanDate,
                                           DateTImeMaturityDate = d.MaturityDate,
                                           TotalBalanceAmount = d.TotalBalanceAmount,
                                           CollectibleAmount = d.CollectibleAmount,
                                           IsLoanRenew = d.IsLoanRenew,
                                           IsLoanReconstruct = d.IsLoanReconstruct,
                                           IsLocked = d.IsLocked,
                                           Particulars = d.Particulars
                                       };

                var grouploanApplications = from d in loanApplications.OrderByDescending(d => d.Id)
                                            group d by d.ApplicantId into g
                                            select new Models.TrnLoan
                                            {
                                                ApplicantId = g.FirstOrDefault().ApplicantId,
                                                Id = g.FirstOrDefault().Id,
                                                Applicant = g.FirstOrDefault().Applicant,
                                                LoanNumber = g.FirstOrDefault().LoanNumber,
                                                DateTImeLoanDate = g.FirstOrDefault().DateTImeLoanDate,
                                                DateTImeMaturityDate = g.FirstOrDefault().DateTImeMaturityDate,
                                                TotalBalanceAmount = g.FirstOrDefault().TotalBalanceAmount,
                                                CollectibleAmount = g.FirstOrDefault().CollectibleAmount,
                                                IsLoanRenew = g.FirstOrDefault().IsLoanRenew,
                                                IsLoanReconstruct = g.FirstOrDefault().IsLoanReconstruct,
                                                IsLocked = g.FirstOrDefault().IsLoanRenew,
                                                Particulars = g.FirstOrDefault().Particulars
                                            };

                var loanApplicationList = from d in grouploanApplications.OrderByDescending(d => d.Id)
                                          where d.IsLoanReconstruct == true
                                          select new Models.TrnLoan
                                          {
                                              ApplicantId = d.ApplicantId,
                                              Id = d.Id,
                                              Applicant = d.Applicant,
                                              LoanNumber = d.LoanNumber,
                                              LoanDate = d.DateTImeLoanDate.ToLongDateString(),
                                              MaturityDate = d.DateTImeMaturityDate.ToShortDateString(),
                                              TotalBalanceAmount = d.TotalBalanceAmount,
                                              CollectibleAmount = d.CollectibleAmount,
                                              IsLoanRenew = d.IsLoanRenew,
                                              IsLoanReconstruct = d.IsLoanReconstruct,
                                              IsLocked = d.IsLoanRenew,
                                              Particulars = d.Particulars
                                          };

                return loanApplicationList.OrderBy(d => d.Applicant).ToList();
            }
            else
            {
                var loanApplications = from d in db.trnLoans
                                       where d.IsLocked == true
                                       && d.TotalBalanceAmount > 0
                                       && d.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                       && d.IsReturnRelease == false
                                       select new Models.TrnLoan
                                       {
                                           ApplicantId = d.ApplicantId,
                                           Id = d.Id,
                                           Applicant = d.mstApplicant.ApplicantLastName + " " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                           LoanNumber = d.LoanNumber,
                                           DateTImeLoanDate = d.LoanDate,
                                           DateTImeMaturityDate = d.MaturityDate,
                                           TotalBalanceAmount = d.TotalBalanceAmount,
                                           CollectibleAmount = d.CollectibleAmount,
                                           IsLoanRenew = d.IsLoanRenew,
                                           IsLoanReconstruct = d.IsLoanReconstruct,
                                           IsLocked = d.IsLocked,
                                           Particulars = d.Particulars
                                       };

                var grouploanApplications = from d in loanApplications.OrderByDescending(d => d.Id)
                                            group d by d.ApplicantId into g
                                            select new Models.TrnLoan
                                            {
                                                ApplicantId = g.FirstOrDefault().ApplicantId,
                                                Id = g.FirstOrDefault().Id,
                                                Applicant = g.FirstOrDefault().Applicant,
                                                LoanNumber = g.FirstOrDefault().LoanNumber,
                                                DateTImeLoanDate = g.FirstOrDefault().DateTImeLoanDate,
                                                DateTImeMaturityDate = g.FirstOrDefault().DateTImeMaturityDate,
                                                TotalBalanceAmount = g.FirstOrDefault().TotalBalanceAmount,
                                                CollectibleAmount = g.FirstOrDefault().CollectibleAmount,
                                                IsLoanRenew = g.FirstOrDefault().IsLoanRenew,
                                                IsLoanReconstruct = g.FirstOrDefault().IsLoanReconstruct,
                                                IsLocked = g.FirstOrDefault().IsLoanRenew,
                                                Particulars = g.FirstOrDefault().Particulars
                                            };

                var loanApplicationList = from d in grouploanApplications.OrderByDescending(d => d.Id)
                                          where d.IsLoanReconstruct == true
                                          select new Models.TrnLoan
                                          {
                                              ApplicantId = d.ApplicantId,
                                              Id = d.Id,
                                              Applicant = d.Applicant,
                                              LoanNumber = d.LoanNumber,
                                              LoanDate = d.DateTImeLoanDate.ToLongDateString(),
                                              MaturityDate = d.DateTImeMaturityDate.ToShortDateString(),
                                              TotalBalanceAmount = d.TotalBalanceAmount,
                                              CollectibleAmount = d.CollectibleAmount,
                                              IsLoanRenew = d.IsLoanRenew,
                                              IsLoanReconstruct = d.IsLoanReconstruct,
                                              IsLocked = d.IsLoanRenew,
                                              Particulars = d.Particulars
                                          };

                return loanApplicationList.OrderBy(d => d.Applicant).ToList();
            }
        }

        // daily release monitoring
        [Authorize]
        [HttpGet]
        [Route("api/loan/dailyReleaseMonitoring/{areaId}/{startLoanDate}/{endLoanDate}")]
        public List<Models.TrnLoan> listDailyReleaseMonitoring(String areaId, String startLoanDate, String endLoanDate)
        {
            if (areaId.Equals("0"))
            {
                var loanApplications = from d in db.trnLoans.OrderBy(d => d.mstApplicant.ApplicantLastName)
                                       where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                       && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                       && d.IsLocked == true
                                       && d.IsLoanReconstruct != true
                                       select new Models.TrnLoan
                                       {
                                           Id = d.Id,
                                           LoanNumber = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
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
                                           TotalPaidAmount = d.TotalPaidAmount,
                                           TotalPenaltyAmount = d.TotalPenaltyAmount,
                                           TotalBalanceAmount = d.TotalBalanceAmount,
                                           IsLoanApplication = d.IsLoanApplication,
                                           IsLoanReconstruct = d.IsLoanReconstruct,
                                           IsLoanRenew = d.IsLoanRenew,
                                           IsReturnRelease = d.IsReturnRelease,
                                           IsLocked = d.IsLocked,
                                           CreatedByUserId = d.CreatedByUserId,
                                           CreatedByUser = d.mstUser1.FullName,
                                           CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                           UpdatedByUserId = d.UpdatedByUserId,
                                           UpdatedByUser = d.mstUser2.FullName,
                                           UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                       };

                return loanApplications.ToList();
            }
            else
            {
                var loanApplications = from d in db.trnLoans.OrderBy(d => d.mstApplicant.ApplicantLastName)
                                       where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                       && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                       && d.mstApplicant.AreaId == Convert.ToInt32(areaId)
                                       && d.IsLocked == true
                                       && d.IsLoanReconstruct != true
                                       select new Models.TrnLoan
                                       {
                                           Id = d.Id,
                                           LoanNumber = d.IsLoanApplication == true ? "LN-" + d.LoanNumber : d.IsLoanReconstruct == true ? "RC-" + d.LoanNumber : d.IsLoanRenew == true ? "RN-" + d.LoanNumber : " ",
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
                                           TotalPaidAmount = d.TotalPaidAmount,
                                           TotalPenaltyAmount = d.TotalPenaltyAmount,
                                           TotalBalanceAmount = d.TotalBalanceAmount,
                                           IsLoanApplication = d.IsLoanApplication,
                                           IsLoanReconstruct = d.IsLoanReconstruct,
                                           IsLoanRenew = d.IsLoanRenew,
                                           IsReturnRelease = d.IsReturnRelease,
                                           IsLocked = d.IsLocked,
                                           CreatedByUserId = d.CreatedByUserId,
                                           CreatedByUser = d.mstUser1.FullName,
                                           CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                           UpdatedByUserId = d.UpdatedByUserId,
                                           UpdatedByUser = d.mstUser2.FullName,
                                           UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                       };

                return loanApplications.ToList();
            }
        }

        // loan renew reconstruct report
        [Authorize]
        [HttpGet]
        [Route("api/loan/renew/reconstruct/summary/report/{loanType}/{areaId}/{startLoanDate}/{endLoanDate}")]
        public List<Models.TrnLoan> loanRenewReconstructSummaryReports(String loanType, String areaId, String startLoanDate, String endLoanDate)
        {
            if (loanType.Equals("loan"))
            {
                if (areaId.Equals("0"))
                {
                    var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                           where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                           && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                           && d.IsLocked == true
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
                                               TotalPaidAmount = d.TotalPaidAmount,
                                               TotalPenaltyAmount = d.TotalPenaltyAmount,
                                               TotalBalanceAmount = d.TotalBalanceAmount,
                                               IsLoanApplication = d.IsLoanApplication,
                                               IsLoanReconstruct = d.IsLoanReconstruct,
                                               IsLoanRenew = d.IsLoanRenew,
                                               IsReturnRelease = d.IsReturnRelease,
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
                    var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                           where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                           && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                           && d.IsLocked == true
                                           && d.mstApplicant.AreaId == Convert.ToInt32(areaId)
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
                                               TotalPaidAmount = d.TotalPaidAmount,
                                               TotalPenaltyAmount = d.TotalPenaltyAmount,
                                               TotalBalanceAmount = d.TotalBalanceAmount,
                                               IsLoanApplication = d.IsLoanApplication,
                                               IsLoanReconstruct = d.IsLoanReconstruct,
                                               IsLoanRenew = d.IsLoanRenew,
                                               IsReturnRelease = d.IsReturnRelease,
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
            }
            else
            {
                if (loanType.Equals("renew"))
                {
                    if (areaId.Equals("0"))
                    {
                        var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                               where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                               && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                               && d.IsLocked == true
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
                                                   TotalPaidAmount = d.TotalPaidAmount,
                                                   TotalPenaltyAmount = d.TotalPenaltyAmount,
                                                   TotalBalanceAmount = d.TotalBalanceAmount,
                                                   IsLoanApplication = d.IsLoanApplication,
                                                   IsLoanReconstruct = d.IsLoanReconstruct,
                                                   IsLoanRenew = d.IsLoanRenew,
                                                   IsReturnRelease = d.IsReturnRelease,
                                                   IsLocked = d.IsLocked,
                                                   CreatedByUserId = d.CreatedByUserId,
                                                   CreatedByUser = d.mstUser1.FullName,
                                                   CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                                   UpdatedByUserId = d.UpdatedByUserId,
                                                   UpdatedByUser = d.mstUser2.FullName,
                                                   UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                               };

                        return loanApplications.ToList();
                    }
                    else
                    {
                        var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                               where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                               && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                               && d.IsLocked == true
                                               && d.mstApplicant.AreaId == Convert.ToInt32(areaId)
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
                                                   TotalPaidAmount = d.TotalPaidAmount,
                                                   TotalPenaltyAmount = d.TotalPenaltyAmount,
                                                   TotalBalanceAmount = d.TotalBalanceAmount,
                                                   IsLoanApplication = d.IsLoanApplication,
                                                   IsLoanReconstruct = d.IsLoanReconstruct,
                                                   IsLoanRenew = d.IsLoanRenew,
                                                   IsReturnRelease = d.IsReturnRelease,
                                                   IsLocked = d.IsLocked,
                                                   CreatedByUserId = d.CreatedByUserId,
                                                   CreatedByUser = d.mstUser1.FullName,
                                                   CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                                   UpdatedByUserId = d.UpdatedByUserId,
                                                   UpdatedByUser = d.mstUser2.FullName,
                                                   UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                               };

                        return loanApplications.ToList();
                    }
                }
                else
                {
                    if (loanType.Equals("reconstruct"))
                    {
                        if (areaId.Equals("0"))
                        {
                            var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                                   where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                                   && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                                   && d.IsLocked == true
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
                                                       TotalPaidAmount = d.TotalPaidAmount,
                                                       TotalPenaltyAmount = d.TotalPenaltyAmount,
                                                       TotalBalanceAmount = d.TotalBalanceAmount,
                                                       IsLoanApplication = d.IsLoanApplication,
                                                       IsLoanReconstruct = d.IsLoanReconstruct,
                                                       IsLoanRenew = d.IsLoanRenew,
                                                       IsReturnRelease = d.IsReturnRelease,
                                                       IsLocked = d.IsLocked,
                                                       CreatedByUserId = d.CreatedByUserId,
                                                       CreatedByUser = d.mstUser1.FullName,
                                                       CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                                       UpdatedByUserId = d.UpdatedByUserId,
                                                       UpdatedByUser = d.mstUser2.FullName,
                                                       UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                                   };

                            return loanApplications.ToList();
                        }
                        else
                        {
                            var loanApplications = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                                   where d.LoanDate >= Convert.ToDateTime(startLoanDate)
                                                   && d.LoanDate <= Convert.ToDateTime(endLoanDate)
                                                   && d.IsLocked == true
                                                   && d.mstApplicant.AreaId == Convert.ToInt32(areaId)
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
                                                       TotalPaidAmount = d.TotalPaidAmount,
                                                       TotalPenaltyAmount = d.TotalPenaltyAmount,
                                                       TotalBalanceAmount = d.TotalBalanceAmount,
                                                       IsLoanApplication = d.IsLoanApplication,
                                                       IsLoanReconstruct = d.IsLoanReconstruct,
                                                       IsLoanRenew = d.IsLoanRenew,
                                                       IsReturnRelease = d.IsReturnRelease,
                                                       IsLocked = d.IsLocked,
                                                       CreatedByUserId = d.CreatedByUserId,
                                                       CreatedByUser = d.mstUser1.FullName,
                                                       CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                                       UpdatedByUserId = d.UpdatedByUserId,
                                                       UpdatedByUser = d.mstUser2.FullName,
                                                       UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                                   };

                            return loanApplications.ToList();
                        }
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
