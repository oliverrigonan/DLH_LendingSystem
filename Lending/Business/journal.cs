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

            if (loanApplications.Any())
            {
                foreach (var loanApplication in loanApplications)
                {
                    if (loanApplication.NetAmount > 0)
                    {
                        Data.sysJournal newLoanJournal = new Data.sysJournal();
                        newLoanJournal.JournalDate = Convert.ToDateTime(loanApplication.LoanDate);
                        newLoanJournal.AccountId = loanApplication.AccountId;
                        newLoanJournal.Particulars = loanApplication.Particulars;
                        newLoanJournal.ReleasedAmount = loanApplication.NetAmount;
                        newLoanJournal.ReceivedAmount = 0;
                        newLoanJournal.DocumentReference = "Loan - " + loanApplication.LoanNumber;
                        newLoanJournal.LoanId = loanId;
                        newLoanJournal.CollectionId = null;
                        newLoanJournal.ExpenseId = null;

                        db.sysJournals.InsertOnSubmit(newLoanJournal);
                        db.SubmitChanges();
                    }
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
            var dailyCollections = from d in db.trnDailyCollections
                                   where d.CollectionId == Convert.ToInt32(collectionId)
                                   select new Models.TrnDailyCollection
                                   {
                                       Id = d.Id,
                                       CollectionId = d.CollectionId,
                                       CollectionNumber = d.trnCollection.CollectionNumber,
                                       AccountId = d.AccountId,
                                       Applicant = d.trnCollection.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnCollection.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + (d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName != null ? d.trnCollection.trnLoanApplication.mstApplicant.ApplicantMiddleName : " "),
                                       DailyCollectionDate = d.DailyCollectionDate.ToShortDateString(),
                                       NetAmount = d.NetAmount,
                                       CollectibleAmount = d.CollectibleAmount,
                                       PenaltyAmount = d.PenaltyAmount,
                                       PaidAmount = d.PaidAmount,
                                       PreviousBalanceAmount = d.PreviousBalanceAmount,
                                       CurrentBalanceAmount = d.CurrentBalanceAmount,
                                       IsCurrentCollection = d.IsCurrentCollection,
                                       IsCleared = d.IsCleared,
                                       IsAbsent = d.IsAbsent,
                                       IsPartiallyPaid = d.IsPartiallyPaid,
                                       IsPaidInAdvanced = d.IsPaidInAdvanced,
                                       IsFullyPaid = d.IsFullyPaid,
                                       IsProcessed = d.IsProcessed,
                                       CanPerformAction = d.CanPerformAction,
                                       IsDueDate = d.IsDueDate,
                                       IsAllowanceDay = d.IsAllowanceDay,
                                       IsLastDay = d.IsLastDay,
                                       ReconstructId = d.ReconstructId != null ? d.ReconstructId : 0,
                                       IsReconstructed = d.IsReconstructed,
                                       Status = collectionStatus.getStatus(d.IsCleared, d.IsAbsent, d.IsPartiallyPaid, d.IsPaidInAdvanced, d.IsFullyPaid, d.trnCollection.IsOverdue)
                                   };


            if (dailyCollections.Any())
            {
                foreach (var dailyCollection in dailyCollections)
                {
                    if (dailyCollection.PaidAmount > 0)
                    {
                        Data.sysJournal newCollectionJournal = new Data.sysJournal();
                        newCollectionJournal.JournalDate = Convert.ToDateTime(dailyCollection.DailyCollectionDate);
                        newCollectionJournal.AccountId = dailyCollection.AccountId;
                        newCollectionJournal.Particulars = "Collection";
                        newCollectionJournal.ReleasedAmount = 0;
                        newCollectionJournal.ReceivedAmount = dailyCollection.PaidAmount;
                        newCollectionJournal.DocumentReference = "Collection - " + dailyCollection.CollectionNumber;
                        newCollectionJournal.LoanId = null;
                        newCollectionJournal.CollectionId = collectionId;
                        newCollectionJournal.ExpenseId = null;

                        db.sysJournals.InsertOnSubmit(newCollectionJournal);
                        db.SubmitChanges();
                    }
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
                           select new Models.TrnExpenses
                           {
                               Id = d.Id,
                               ExpenseNumber = d.ExpenseNumber,
                               ExpenseDate = d.ExpenseDate.ToShortDateString(),
                               AccountId = d.AccountId,
                               Account = d.mstAccount.Account,
                               CollectorStaffId = d.CollectorStaffId,
                               CollectorStaff = d.mstStaff.Staff,
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
                        Data.sysJournal newExpenseJournal = new Data.sysJournal();
                        newExpenseJournal.JournalDate = Convert.ToDateTime(expense.ExpenseDate);
                        newExpenseJournal.AccountId = expense.AccountId;
                        newExpenseJournal.Particulars = expense.Particulars;
                        newExpenseJournal.ReleasedAmount = expense.ExpenseAmount;
                        newExpenseJournal.ReceivedAmount = 0;
                        newExpenseJournal.DocumentReference = "Expenses - " + expense.ExpenseNumber;
                        newExpenseJournal.LoanId = null;
                        newExpenseJournal.CollectionId = null;
                        newExpenseJournal.ExpenseId = expenseId;

                        db.sysJournals.InsertOnSubmit(newExpenseJournal);
                        db.SubmitChanges();
                    }
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