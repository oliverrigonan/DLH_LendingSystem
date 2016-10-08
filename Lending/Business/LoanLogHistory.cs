using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lending.Business
{
    public class LoanLogHistory
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan log history
        public void postLoanLogHistory(Int32 loanId)
        {
            var loanApplications = from d in db.trnLoanApplications
                                   where d.Id == loanId
                                   && d.IsLocked == true
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
                                       Particulars = d.Particulars,
                                       PreparedByUserId = d.PreparedByUserId,
                                       PreparedByUser = d.mstUser.FullName,
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
                                       IsLocked = d.IsLocked,
                                       CreatedByUserId = d.CreatedByUserId,
                                       CreatedByUser = d.mstUser1.FullName,
                                       CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                       UpdatedByUserId = d.UpdatedByUserId,
                                       UpdatedByUser = d.mstUser2.FullName,
                                       UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                                   };

            if (loanApplications.Any())
            {
                foreach (var loanApplication in loanApplications)
                {
                    if (loanApplication.NetAmount > 0)
                    {
                        var numberOfDays = (Convert.ToDateTime(loanApplication.MaturityDate) - Convert.ToDateTime(loanApplication.LoanDate)).TotalDays;
                        for (var i = 1; i <= numberOfDays; i++)
                        {
                            Data.trnLoanLogHistory newLoanLogHistory = new Data.trnLoanLogHistory();
                            newLoanLogHistory.LoanId = loanId;
                            newLoanLogHistory.CollectibleDate = Convert.ToDateTime(loanApplication.LoanDate).Date.AddDays(i);
                            newLoanLogHistory.NetAmount = loanApplication.NetAmount;
                            newLoanLogHistory.CollectibleAmount = Math.Round(loanApplication.NetAmount / Convert.ToDecimal(numberOfDays), 2);
                            
                            Decimal penaltyValue = 10;
                            if(i % 3 == 0) {
                                penaltyValue = 20;
                            }

                            newLoanLogHistory.Penalty = penaltyValue; 
                            newLoanLogHistory.PaidAmount = 0;
                            newLoanLogHistory.PreviousBalance = 0;
                            newLoanLogHistory.CurrentBalance = 0;
                            newLoanLogHistory.BalanceNetAmount = loanApplication.NetAmount;
                            newLoanLogHistory.IsPenalty = false;
                            db.trnLoanLogHistories.InsertOnSubmit(newLoanLogHistory);
                            db.SubmitChanges();
                        }
                    }
                }
            }
        }

        // delete loan log history
        public void deleteLoanLogHistory(Int32 loanId)
        {
            var loanLogHistories = from d in db.trnLoanLogHistories where d.LoanId == loanId select d;
            if (loanLogHistories.Any())
            {
                db.trnLoanLogHistories.DeleteAllOnSubmit(loanLogHistories);
                db.SubmitChanges();
            }
        }
    }
}