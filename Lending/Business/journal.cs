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
            var loans = from d in db.trnLoans
                        where d.Id == loanId
                        && d.IsLocked == true
                        select d;

            if (loans.Any())
            {
                if (loans.FirstOrDefault().NetAmount > 0)
                {
                    Data.sysJournal newLoanJournal = new Data.sysJournal();
                    newLoanJournal.JournalDate = Convert.ToDateTime(loans.FirstOrDefault().LoanDate);
                    newLoanJournal.LoanId = loanId;
                    newLoanJournal.CollectionId = null;
                    newLoanJournal.ExpenseId = null;
                    newLoanJournal.ReleasedAmount = loans.FirstOrDefault().NetAmount;
                    newLoanJournal.ReceivedAmount = 0;
                    newLoanJournal.DocumentReference = "Loan - " + loans.FirstOrDefault().LoanNumber;

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
        public void postCollectionJournal(Int32 collectionId)
        {
            var collections = from d in db.trnCollections
                              where d.Id == collectionId
                              && d.IsLocked == true
                              select d;

            if (collections.Any())
            {
                if (collections.FirstOrDefault().PaidAmount > 0)
                {
                    Data.sysJournal newCollectionJournal = new Data.sysJournal();
                    newCollectionJournal.JournalDate = Convert.ToDateTime(collections.FirstOrDefault().CollectionDate);
                    newCollectionJournal.LoanId = null;
                    newCollectionJournal.CollectionId = collections.FirstOrDefault().Id;
                    newCollectionJournal.ExpenseId = null;
                    newCollectionJournal.ReleasedAmount = 0;
                    newCollectionJournal.ReceivedAmount = collections.FirstOrDefault().PaidAmount;
                    newCollectionJournal.DocumentReference = "Collection - " + collections.FirstOrDefault().CollectionNumber;

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
                           && d.IsLocked == true
                           select d;

            if (expenses.Any())
            {
                if (expenses.FirstOrDefault().ExpenseAmount > 0)
                {
                    Data.sysJournal newExpenseJournal = new Data.sysJournal();
                    newExpenseJournal.JournalDate = Convert.ToDateTime(expenses.FirstOrDefault().ExpenseDate);
                    newExpenseJournal.LoanId = null;
                    newExpenseJournal.CollectionId = null;
                    newExpenseJournal.ExpenseId = expenseId;
                    newExpenseJournal.ReleasedAmount = expenses.FirstOrDefault().ExpenseAmount;
                    newExpenseJournal.ReceivedAmount = 0;
                    newExpenseJournal.DocumentReference = "Expenses - " + expenses.FirstOrDefault().ExpenseNumber;

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