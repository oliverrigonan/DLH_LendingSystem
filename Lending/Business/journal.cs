using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lending.Business
{
    public class Journal
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan journal
        public void postLoanJournal(Int32 loanId)
        {
            var loanApplications = from d in db.trnLoanApplications
                                   where d.Id == loanId
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

            if (loanApplications.Any())
            {
                foreach (var loanApplication in loanApplications)
                {
                    Data.trnJournal newLoanJournal = new Data.trnJournal();
                    newLoanJournal.JournalDate = Convert.ToDateTime(loanApplication.LoanDate);
                    newLoanJournal.BranchId = loanApplication.BranchId;
                    newLoanJournal.AccountId = loanApplication.AccountId;
                    newLoanJournal.Particulars = "NA";
                    newLoanJournal.Amount = loanApplication.LoanAmount;
                    newLoanJournal.DocumentReference = "Loan - " + loanApplication.LoanNumber;
                    newLoanJournal.LoanId = loanId;
                    newLoanJournal.CollectionId = null;
                    newLoanJournal.DisbursementId = null;

                    db.trnJournals.InsertOnSubmit(newLoanJournal);
                    db.SubmitChanges();
                }
            }
        }

        // delete loan journal
        public void deleteLoanJournal(Int32 loanId)
        {
            var journals = from d in db.trnJournals where d.LoanId == loanId select d;
            if (journals.Any())
            {
                db.trnJournals.DeleteAllOnSubmit(journals);
                db.SubmitChanges();
            }
        }
    }
}