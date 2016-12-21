using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace Lending.Business
{
    public class Collection
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

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

        // loan collection
        public void postCollection(Int32 loanId, Int32 userId)
        {
            var loanApplication = from d in db.trnLoanApplications where d.Id == loanId select d;
            if (loanApplication.Any())
            {
                String collectionNumber = "0000000001";
                var previousCollection = from d in db.trnCollections.OrderByDescending(d => d.Id) select d;
                if (previousCollection.Any())
                {
                    var newCollectionNumber = Convert.ToInt32(previousCollection.FirstOrDefault().CollectionNumber) + 0000000001;
                    collectionNumber = newCollectionNumber.ToString();
                }

                Data.trnCollection newCollection = new Data.trnCollection();
                newCollection.CollectionNumber = zeroFill(Convert.ToInt32(collectionNumber), 10);
                newCollection.CollectionDate = DateTime.Today;
                newCollection.LoanId = loanId;
                newCollection.TermId = loanApplication.FirstOrDefault().TermId;
                newCollection.TermNoOfDays = loanApplication.FirstOrDefault().mstTerm.NoOfDays;
                newCollection.TermNoOfAllowanceDays = loanApplication.FirstOrDefault().mstTerm.NoOfAllowanceDays;
                newCollection.IsFullyPaid = false;
                newCollection.IsOverdue = false;
                newCollection.IsLocked = true;
                newCollection.CreatedByUserId = userId;
                newCollection.CreatedDateTime = DateTime.Now;
                newCollection.UpdatedByUserId = userId;
                newCollection.UpdatedDateTime = DateTime.Now;
                db.trnCollections.InsertOnSubmit(newCollection);
                db.SubmitChanges();

                var collection = from d in db.trnCollections
                                 where d.Id == newCollection.Id
                                 select d;

                if (collection.Any())
                {
                    if (loanApplication.FirstOrDefault().NetAmount > 0)
                    {
                        var numberOfDays = (Convert.ToDateTime(loanApplication.FirstOrDefault().MaturityDate) - Convert.ToDateTime(loanApplication.FirstOrDefault().LoanDate)).TotalDays;

                        Decimal netAmount = loanApplication.FirstOrDefault().NetAmount;
                        Decimal collectibleAmount = Math.Round(loanApplication.FirstOrDefault().NetAmount / Convert.ToDecimal(numberOfDays), 1);
                        Decimal collectibleAmountCeil = Math.Ceiling((collectibleAmount + 1) / 5) * 5;

                        for (var i = 1; i <= numberOfDays; i++)
                        {
                            if (i % collection.FirstOrDefault().TermNoOfDays == 0)
                            {
                                if (netAmount > (collectibleAmountCeil * collection.FirstOrDefault().TermNoOfDays))
                                {
                                    Boolean isActionValue = false, isDueDateValue = false, isCurrentCollectionValue = false, isLastDay = false;

                                    if (i == collection.FirstOrDefault().TermNoOfDays)
                                    {
                                        isActionValue = true;
                                        isCurrentCollectionValue = true;
                                    }

                                    if (i == numberOfDays)
                                    {
                                        isDueDateValue = true;
                                        isLastDay = true;
                                    }

                                    Data.trnDailyCollection newDailyCollection = new Data.trnDailyCollection();
                                    newDailyCollection.CollectionId = newCollection.Id;
                                    newDailyCollection.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
                                    newDailyCollection.DailyCollectionDate = Convert.ToDateTime(loanApplication.FirstOrDefault().LoanDate).Date.AddDays(i);
                                    newDailyCollection.NetAmount = loanApplication.FirstOrDefault().NetAmount;
                                    newDailyCollection.CollectibleAmount = collectibleAmountCeil * collection.FirstOrDefault().TermNoOfDays;
                                    newDailyCollection.PenaltyAmount = 0;
                                    newDailyCollection.PaidAmount = 0;
                                    newDailyCollection.PreviousBalanceAmount = 0;
                                    newDailyCollection.CurrentBalanceAmount = collectibleAmountCeil * collection.FirstOrDefault().TermNoOfDays;
                                    newDailyCollection.IsCurrentCollection = isCurrentCollectionValue;
                                    newDailyCollection.IsCleared = false;
                                    newDailyCollection.IsAbsent = false;
                                    newDailyCollection.IsPartiallyPaid = false;
                                    newDailyCollection.IsPaidInAdvanced = false;
                                    newDailyCollection.IsFullyPaid = false;
                                    newDailyCollection.IsDueDate = isDueDateValue;
                                    newDailyCollection.IsProcessed = false;
                                    newDailyCollection.CanPerformAction = isActionValue;
                                    newDailyCollection.IsLastDay = isLastDay;
                                    db.trnDailyCollections.InsertOnSubmit(newDailyCollection);
                                    db.SubmitChanges();

                                    netAmount -= (collectibleAmountCeil * collection.FirstOrDefault().TermNoOfDays);
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

                                    Data.trnDailyCollection newDailyCollection = new Data.trnDailyCollection();
                                    newDailyCollection.CollectionId = newCollection.Id;
                                    newDailyCollection.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
                                    newDailyCollection.DailyCollectionDate = Convert.ToDateTime(loanApplication.FirstOrDefault().LoanDate).Date.AddDays(i);
                                    newDailyCollection.NetAmount = loanApplication.FirstOrDefault().NetAmount;
                                    newDailyCollection.CollectibleAmount = netAmount;
                                    newDailyCollection.PenaltyAmount = 0;
                                    newDailyCollection.PaidAmount = 0;
                                    newDailyCollection.PreviousBalanceAmount = 0;
                                    newDailyCollection.CurrentBalanceAmount = netAmount;
                                    newDailyCollection.IsCurrentCollection = isCurrentCollectionValue;
                                    newDailyCollection.IsCleared = false;
                                    newDailyCollection.IsAbsent = false;
                                    newDailyCollection.IsPartiallyPaid = false;
                                    newDailyCollection.IsPaidInAdvanced = false;
                                    newDailyCollection.IsFullyPaid = false;
                                    newDailyCollection.IsDueDate = isDueDateValue;
                                    newDailyCollection.IsProcessed = false;
                                    newDailyCollection.CanPerformAction = isActionValue;
                                    newDailyCollection.IsLastDay = isLastDay;
                                    db.trnDailyCollections.InsertOnSubmit(newDailyCollection);
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

                                    Data.trnDailyCollection newDailyCollection = new Data.trnDailyCollection();
                                    newDailyCollection.CollectionId = newCollection.Id;
                                    newDailyCollection.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 2 select d.Id).FirstOrDefault();
                                    newDailyCollection.DailyCollectionDate = Convert.ToDateTime(loanApplication.FirstOrDefault().LoanDate).Date.AddDays(i);
                                    newDailyCollection.NetAmount = loanApplication.FirstOrDefault().NetAmount;
                                    newDailyCollection.CollectibleAmount = netAmount;
                                    newDailyCollection.PenaltyAmount = 0;
                                    newDailyCollection.PaidAmount = 0;
                                    newDailyCollection.PreviousBalanceAmount = 0;
                                    newDailyCollection.CurrentBalanceAmount = netAmount;
                                    newDailyCollection.IsCurrentCollection = isCurrentCollectionValue;
                                    newDailyCollection.IsCleared = false;
                                    newDailyCollection.IsAbsent = false;
                                    newDailyCollection.IsPartiallyPaid = false;
                                    newDailyCollection.IsPaidInAdvanced = false;
                                    newDailyCollection.IsFullyPaid = false;
                                    newDailyCollection.IsDueDate = isDueDateValue;
                                    newDailyCollection.IsProcessed = false;
                                    newDailyCollection.CanPerformAction = isActionValue;
                                    newDailyCollection.IsLastDay = isLastDay;
                                    db.trnDailyCollections.InsertOnSubmit(newDailyCollection);
                                    db.SubmitChanges();

                                    netAmount *= 0;
                                }
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