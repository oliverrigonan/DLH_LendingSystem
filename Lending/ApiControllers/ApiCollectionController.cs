using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Diagnostics;

namespace Lending.ApiControllers
{
    public class ApiCollectionController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // collection list by collectionDate
        [Authorize]
        [HttpGet]
        [Route("api/collections/list/ByCollectionDate/{collectionDate}")]
        public List<Models.TrnCollection> listCollectionByCollectionDate(String collectionDate)
        {
            var collections = from d in db.trnCollections
                              where d.CollectionDate == Convert.ToDateTime(collectionDate)
                              select new Models.TrnCollection
                              {
                                  Id = d.Id,
                                  CollectionNumber = d.CollectionNumber,
                                  CollectionDate = d.CollectionDate.ToShortDateString(),
                                  ApplicantId = d.ApplicantId,
                                  Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                  StatusId = d.StatusId,
                                  Status = d.sysCollectionStatus.Status,
                                  Particulars = d.Particulars,
                                  TotalPaidAmount = d.TotalPaidAmount,
                                  PreparedByUserId = d.PreparedByUserId,
                                  PreparedByUser = d.mstUser.FullName,
                                  IsLocked = d.IsLocked,
                                  CreatedByUserId = d.CreatedByUserId,
                                  CreatedByUser = d.mstUser.FullName,
                                  CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                  UpdatedByUserId = d.UpdatedByUserId,
                                  UpdatedByUser = d.mstUser1.FullName,
                                  UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                              };

            return collections.ToList();
        }

