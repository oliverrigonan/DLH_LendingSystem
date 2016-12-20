using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lending.Business
{
    public class Collection
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // loan collection
        public void postCollection(Int32 collectionId)
        {
            var collection = from d in db.trnCollections
                             where d.Id == Convert.ToInt32(collectionId)
                             select new Models.TrnCollection
                             {
                                 Id = d.Id,
                                 CollectionNumber = d.CollectionNumber,
                                 CollectionDate = d.CollectionDate.ToShortDateString(),
                                 LoanId = d.LoanId,
                                 LoanNumber = d.trnLoanApplication.LoanNumber + " - from " + d.trnLoanApplication.LoanDate + " to " + d.trnLoanApplication.MaturityDate,
                                 NetAmount = d.trnLoanApplication.NetAmount,
                                 Applicant = d.trnLoanApplication.mstApplicant.ApplicantLastName + ", " + d.trnLoanApplication.mstApplicant.ApplicantFirstName + " " + (d.trnLoanApplication.mstApplicant.ApplicantMiddleName != null ? d.trnLoanApplication.mstApplicant.ApplicantMiddleName : " "),
                                 Area = d.trnLoanApplication.mstApplicant.mstArea.Area,
                                 TermId = d.TermId,
                                 TermNoOfDays = d.TermNoOfAllowanceDays,
                                 TermNoOfAllowanceDays = d.TermNoOfAllowanceDays,
                                 IsFullyPaid = d.IsFullyPaid,
                                 IsOverdue = d.IsOverdue,
                                 IsLocked = d.IsLocked,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.mstUser.FullName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.mstUser1.FullName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            if (collection.Any())
            {
                if (collection.FirstOrDefault().NetAmount > 0)
                {
                    var numberOfDays = (Convert.ToDateTime(loanApplication.FirstOrDefault().MaturityDate) - Convert.ToDateTime(loanApplication.FirstOrDefault().LoanDate)).TotalDays;

                    Decimal netAmount = loanApplication.FirstOrDefault().NetAmount;

                    Decimal collectibleAmount = Math.Round(loanApplication.FirstOrDefault().NetAmount / Convert.ToDecimal(numberOfDays), 1);
                    Decimal collectibleAmountCeil = Math.Ceiling((collectibleAmount + 1) / 5) * 5;

                    for (var i = 1; i <= numberOfDays; i++)
                    {
                        if (i % loanApplication.FirstOrDefault().TermNoOfDays == 0)
                        {
                            if (netAmount > (collectibleAmountCeil * loanApplication.FirstOrDefault().TermNoOfDays))
                            {
                                Boolean isActionValue = false, isDueDateValue = false, isCurrentCollectionValue = false, isLastDay = false;

                                if (i == loanApplication.FirstOrDefault().TermNoOfDays)
                                {
                                    isActionValue = true;
                                    isCurrentCollectionValue = true;
                                }

                                if (i == numberOfDays)
                                {
                                    isDueDateValue = true;
                                    isLastDay = true;
                                }

                                Data.trnCollection newCollection = new Data.trnCollection();
                                newCollection.LoanId = loanId;
                                newCollection.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
                                newCollection.CollectionDate = Convert.ToDateTime(loanApplication.FirstOrDefault().LoanDate).Date.AddDays(i);
                                newCollection.NetAmount = loanApplication.FirstOrDefault().NetAmount;
                                newCollection.CollectibleAmount = collectibleAmountCeil * loanApplication.FirstOrDefault().TermNoOfDays;
                                newCollection.PenaltyAmount = 0;
                                newCollection.PaidAmount = 0;
                                newCollection.PreviousBalanceAmount = 0;
                                newCollection.CurrentBalanceAmount = collectibleAmountCeil * loanApplication.FirstOrDefault().TermNoOfDays;
                                newCollection.IsCleared = false;
                                newCollection.IsAbsent = false;
                                newCollection.IsPartialPayment = false;
                                newCollection.IsAdvancePayment = false;
                                newCollection.IsFullPayment = false;
                                newCollection.IsDueDate = isDueDateValue;
                                newCollection.IsExtendCollection = false;
                                newCollection.IsOverdueCollection = false;
                                newCollection.IsCurrentCollection = isCurrentCollectionValue;
                                newCollection.IsProcessed = false;
                                newCollection.IsAction = isActionValue;
                                newCollection.IsLastDay = isLastDay;
                                db.trnCollections.InsertOnSubmit(newCollection);
                                db.SubmitChanges();

                                netAmount -= (collectibleAmountCeil * loanApplication.FirstOrDefault().TermNoOfDays);
                            }
                            else
                            {
                                Boolean isActionValue = false, isDueDateValue = false, isCurrentCollectionValue = false, isLastDay = false;

                                if (i == 1)
                                {
                                    isActionValue = true;
                                    isCurrentCollectionValue = true;
                                }

                                if (i == numberOfDays)
                                {
                                    isDueDateValue = true;
                                    isLastDay = true;
                                }

                                Data.trnCollection newCollection = new Data.trnCollection();
                                newCollection.LoanId = loanId;
                                newCollection.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
                                newCollection.CollectionDate = Convert.ToDateTime(loanApplication.FirstOrDefault().LoanDate).Date.AddDays(i);
                                newCollection.NetAmount = loanApplication.FirstOrDefault().NetAmount;
                                newCollection.CollectibleAmount = netAmount;
                                newCollection.PenaltyAmount = 0;
                                newCollection.PaidAmount = 0;
                                newCollection.PreviousBalanceAmount = 0;
                                newCollection.CurrentBalanceAmount = netAmount;
                                newCollection.IsCleared = false;
                                newCollection.IsAbsent = false;
                                newCollection.IsPartialPayment = false;
                                newCollection.IsAdvancePayment = false;
                                newCollection.IsFullPayment = false;
                                newCollection.IsDueDate = isDueDateValue;
                                newCollection.IsExtendCollection = false;
                                newCollection.IsOverdueCollection = false;
                                newCollection.IsCurrentCollection = isCurrentCollectionValue;
                                newCollection.IsProcessed = false;
                                newCollection.IsAction = isActionValue;
                                newCollection.IsLastDay = isLastDay;
                                db.trnCollections.InsertOnSubmit(newCollection);
                                db.SubmitChanges();

                                netAmount *= 0;
                            }
                        }
                        else
                        {
                            if (i == numberOfDays)
                            {
                                Boolean isActionValue = false, isDueDateValue = false, isCurrentCollectionValue = false, isLastDay = false;

                                if (i == 1)
                                {
                                    isActionValue = true;
                                    isCurrentCollectionValue = true;
                                }

                                isDueDateValue = true;
                                isLastDay = true;

                                Data.trnCollection newCollection = new Data.trnCollection();
                                newCollection.LoanId = loanId;
                                newCollection.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
                                newCollection.CollectionDate = Convert.ToDateTime(loanApplication.FirstOrDefault().LoanDate).Date.AddDays(i);
                                newCollection.NetAmount = loanApplication.FirstOrDefault().NetAmount;
                                newCollection.CollectibleAmount = netAmount;
                                newCollection.PenaltyAmount = 0;
                                newCollection.PaidAmount = 0;
                                newCollection.PreviousBalanceAmount = 0;
                                newCollection.CurrentBalanceAmount = netAmount;
                                newCollection.IsCleared = false;
                                newCollection.IsAbsent = false;
                                newCollection.IsPartialPayment = false;
                                newCollection.IsAdvancePayment = false;
                                newCollection.IsFullPayment = false;
                                newCollection.IsDueDate = isDueDateValue;
                                newCollection.IsExtendCollection = false;
                                newCollection.IsOverdueCollection = false;
                                newCollection.IsCurrentCollection = isCurrentCollectionValue;
                                newCollection.IsProcessed = false;
                                newCollection.IsAction = isActionValue;
                                newCollection.IsLastDay = isLastDay;
                                db.trnCollections.InsertOnSubmit(newCollection);
                                db.SubmitChanges();

                                netAmount *= 0;
                            }
                        }
                    }
                }
            }
        }

        // delete collection
        public void deleteCollection(Int32 loanId)
        {
            var collections = from d in db.trnCollections where d.LoanId == loanId select d;
            if (collections.Any())
            {
                db.trnCollections.DeleteAllOnSubmit(collections);
                db.SubmitChanges();
            }
        }
    }
}