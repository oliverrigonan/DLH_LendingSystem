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
            var loans = from d in db.trnLoans where d.Id == loanId select d;
            if (loans.Any())
            {
                Decimal loanNetAmount = loans.FirstOrDefault().NetAmount;

                // get collection
                var collections = from d in db.trnCollections
                                  where d.LoanId == loanId
                                  && d.IsLocked == true
                                  select d;

                Decimal loanPaidAmount = 0;
                Decimal loanPenaltyAmount = 0;
                if (collections.Any())
                {
                    loanPaidAmount = collections.Sum(d => d.PaidAmount);
                    loanPenaltyAmount = collections.Sum(d => d.PenaltyAmount);
                }

                // get reconstruct
                var reconstruct = from d in db.trnReconstructs
                                  where d.LoanId == loanId
                                  && d.IsLocked == true
                                  select d;

                Decimal loanReconstructInterestAmount = 0;
                Boolean isReconstruct = false;
                if (reconstruct.Any())
                {
                    loanReconstructInterestAmount = reconstruct.Sum(d => d.ReconstructAmount);
                    isReconstruct = true;
                }

                // total all for loan balance amount
                Decimal loanBalanceAmount = (loanNetAmount + loanReconstructInterestAmount + loanPenaltyAmount) - loanPaidAmount;

                Boolean isFullyPaid = false;
                if (loanBalanceAmount == 0)
                {
                    isFullyPaid = true;
                }

                // get collection number of absent
                var collectionNumberOfAbsent = from d in db.trnCollections
                                               where d.LoanId == loanId
                                               && d.IsAbsent == false
                                               && d.IsLocked == true
                                               select d;

                Decimal numberOfAbsent = 0;
                if (collectionNumberOfAbsent.Any())
                {
                    numberOfAbsent = collectionNumberOfAbsent.Count();
                }

                // get collection for next paid date
                var collectionNextPaidDate = from d in db.trnCollections.OrderByDescending(d => d.Id)
                                             where d.LoanId == loanId
                                             && d.IsLocked == true
                                             select d;

                var teamNoOfDays = loans.FirstOrDefault().TermNoOfDays;
                DateTime nextPaidDate = loans.FirstOrDefault().LoanDate.AddDays(Convert.ToDouble(teamNoOfDays));
                if (collectionNextPaidDate.Any())
                {
                    nextPaidDate = collectionNextPaidDate.FirstOrDefault().CollectionDate.AddDays(Convert.ToDouble(teamNoOfDays));
                }

                // get reconstruct last maturity date
                var reconstructLastMaturityDate = from d in db.trnReconstructs.OrderByDescending(d => d.Id)
                                                  where d.LoanId == loanId
                                                  && d.IsLocked == true
                                                  select d;

                DateTime loanEndDate = loans.FirstOrDefault().LoanEndDate;
                if (reconstructLastMaturityDate.Any())
                {
                    loanEndDate = reconstructLastMaturityDate.FirstOrDefault().MaturityDate;
                }

                // update loan
                var updateLoan = loans.FirstOrDefault();
                updateLoan.LoanEndDate = loanEndDate;
                updateLoan.NoOfAbsent = numberOfAbsent;
                updateLoan.PaidAmount = loanPaidAmount;
                updateLoan.NextPaidDate = nextPaidDate;
                updateLoan.PenaltyAmount = loanPenaltyAmount;
                updateLoan.IsReconstruct = isReconstruct;
                updateLoan.ReconstructInterestAmount = loanReconstructInterestAmount;
                updateLoan.BalanceAmount = loanBalanceAmount;
                updateLoan.IsFullyPaid = isFullyPaid;
                db.SubmitChanges();
            }
        }
    }
}