        // collection get by id
        [Authorize]
        [HttpGet]
        [Route("api/collection/getById/{id}")]
        public Models.TrnCollection getCollectionById(String id)
        {
            var collection = from d in db.trnCollections
                             where d.Id == Convert.ToInt32(id)
                             select new Models.TrnCollection
                             {
                                 Id = d.Id,
                                 CollectionNumber = d.CollectionNumber,
                                 CollectionDate = d.CollectionDate.ToShortDateString(),
                                 ApplicantId = d.ApplicantId,
                                 Applicant = d.mstApplicant.ApplicantLastName + ", " + d.mstApplicant.ApplicantFirstName + " " + (d.mstApplicant.ApplicantMiddleName != null ? d.mstApplicant.ApplicantMiddleName : " "),
                                 StatusId = d.StatusId,
                                 Status = d.sysCollectionStatus.Status,
                                 Particulars = d.Particulars,
                                 TotalPaidAmount = d.TotalPaidAmount,
                                 PreparedByUserId = d.PreparedByUserId,
                                 PreparedByUser = d.mstUser.FullName,
                                 IsLocked = d.IsLocked,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.mstUser.FullName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.mstUser1.FullName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            return (Models.TrnCollection)collection.FirstOrDefault();
        }

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

        // add collection
        [Authorize]
        [HttpPost]
        [Route("api/collection/add")]
        public Int32 addCollection()
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                var mstUserForms = from d in db.mstUserForms
                                   where d.UserId == userId
                                   select new Models.MstUserForm
                                   {
                                       Id = d.Id,
                                       Form = d.sysForm.Form,
                                       CanPerformActions = d.CanPerformActions
                                   };

                if (mstUserForms.Any())
                {
                    String matchPageString = "CollectionList";
                    Boolean canPerformActions = false;

                    foreach (var mstUserForm in mstUserForms)
                    {
                        if (mstUserForm.Form.Equals(matchPageString))
                        {
                            if (mstUserForm.CanPerformActions)
                            {
                                canPerformActions = true;
                            }

                            break;
                        }
                    }

                    if (canPerformActions)
                    {
                        String collectionNumber = "0000000001";
                        var collection = from d in db.trnCollections.OrderByDescending(d => d.Id) select d;
                        if (collection.Any())
                        {
                            var newCollectionNumber = Convert.ToInt32(collection.FirstOrDefault().CollectionNumber) + 0000000001;
                            collectionNumber = newCollectionNumber.ToString();
                        }

                        var applicant = from d in db.mstApplicants select d;
                        if (applicant.Any())
                        {
                            var status = from d in db.sysCollectionStatus select d;
                            if (status.Any())
                            {
                                Data.trnCollection newCollection = new Data.trnCollection();
                                newCollection.CollectionNumber = zeroFill(Convert.ToInt32(collectionNumber), 10);
                                newCollection.CollectionDate = DateTime.Today;
                                newCollection.ApplicantId = applicant.FirstOrDefault().Id;
                                newCollection.StatusId = status.FirstOrDefault().Id;
                                newCollection.Particulars = "NA";
                                newCollection.PreparedByUserId = userId;
                                newCollection.TotalPaidAmount = 0;
                                newCollection.IsLocked = false;
                                newCollection.CreatedByUserId = userId;
                                newCollection.CreatedDateTime = DateTime.Now;
                                newCollection.UpdatedByUserId = userId;
                                newCollection.UpdatedDateTime = DateTime.Now;
                                db.trnCollections.InsertOnSubmit(newCollection);
                                db.SubmitChanges();

                                return newCollection.Id;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        // update loan lines
        public void updateLoan(Int32 collectionId)
        {
            var lockedCollectionLines = from d in db.trnCollectionLines
                                        where d.CollectionId == collectionId
                                        && d.trnCollection.IsLocked == true
                                        select new Models.TrnCollectionLines
                                        {
                                            Id = d.Id,
                                            CollectionId = d.CollectionId,
                                            LoanId = d.LoanId,
                                            LoanNumber = d.trnLoan.LoanNumber,
                                            LoanLinesId = d.LoanLinesId,
                                            LoanLinesDayReference = d.trnLoanLine.DayReference,
                                            LoanLinesCollectibleDate = d.trnLoanLine.CollectibleDate.ToShortDateString(),
                                            PenaltyId = d.PenaltyId,
                                            Penalty = d.mstPenalty.Penalty,
                                            PenaltyAmount = d.PenaltyAmount,
                                            PaidAmount = d.PaidAmount,
                                        };

            if (lockedCollectionLines.Any())
            {
                foreach (var collectionLine in lockedCollectionLines)
                {
                    var loanLines = from d in db.trnLoanLines
                                    where d.Id == collectionLine.LoanLinesId
                                    select d;

                    if (loanLines.Any())
                    {
                        var collectionPaidAmount = loanLines.FirstOrDefault().PaidAmount + collectionLine.PaidAmount;
                        var collectionPenaltyAmount = loanLines.FirstOrDefault().PenaltyAmount + collectionLine.PenaltyAmount;
                        var collectionBalanceAmount = (loanLines.FirstOrDefault().CollectibleAmount + collectionPenaltyAmount) - collectionPaidAmount;

                        var isCleared = false;
                        if (collectionBalanceAmount == 0)
                        {
                            isCleared = true;
                        }

                        var updateLoanLines = loanLines.FirstOrDefault();
                        updateLoanLines.PaidAmount = collectionPaidAmount;
                        updateLoanLines.PenaltyAmount = collectionPenaltyAmount;
                        updateLoanLines.BalanceAmount = collectionBalanceAmount;
                        updateLoanLines.IsCleared = isCleared;
                        db.SubmitChanges();

                        var loan = from d in db.trnLoans where d.Id == loanLines.FirstOrDefault().LoanId select d;
                        if (loan.Any())
                        {
                            var allLoanLines = from d in db.trnLoanLines
                                                where d.LoanId == loanLines.FirstOrDefault().LoanId
                                                select d;

                            if (allLoanLines.Any())
                            {
                                var updateLoan = loan.FirstOrDefault();
                                updateLoan.TotalPaidAmount = allLoanLines.Sum(d => d.PaidAmount);
                                updateLoan.TotalPenaltyAmount = allLoanLines.Sum(d => d.PenaltyAmount);
                                updateLoan.TotalBalanceAmount = allLoanLines.Sum(d => d.BalanceAmount);
                                db.SubmitChanges();
                            }
                        }
                    }
                }
            }
            else
            {
                var unlockedCollectionLines = from d in db.trnCollectionLines
                                              where d.CollectionId == collectionId
                                              && d.trnCollection.IsLocked == false
                                              select new Models.TrnCollectionLines
                                              {
                                                  Id = d.Id,
                                                  CollectionId = d.CollectionId,
                                                  LoanId = d.LoanId,
                                                  LoanNumber = d.trnLoan.LoanNumber,
                                                  LoanLinesId = d.LoanLinesId,
                                                  LoanLinesDayReference = d.trnLoanLine.DayReference,
                                                  LoanLinesCollectibleDate = d.trnLoanLine.CollectibleDate.ToShortDateString(),
                                                  PenaltyId = d.PenaltyId,
                                                  Penalty = d.mstPenalty.Penalty,
                                                  PenaltyAmount = d.PenaltyAmount,
                                                  PaidAmount = d.PaidAmount,
                                              };

                if (unlockedCollectionLines.Any())
                {
                    foreach (var collectionLine in unlockedCollectionLines)
                    {
                        var loanLines = from d in db.trnLoanLines
                                        where d.Id == collectionLine.LoanLinesId
                                        select d;

                        if (loanLines.Any())
                        {
                            var collectionLinesAmount = from d in db.trnCollectionLines
                                                        where d.LoanLinesId == loanLines.FirstOrDefault().Id
                                                        select d;

                            if (collectionLinesAmount.Any())
                            {
                                var collectionPaidAmount = loanLines.FirstOrDefault().PaidAmount - collectionLine.PaidAmount;
                                var collectionPenaltyAmount = loanLines.FirstOrDefault().PenaltyAmount - collectionLine.PenaltyAmount;
                                var collectionBalanceAmount = (loanLines.FirstOrDefault().CollectibleAmount + collectionPenaltyAmount) - collectionPaidAmount;

                                var isCleared = false;
                                if (collectionBalanceAmount == 0)
                                {
                                    isCleared = true;
                                }

                                var updateLoanLines = loanLines.FirstOrDefault();
                                updateLoanLines.PaidAmount = collectionPaidAmount;
                                updateLoanLines.PenaltyAmount = collectionPenaltyAmount;
                                updateLoanLines.BalanceAmount = collectionBalanceAmount;
                                updateLoanLines.IsCleared = isCleared;
                                db.SubmitChanges();

                                var loan = from d in db.trnLoans where d.Id == loanLines.FirstOrDefault().LoanId select d;
                                if (loan.Any())
                                {
                                    var allLoanLines = from d in db.trnLoanLines
                                                       where d.LoanId == loanLines.FirstOrDefault().LoanId
                                                       select d;

                                    if (allLoanLines.Any())
                                    {
                                        var updateLoan = loan.FirstOrDefault();
                                        updateLoan.TotalPaidAmount = allLoanLines.Sum(d => d.PaidAmount);
                                        updateLoan.TotalPenaltyAmount = allLoanLines.Sum(d => d.PenaltyAmount);
                                        updateLoan.TotalBalanceAmount = allLoanLines.Sum(d => d.BalanceAmount);
                                        db.SubmitChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // lock collection
        [Authorize]
        [HttpPut]
        [Route("api/collection/lock/{id}")]
        public HttpResponseMessage lockCollection(String id, Models.TrnCollection collection)
        {
            try
            {
                var collections = from d in db.trnCollections where d.Id == Convert.ToInt32(id) select d;
                if (collections.Any())
                {
                    if (!collections.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                        var mstUserForms = from d in db.mstUserForms
                                           where d.UserId == userId
                                           select new Models.MstUserForm
                                           {
                                               Id = d.Id,
                                               Form = d.sysForm.Form,
                                               CanPerformActions = d.CanPerformActions
                                           };

                        if (mstUserForms.Any())
                        {
                            String matchPageString = "CollectionDetail";
                            Boolean canPerformActions = false;

                            foreach (var mstUserForm in mstUserForms)
                            {
                                if (mstUserForm.Form.Equals(matchPageString))
                                {
                                    if (mstUserForm.CanPerformActions)
                                    {
                                        canPerformActions = true;
                                    }

                                    break;
                                }
                            }

                            if (canPerformActions)
                            {
                                var collectionLines = from d in db.trnCollectionLines
                                                      where d.CollectionId == Convert.ToInt32(id)
                                                      select d;

                                Decimal totalPaidAmount = 0;
                                if (collectionLines.Any())
                                {
                                    totalPaidAmount = collectionLines.Sum(d => d.PaidAmount);
                                }

                                var lockCollection = collections.FirstOrDefault();
                                lockCollection.CollectionDate = Convert.ToDateTime(collection.CollectionDate);
                                lockCollection.ApplicantId = collection.ApplicantId;
                                lockCollection.StatusId = collection.StatusId;
                                lockCollection.Particulars = collection.Particulars;
                                lockCollection.PreparedByUserId = collection.PreparedByUserId;
                                lockCollection.TotalPaidAmount = totalPaidAmount;
                                lockCollection.IsLocked = true;
                                lockCollection.UpdatedByUserId = userId;
                                lockCollection.UpdatedDateTime = DateTime.Now;
                                db.SubmitChanges();

                                this.updateLoan(Convert.ToInt32(id));

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // unlock collection
        [Authorize]
        [HttpPut]
        [Route("api/collection/unlock/{id}")]
        public HttpResponseMessage unlockCollection(String id)
        {
            try
            {
                var collections = from d in db.trnCollections where d.Id == Convert.ToInt32(id) select d;
                if (collections.Any())
                {
                    if (collections.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                        var mstUserForms = from d in db.mstUserForms
                                           where d.UserId == userId
                                           select new Models.MstUserForm
                                           {
                                               Id = d.Id,
                                               Form = d.sysForm.Form,
                                               CanPerformActions = d.CanPerformActions
                                           };

                        if (mstUserForms.Any())
                        {
                            String matchPageString = "CollectionDetail";
                            Boolean canPerformActions = false;

                            foreach (var mstUserForm in mstUserForms)
                            {
                                if (mstUserForm.Form.Equals(matchPageString))
                                {
                                    if (mstUserForm.CanPerformActions)
                                    {
                                        canPerformActions = true;
                                    }

                                    break;
                                }
                            }

                            if (canPerformActions)
                            {
                                var unlockCollection = collections.FirstOrDefault();
                                unlockCollection.IsLocked = false;
                                unlockCollection.UpdatedByUserId = userId;
                                unlockCollection.UpdatedDateTime = DateTime.Now;
                                db.SubmitChanges();

                                this.updateLoan(Convert.ToInt32(id));

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // delete loan
        [Authorize]
        [HttpDelete]
        [Route("api/collection/delete/{id}")]
        public HttpResponseMessage deleteCollection(String id)
        {
            try
            {
                var collections = from d in db.trnCollections where d.Id == Convert.ToInt32(id) select d;
                if (collections.Any())
                {
                    if (!collections.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                        var mstUserForms = from d in db.mstUserForms
                                           where d.UserId == userId
                                           select new Models.MstUserForm
                                           {
                                               Id = d.Id,
                                               Form = d.sysForm.Form,
                                               CanPerformActions = d.CanPerformActions
                                           };

                        if (mstUserForms.Any())
                        {
                            String matchPageString = "CollectionDetail";
                            Boolean canPerformActions = false;

                            foreach (var mstUserForm in mstUserForms)
                            {
                                if (mstUserForm.Form.Equals(matchPageString))
                                {
                                    if (mstUserForm.CanPerformActions)
                                    {
                                        canPerformActions = true;
                                    }

                                    break;
                                }
                            }

                            if (canPerformActions)
                            {
                                var collectionId = Convert.ToInt32(id);

                                db.trnCollections.DeleteOnSubmit(collections.First());
                                db.SubmitChanges();

                                this.updateLoan(collectionId);

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
