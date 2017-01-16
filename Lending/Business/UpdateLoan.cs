using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lending.Business
{
    public class UpdateLoan
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // update loan
        public void updateLoan(Int32 loanId)
        {
            var loan = from d in db.trnLoans where d.Id == loanId select d;
            if (loan.Any())
            {
                Decimal principalAmount = loan.FirstOrDefault().PrincipalAmount;
                Decimal interestAmount = loan.FirstOrDefault().InterestAmount;
                Decimal deductionAmount = loan.FirstOrDefault().DeductionAmount;
                Decimal netAmount = loan.FirstOrDefault().NetAmount;

                var collectionLines = from d in db.trnCollectionLines
                                      where d.trnCollection.LoanId == loanId
                                      && d.trnCollection.IsLocked == true
                                      select d;

                Decimal totalPaidAmount = 0;
                Decimal totalPenaltyAmount = 0;

                if (collectionLines.Any())
                {
                    totalPaidAmount = collectionLines.Sum(d => d.PaidAmount);
                    totalPenaltyAmount = collectionLines.Sum(d => d.PenaltyAmount);
                }

                Decimal totalBalanceAmount = (((principalAmount + interestAmount) - deductionAmount) + totalPenaltyAmount) - totalPaidAmount;
                if (loan.FirstOrDefault().IsAdvanceInterest)
                {
                    totalBalanceAmount = ((principalAmount - interestAmount - deductionAmount) + totalPenaltyAmount) - totalPaidAmount;
                }

                Decimal noOfAbsent = 0;
                var collectionLinesNoOfAbsent = from d in db.trnCollectionLines
                                                where d.trnCollection.LoanId == loanId
                                                && d.PaidAmount == 0
                                                && d.trnCollection.IsLocked == true
                                                select d;

                if (collectionLinesNoOfAbsent.Any())
                {
                    noOfAbsent = collectionLinesNoOfAbsent.Count();
                }

                var updateLoan = loan.FirstOrDefault();
                updateLoan.TotalPaidAmount = totalPaidAmount;
                updateLoan.TotalPenaltyAmount = totalPenaltyAmount;
                updateLoan.TotalBalanceAmount = totalBalanceAmount;
                updateLoan.NoOfAbsent = noOfAbsent;
                db.SubmitChanges();
            }
        }

        // update loan lines
        public void updateLoanLines(Int32 loanId)
        {
            var collectionLines = from d in db.trnCollectionLines
                                  where d.trnCollection.LoanId == loanId
                                  && d.trnCollection.IsLocked == true
                                  select new Models.TrnCollectionLines
                                  {
                                      Id = d.Id,
                                      LoanLinesId = d.LoanLinesId,
                                      PaidAmount = d.PaidAmount,
                                      PenaltyAmount = d.PenaltyAmount,
                                      BalanceAmount = d.BalanceAmount
                                  };

            if (collectionLines.Any())
            {
                foreach (var collectionLine in collectionLines)
                {
                    var loanLine = from d in db.trnLoanLines
                                   where d.Id == collectionLine.LoanLinesId
                                   select d;

                    if (loanLine.Any())
                    {
                        var updateLoanLines = loanLine.FirstOrDefault();
                        updateLoanLines.PaidAmount = collectionLine.PaidAmount;
                        updateLoanLines.PenaltyAmount = collectionLine.PenaltyAmount;
                        updateLoanLines.BalanceAmount = collectionLine.BalanceAmount;

                        Boolean isCleared = false;
                        if (collectionLine.BalanceAmount == 0)
                        {
                            isCleared = true;
                        }

                        updateLoanLines.IsCleared = isCleared;
                        db.SubmitChanges();
                    }
                }

                this.updateLoan(loanId);
            }
        }
    }
}