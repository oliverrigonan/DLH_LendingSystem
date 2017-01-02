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
        private Business.CollectionStatus collectionStatus = new Business.CollectionStatus();

        // loan journal
        public void postLoanJournal(Int32 loanId)
        {
            var loanApplications = from d in db.trnLoanApplications
                                   where d.Id == loanId
                                   select d;

            if (loanApplications.Any())
            {
                if (loanApplications.FirstOrDefault().NetAmount > 0)
                {
                    Data.sysJournal newLoanJournal = new Data.sysJournal();
                    newLoanJournal.JournalDate = Convert.ToDateTime(loanApplications.FirstOrDefault().LoanDate);
                    newLoanJournal.AccountId = loanApplications.FirstOrDefault().AccountId;
                    newLoanJournal.Particulars = loanApplications.FirstOrDefault().Particulars;
                    newLoanJournal.ReleasedAmount = loanApplications.FirstOrDefault().NetAmount;
                    newLoanJournal.ReceivedAmount = 0;
                    newLoanJournal.DocumentReference = "Loan - " + loanApplications.FirstOrDefault().LoanNumber;
                    newLoanJournal.LoanId = loanId;
                    newLoanJournal.CollectionId = null;
                    newLoanJournal.ExpenseId = null;

                    db.sysJournals.InsertOnSubmit(newLoanJournal);
                    db.SubmitChanges();
                }
            }
        }

        // delete loan journal
        public void deleteLoanJournal(Int32 loanId)
        {
            var journals = from d in db.sysJournals where d.LoanId == loanId select d;
            if (journals.Any())
            {
                db.sysJournals.DeleteAllOnSubmit(journals);
                db.SubmitChanges();
            }
        }

        // collection journal
        public void postCollectionJournal(Int32 id)
        {
            var dailyCollections = from d in db.trnDailyCollections
                                   where d.Id == id
                                   select d;

            if (dailyCollections.Any())
            {
                if (dailyCollections.FirstOrDefault().PaidAmount > 0)
                {
                    Data.sysJournal newCollectionJournal = new Data.sysJournal();
                    newCollectionJournal.JournalDate = Convert.ToDateTime(dailyCollections.FirstOrDefault().DailyCollectionDate);
                    newCollectionJournal.AccountId = dailyCollections.FirstOrDefault().AccountId;
                    newCollectionJournal.Particulars = "Collection";
                    newCollectionJournal.ReleasedAmount = 0;
                    newCollectionJournal.ReceivedAmount = dailyCollections.FirstOrDefault().PaidAmount;
                    newCollectionJournal.DocumentReference = "Collection - " + dailyCollections.FirstOrDefault().trnCollection.CollectionNumber;
                    newCollectionJournal.LoanId = null;
                    newCollectionJournal.CollectionId = dailyCollections.FirstOrDefault().CollectionId;
                    newCollectionJournal.ExpenseId = null;

                    db.sysJournals.InsertOnSubmit(newCollectionJournal);
                    db.SubmitChanges();
                }
            }
        }

        // delete collection journal
        public void deleteCollectionJournal(Int32 collectionId)
        {
            var journals = from d in db.sysJournals where d.CollectionId == collectionId select d;
            if (journals.Any())
            {
                db.sysJournals.DeleteAllOnSubmit(journals);
                db.SubmitChanges();
            }
        }

        // expenses journal
        public void postExpensesJournal(Int32 expenseId)
        {
            var expenses = from d in db.trnExpenses
                           where d.Id == expenseId
                           select d;

            if (expenses.Any())
            {
                if (expenses.FirstOrDefault().ExpenseAmount > 0)
                {
                    Data.sysJournal newExpenseJournal = new Data.sysJournal();
                    newExpenseJournal.JournalDate = Convert.ToDateTime(expenses.FirstOrDefault().ExpenseDate);
                    newExpenseJournal.AccountId = expenses.FirstOrDefault().AccountId;
                    newExpenseJournal.Particulars = expenses.FirstOrDefault().Particulars;
                    newExpenseJournal.ReleasedAmount = expenses.FirstOrDefault().ExpenseAmount;
                    newExpenseJournal.ReceivedAmount = 0;
                    newExpenseJournal.DocumentReference = "Expenses - " + expenses.FirstOrDefault().ExpenseNumber;
                    newExpenseJournal.LoanId = null;
                    newExpenseJournal.CollectionId = null;
                    newExpenseJournal.ExpenseId = expenseId;

                    db.sysJournals.InsertOnSubmit(newExpenseJournal);
                    db.SubmitChanges();
                }
            }
        }

        // delete expenses journal
        public void deleteExpensesJournal(Int32 expenseId)
        {
            var journals = from d in db.sysJournals where d.ExpenseId == expenseId select d;
            if (journals.Any())
            {
                db.sysJournals.DeleteAllOnSubmit(journals);
                db.SubmitChanges();
            }
        }
    }
}