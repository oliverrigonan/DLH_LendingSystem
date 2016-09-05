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
                    if (loanApplication.LoanAmount > 0)
                    {
                        Data.trnJournal newLoanJournal = new Data.trnJournal();
                        newLoanJournal.JournalDate = Convert.ToDateTime(loanApplication.LoanDate);
                        newLoanJournal.BranchId = loanApplication.BranchId;
                        newLoanJournal.AccountId = loanApplication.AccountId;
                        newLoanJournal.Particulars = "NA";
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
            var collections = from d in db.trnCollections
                              where d.Id == collectionId
                              select new Models.TrnCollection
                              {
                                  Id = d.Id,
                                  CollectionNumber = d.CollectionNumber,
                                  CollectionDate = d.CollectionDate.ToShortDateString(),
                                  BranchId = d.BranchId,
                                  Branch = d.mstBranch.Branch,
                                  AccountId = d.AccountId,
                                  Account = d.mstAccount.Account,
                                  ApplicantId = d.ApplicantId,
                                  Applicant = d.mstApplicant.ApplicantFullName,
                                  Particulars = d.Particulars,
                                  PaidAmount = d.PaidAmount,
                                  PreparedByUserId = d.PreparedByUserId,
                                  PreparedByUser = d.mstUser2.FullName,
                                  VerifiedByUserId = d.VerifiedByUserId,
                                  VerifiedByUser = d.mstUser3.FullName,
                                  IsLocked = d.IsLocked,
                                  CreatedByUserId = d.CreatedByUserId,
                                  CreatedByUser = d.mstUser.FullName,
                                  CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                  UpdatedByUserId = d.UpdatedByUserId,
                                  UpdatedByUser = d.mstUser1.FullName,
                                  UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                              };

            if (collections.Any())
            {
                foreach (var collection in collections)
                {
                    if (collection.PaidAmount > 0)
                    {
                        Data.trnJournal newCollectionJournal = new Data.trnJournal();
                        newCollectionJournal.JournalDate = Convert.ToDateTime(collection.CollectionDate);
                        newCollectionJournal.BranchId = collection.BranchId;
                        newCollectionJournal.AccountId = collection.AccountId;
                        newCollectionJournal.Particulars = "NA";
                        newCollectionJournal.ReleasedAmount = 0;
                        newCollectionJournal.ReceivedAmount = collection.PaidAmount;
                        newCollectionJournal.DocumentReference = "Collection - " + collection.CollectionNumber;
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
    }
}