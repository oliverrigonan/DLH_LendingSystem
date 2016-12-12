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
        public void postCollection(Int32 loanId)
        {
            var loanApplication = from d in db.trnLoanApplications
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
                                       Area = d.mstApplicant.mstArea.Area,
                                       Particulars = d.Particulars,
                                       LoanTypeId = d.LoanTypeId,
                                       LoanType = d.mstLoanType.LoanType,
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
                                       IsFullyPaid = d.IsFullyPaid,
                                       TermId = d.TermId,
                                       Term = d.mstTerm.Term,
                                       TermNoOfDays = d.mstTerm.NoOfDays,
                                       InterestId = d.InterestId,
                                       Interest = d.mstInterest.Interest,
                                       PenaltyId = d.PenaltyId,
                                       Penalty = d.mstPenalty.Penalty,
                                       IsLocked = d.IsLocked,
                                       CreatedByUserId = d.CreatedByUserId,
                                       CreatedByUser = d.mstUser1.FullName,
                                       CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                       UpdatedByUserId = d.UpdatedByUserId,
                                       UpdatedByUser = d.mstUser2.FullName,
                                       UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                                   };

            if (loanApplication.Any())
            {
                if (loanApplication.FirstOrDefault().NetAmount > 0)
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