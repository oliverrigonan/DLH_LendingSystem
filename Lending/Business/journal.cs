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
                                       AccountId = d.AccountId,
                                       Account = d.mstAccount.Account,
                                       ApplicantId = d.ApplicantId,
                                       Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                       Particulars = d.Particulars,
                                       PreparedByUserId = d.PreparedByUserId,
                                       PreparedByUser = d.mstUser.FullName,
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
                        Data.trnJournal newLoanJournal = new Data.trnJournal();
                        newLoanJournal.JournalDate = Convert.ToDateTime(loanApplication.LoanDate);
                        newLoanJournal.AccountId = loanApplication.AccountId;
                        newLoanJournal.Particulars = loanApplication.Particulars;
                        newLoanJournal.ReleasedAmount = loanApplication.NetAmount;
                        newLoanJournal.ReceivedAmount = 0;
                        newLoanJournal.DocumentReference = "Loan - " + loanApplication.LoanNumber;
                        newLoanJournal.LoanId = loanId;
                        newLoanJournal.CollectionId = null;
                        newLoanJournal.ExpenseId = null;

                        db.trnJournals.InsertOnSubmit(newLoanJournal);
                        db.SubmitChanges();
                    }
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

        // expenses journal
        public void postExpensesJournal(Int32 expenseId)
        {
            var expenses = from d in db.trnExpenses
                           where d.Id == expenseId
                           select new Models.TrnExpenses
                           {
                               Id = d.Id,
                               ExpenseNumber = d.ExpenseNumber,
                               ExpenseDate = d.ExpenseDate.ToShortDateString(),
                               AccountId = d.AccountId,
                               Account = d.mstAccount.Account,
                               CollectorId = d.CollectorId,
                               Collector = d.mstCollector.Collector,
                               ExpenseTypeId = d.ExpenseTypeId,
                               ExpenseType = d.mstExpenseType.ExpenseType,
                               Particulars = d.Particulars,
                               ExpenseAmount = d.ExpenseAmount,
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

            if (expenses.Any())
            {
                foreach (var expense in expenses)
                {
                    if (expense.ExpenseAmount > 0)
                    {
                        Data.trnJournal newLoanJournal = new Data.trnJournal();
                        newLoanJournal.JournalDate = Convert.ToDateTime(expense.ExpenseDate);
                        newLoanJournal.AccountId = expense.AccountId;
                        newLoanJournal.Particulars = expense.Particulars;
                        newLoanJournal.ReleasedAmount = expense.ExpenseAmount;
                        newLoanJournal.ReceivedAmount = 0;
                        newLoanJournal.DocumentReference = "Expenses - " + expense.ExpenseNumber;
                        newLoanJournal.LoanId = null;
                        newLoanJournal.CollectionId = null;
                        newLoanJournal.ExpenseId = expense.Id;

                        db.trnJournals.InsertOnSubmit(newLoanJournal);
                        db.SubmitChanges();
                    }
                }
            }
        }

        // delete expenses journal
        public void deleteExpensesJournal(Int32 expenseId)
        {
            var journals = from d in db.trnJournals where d.ExpenseId == expenseId select d;
            if (journals.Any())
            {
                db.trnJournals.DeleteAllOnSubmit(journals);
                db.SubmitChanges();
            }
        }
    }
}