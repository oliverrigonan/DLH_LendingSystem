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
                                       Applicant = d.mstApplicant.ApplicantFullName,
                                       AreaId = d.AreaId,
                                       Area = d.mstArea.Area,
                                       Particulars = d.Particulars,
                                       LoanAmount = d.LoanAmount,
                                       PaidAmount = d.PaidAmount,
                                       BalanceAmount = d.BalanceAmount,
                                       CollectorId = d.CollectorId,
                                       Collector = d.mstCollector.Collector,
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

            if (loanApplications.Any())
            {
                foreach (var loanApplication in loanApplications)
                {
                    if (loanApplication.LoanAmount > 0)
                    {
                        Data.trnJournal newLoanJournal = new Data.trnJournal();
                        newLoanJournal.JournalDate = Convert.ToDateTime(loanApplication.LoanDate);
                        newLoanJournal.AccountId = loanApplication.AccountId;
                        newLoanJournal.Particulars = loanApplication.Particulars;
                        newLoanJournal.ReleasedAmount = loanApplication.LoanAmount;
                        newLoanJournal.ReceivedAmount = 0;
                        newLoanJournal.DocumentReference = "Loan - " + loanApplication.LoanNumber;
                        newLoanJournal.LoanId = loanId;
                        newLoanJournal.CollectionId = null;
                        newLoanJournal.DisbursementId = null;

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

        // collection journal
        public void postCollectionJournal(Int32 collectionId)
        {
            var collectionLines = from d in db.trnCollectionLines
                                  where d.CollectionId == collectionId
                                  select new Models.TrnCollectionLines
                                  {
                                      Id = d.Id,
                                      CollectionId = d.CollectionId,
                                      CollectionNumber = d.trnCollection.CollectionNumber,
                                      CollectionDate = d.trnCollection.CollectionDate.ToShortDateString(),
                                      AccountId = d.AccountId,
                                      Account = d.mstAccount.Account,
                                      LoanId = d.LoanId,
                                      LoanNumber = d.trnLoanApplication.LoanNumber,
                                      LoanDate = d.trnLoanApplication.LoanDate.ToShortDateString(),
                                      Particulars = d.Particulars,
                                      Amount = d.Amount,
                                      CollectedByCollectorId = d.CollectedByCollectorId,
                                      CollectedByCollector = d.mstCollector.Collector
                                  };

            if (collectionLines.Any())
            {
                foreach (var collectionLine in collectionLines)
                {
                    if (collectionLine.Amount > 0)
                    {
                        Data.trnJournal newCollectionJournal = new Data.trnJournal();
                        newCollectionJournal.JournalDate = Convert.ToDateTime(collectionLine.CollectionDate);
                        newCollectionJournal.AccountId = collectionLine.AccountId;
                        newCollectionJournal.Particulars = collectionLine.Particulars;
                        newCollectionJournal.ReleasedAmount = 0;
                        newCollectionJournal.ReceivedAmount = collectionLine.Amount;
                        newCollectionJournal.DocumentReference = "Collection - " + collectionLine.CollectionNumber;
                        newCollectionJournal.LoanId = null;
                        newCollectionJournal.CollectionId = collectionId;
                        newCollectionJournal.DisbursementId = null;

                        db.trnJournals.InsertOnSubmit(newCollectionJournal);
                        db.SubmitChanges();
                    }
                }
            }
        }

        // delete collection journal
        public void deleteCollectionJournal(Int32 collectionId)
        {
            var journals = from d in db.trnJournals where d.CollectionId == collectionId select d;
            if (journals.Any())
            {
                db.trnJournals.DeleteAllOnSubmit(journals);
                db.SubmitChanges();
            }
        }

        // disbursement journal
        public void postDisbursementJournal(Int32 disbursementId)
        {
            var disbursements = from d in db.trnDisbursements
                               where d.Id == disbursementId
                               select new Models.TrnDisbursement
                               {
                                   Id = d.Id,
                                   DisbursementNumber = d.DisbursementNumber,
                                   DisbursementDate = d.DisbursementDate.ToShortDateString(),
                                   AccountId = d.AccountId,
                                   Account = d.mstAccount.Account,
                                   Payee = d.Payee,
                                   Particulars = d.Particulars,
                                   Amount = d.Amount,
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

            if (disbursements.Any())
            {
                foreach (var disbursement in disbursements)
                {
                    if (disbursement.Amount > 0)
                    {
                        Data.trnJournal newLoanJournal = new Data.trnJournal();
                        newLoanJournal.JournalDate = Convert.ToDateTime(disbursement.DisbursementDate);
                        newLoanJournal.AccountId = disbursement.AccountId;
                        newLoanJournal.Particulars = disbursement.Particulars;
                        newLoanJournal.ReleasedAmount = disbursement.Amount;
                        newLoanJournal.ReceivedAmount = 0;
                        newLoanJournal.DocumentReference = "Disbursement - " + disbursement.DisbursementNumber;
                        newLoanJournal.LoanId = null;
                        newLoanJournal.CollectionId = null;
                        newLoanJournal.DisbursementId = disbursementId;

                        db.trnJournals.InsertOnSubmit(newLoanJournal);
                        db.SubmitChanges();
                    }
                }
            }
        }

        // delete disbursement journal
        public void deleteDisbursementJournal(Int32 disbursementId)
        {
            var journals = from d in db.trnJournals where d.DisbursementId == disbursementId select d;
            if (journals.Any())
            {
                db.trnJournals.DeleteAllOnSubmit(journals);
                db.SubmitChanges();
            }
        }
    }
}