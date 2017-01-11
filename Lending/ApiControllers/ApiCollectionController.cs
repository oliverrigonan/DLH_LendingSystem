using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

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
                                  LoanId = d.LoanId,
                                  LoanNumber = d.trnLoan.LoanNumber,
                                  LoanDetail = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " ") + " - " + d.trnLoan.LoanNumber + " (from " + d.trnLoan.LoanDate + " to " + d.trnLoan.MaturityDate + ")",
                                  CollectibleAmount = d.CollectibleAmount,
                                  IsCleared = d.IsCleared,
                                  IsAbsent = d.IsAbsent,
                                  PenaltyAmount = d.PenaltyAmount,
                                  IsPartiallyPaid = d.IsPartiallyPaid,
                                  IsPaidInAdvanced = d.IsPaidInAdvanced,
                                  AdvancePaidDate = (d.AdvancePaidDate != null ? Convert.ToDateTime(d.AdvancePaidDate).ToShortDateString() : " "),
                                  IsFullyPaid = d.IsFullyPaid,
                                  PaidAmount = d.PaidAmount,
                                  BalanceAmount = d.BalanceAmount,
                                  Status = d.Status,
                                  IsAllowanceDay = d.IsAllowanceDay,
                                  Particulars = d.Particulars,
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
                                 LoanId = d.LoanId,
                                 LoanNumber = d.trnLoan.LoanNumber,
                                 LoanDetail = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " ") + " - " + d.trnLoan.LoanNumber + " (from " + d.trnLoan.LoanDate + " to " + d.trnLoan.MaturityDate + ")",
                                 CollectibleAmount = d.CollectibleAmount,
                                 IsCleared = d.IsCleared,
                                 IsAbsent = d.IsAbsent,
                                 PenaltyAmount = d.PenaltyAmount,
                                 IsPartiallyPaid = d.IsPartiallyPaid,
                                 IsPaidInAdvanced = d.IsPaidInAdvanced,
                                 AdvancePaidDate = (d.AdvancePaidDate != null ? Convert.ToDateTime(d.AdvancePaidDate).ToShortDateString() : " "),
                                 IsFullyPaid = d.IsFullyPaid,
                                 PaidAmount = d.PaidAmount,
                                 BalanceAmount = d.BalanceAmount,
                                 Status = d.Status,
                                 IsAllowanceDay = d.IsAllowanceDay,
                                 Particulars = d.Particulars,
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

                        var loan = from d in db.trnLoans where d.IsLocked == true select d;
                        if (loan.Any())
                        {
                            Data.trnCollection newCollection = new Data.trnCollection();
                            newCollection.CollectionNumber = zeroFill(Convert.ToInt32(collectionNumber), 10);
                            newCollection.CollectionDate = DateTime.Today;
                            newCollection.LoanId = loan.FirstOrDefault().Id;
                            newCollection.CollectibleAmount = loan.FirstOrDefault().CurrentCollectibeAmount;
                            newCollection.IsCleared = false;
                            newCollection.IsAbsent = false;
                            newCollection.PenaltyAmount = 0;
                            newCollection.IsPartiallyPaid = false;
                            newCollection.IsPaidInAdvanced = false;
                            newCollection.AdvancePaidDate = null;
                            newCollection.IsFullyPaid = false;
                            newCollection.PaidAmount = 0;
                            newCollection.BalanceAmount = 0;
                            newCollection.Status = "?";
                            newCollection.IsAllowanceDay = false;
                            newCollection.Particulars = "NA";
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
            catch
            {
                return 0;
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
                                var loan = from d in db.trnLoans where d.Id == collection.LoanId where d.IsLocked == true select d;
                                if (loan.Any())
                                {
                                    var lockCollection = collections.FirstOrDefault();

                                    var isAllowanceDay = false;
                                    DateTime loanEndDate = loan.FirstOrDefault().LoanEndDate;
                                    if (Convert.ToDateTime(collection.CollectionDate) > loan.FirstOrDefault().MaturityDate)
                                    {
                                        isAllowanceDay = true;
                                    }

                                    if (collection.IsCleared)
                                    {
                                        lockCollection.CollectionDate = Convert.ToDateTime(collection.CollectionDate);
                                        lockCollection.LoanId = loan.FirstOrDefault().Id;
                                        lockCollection.CollectibleAmount = loan.FirstOrDefault().CurrentCollectibeAmount;
                                        lockCollection.IsCleared = true;
                                        lockCollection.IsAbsent = false;
                                        lockCollection.PenaltyAmount = 0;
                                        lockCollection.IsPartiallyPaid = false;
                                        lockCollection.IsPaidInAdvanced = false;
                                        lockCollection.AdvancePaidDate = null;
                                        lockCollection.IsFullyPaid = false;
                                        lockCollection.PaidAmount = collection.PaidAmount;
                                        lockCollection.BalanceAmount = loan.FirstOrDefault().CurrentCollectibeAmount - collection.PaidAmount;
                                        lockCollection.Status = "Paid";
                                        lockCollection.IsAllowanceDay = isAllowanceDay;
                                        lockCollection.Particulars = collection.Particulars;
                                        lockCollection.IsLocked = true;
                                        lockCollection.UpdatedByUserId = userId;
                                        lockCollection.UpdatedDateTime = DateTime.Now;

                                        Business.UpdateLoan updateLoan = new Business.UpdateLoan();
                                        updateLoan.updateLoan(loan.FirstOrDefault().Id);

                                        Business.Journal journal = new Business.Journal();
                                        journal.postLoanJournal(Convert.ToInt32(id));

                                        return Request.CreateResponse(HttpStatusCode.OK);
                                    }
                                    else
                                    {
                                        if (collection.IsAbsent)
                                        {
                                            var numberOfAbsent = loan.FirstOrDefault().NoOfAbsent;
                                            var penaltyAmount = loan.FirstOrDefault().mstPenalty.DefaultPenaltyAmount;
                                            if (loan.FirstOrDefault().mstPenalty.NoOfLimitAbsent % numberOfAbsent == 0)
                                            {
                                                penaltyAmount = loan.FirstOrDefault().mstPenalty.PenaltyAmountOverNoOfLimitAbsent;
                                            }

                                            lockCollection.CollectionDate = Convert.ToDateTime(collection.CollectionDate);
                                            lockCollection.LoanId = loan.FirstOrDefault().Id;
                                            lockCollection.CollectibleAmount = loan.FirstOrDefault().CurrentCollectibeAmount;
                                            lockCollection.IsCleared = false;
                                            lockCollection.IsAbsent = true;
                                            lockCollection.IsPartiallyPaid = false;
                                            lockCollection.IsPaidInAdvanced = false;
                                            lockCollection.AdvancePaidDate = null;
                                            lockCollection.IsFullyPaid = false;
                                            lockCollection.PaidAmount = 0;
                                            lockCollection.BalanceAmount = loan.FirstOrDefault().CurrentCollectibeAmount;
                                            lockCollection.Status = "Absent";
                                            lockCollection.IsAllowanceDay = isAllowanceDay;
                                            lockCollection.Particulars = collection.Particulars;
                                            lockCollection.IsLocked = true;
                                            lockCollection.UpdatedByUserId = userId;
                                            lockCollection.UpdatedDateTime = DateTime.Now;

                                            Business.UpdateLoan updateLoan = new Business.UpdateLoan();
                                            updateLoan.updateLoan(loan.FirstOrDefault().Id);

                                            return Request.CreateResponse(HttpStatusCode.OK);
                                        }
                                        else
                                        {
                                            if (collection.IsPartiallyPaid)
                                            {
                                                if (collection.PaidAmount < loan.FirstOrDefault().CurrentCollectibeAmount)
                                                {
                                                    lockCollection.CollectionDate = Convert.ToDateTime(collection.CollectionDate);
                                                    lockCollection.LoanId = loan.FirstOrDefault().Id;
                                                    lockCollection.CollectibleAmount = loan.FirstOrDefault().CurrentCollectibeAmount;
                                                    lockCollection.IsCleared = false;
                                                    lockCollection.IsAbsent = false;
                                                    lockCollection.PenaltyAmount = collection.PenaltyAmount;
                                                    lockCollection.IsPartiallyPaid = true;
                                                    lockCollection.IsPaidInAdvanced = false;
                                                    lockCollection.AdvancePaidDate = null;
                                                    lockCollection.IsFullyPaid = false;
                                                    lockCollection.PaidAmount = collection.PaidAmount;
                                                    lockCollection.BalanceAmount = loan.FirstOrDefault().CurrentCollectibeAmount - collection.PaidAmount;
                                                    lockCollection.Status = "Paid";
                                                    lockCollection.IsAllowanceDay = false;
                                                    lockCollection.Particulars = collection.Particulars;
                                                    lockCollection.IsLocked = true;
                                                    lockCollection.UpdatedByUserId = userId;
                                                    lockCollection.UpdatedDateTime = DateTime.Now;

                                                    Business.UpdateLoan updateLoan = new Business.UpdateLoan();
                                                    updateLoan.updateLoan(loan.FirstOrDefault().Id);

                                                    Business.Journal journal = new Business.Journal();
                                                    journal.postLoanJournal(Convert.ToInt32(id));

                                                    return Request.CreateResponse(HttpStatusCode.OK);
                                                }
                                                else
                                                {
                                                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                                                }
                                            }
                                            else
                                            {
                                                if (collection.IsPaidInAdvanced)
                                                {
                                                    lockCollection.CollectionDate = Convert.ToDateTime(collection.CollectionDate);
                                                    lockCollection.LoanId = loan.FirstOrDefault().Id;
                                                    lockCollection.CollectibleAmount = loan.FirstOrDefault().CurrentCollectibeAmount;
                                                    lockCollection.IsCleared = false;
                                                    lockCollection.IsAbsent = false;
                                                    lockCollection.PenaltyAmount = 0;
                                                    lockCollection.IsPartiallyPaid = false;
                                                    lockCollection.IsPaidInAdvanced = true;
                                                    lockCollection.AdvancePaidDate = Convert.ToDateTime(collection.AdvancePaidDate);
                                                    lockCollection.IsFullyPaid = false;
                                                    lockCollection.PaidAmount = collection.PaidAmount;
                                                    lockCollection.BalanceAmount = loan.FirstOrDefault().CurrentCollectibeAmount - collection.PaidAmount;
                                                    lockCollection.Status = "Advance";
                                                    lockCollection.IsAllowanceDay = isAllowanceDay;
                                                    lockCollection.Particulars = collection.Particulars;
                                                    lockCollection.IsLocked = true;
                                                    lockCollection.UpdatedByUserId = userId;
                                                    lockCollection.UpdatedDateTime = DateTime.Now;

                                                    Business.UpdateLoan updateLoan = new Business.UpdateLoan();
                                                    updateLoan.updateLoan(loan.FirstOrDefault().Id);

                                                    Business.Journal journal = new Business.Journal();
                                                    journal.postLoanJournal(Convert.ToInt32(id));

                                                    return Request.CreateResponse(HttpStatusCode.OK);
                                                }
                                                else
                                                {
                                                    if (collection.IsFullyPaid)
                                                    {
                                                        lockCollection.CollectionDate = Convert.ToDateTime(collection.CollectionDate);
                                                        lockCollection.LoanId = loan.FirstOrDefault().Id;
                                                        lockCollection.CollectibleAmount = loan.FirstOrDefault().CurrentCollectibeAmount;
                                                        lockCollection.IsCleared = false;
                                                        lockCollection.IsAbsent = false;
                                                        lockCollection.PenaltyAmount = 0;
                                                        lockCollection.IsPartiallyPaid = false;
                                                        lockCollection.IsPaidInAdvanced = false;
                                                        lockCollection.AdvancePaidDate = null;
                                                        lockCollection.IsFullyPaid = true;
                                                        lockCollection.PaidAmount = loan.FirstOrDefault().BalanceAmount;
                                                        lockCollection.BalanceAmount = 0;
                                                        lockCollection.Status = "Full";
                                                        lockCollection.IsAllowanceDay = isAllowanceDay;
                                                        lockCollection.Particulars = collection.Particulars;
                                                        lockCollection.IsLocked = true;
                                                        lockCollection.UpdatedByUserId = userId;
                                                        lockCollection.UpdatedDateTime = DateTime.Now;

                                                        Business.UpdateLoan updateLoan = new Business.UpdateLoan();
                                                        updateLoan.updateLoan(loan.FirstOrDefault().Id);

                                                        Business.Journal journal = new Business.Journal();
                                                        journal.postLoanJournal(Convert.ToInt32(id));

                                                        return Request.CreateResponse(HttpStatusCode.OK);
                                                    }
                                                    else
                                                    {
                                                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                                                    }
                                                }
                                            }
                                        }
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
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
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
                            String matchPageString = "LoanApplicationDetail";
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
                                var lastCollection = from d in db.trnCollections.OrderByDescending(d => d.Id)
                                                             where d.LoanId == collections.FirstOrDefault().LoanId
                                                             select d;

                                if (lastCollection.Any())
                                {
                                    if (lastCollection.FirstOrDefault().Id == Convert.ToInt32(id))
                                    {
                                        Int32 loanId = collections.FirstOrDefault().LoanId;

                                        var unlockCollection = collections.FirstOrDefault();
                                        unlockCollection.IsLocked = false;
                                        unlockCollection.UpdatedByUserId = userId;
                                        unlockCollection.UpdatedDateTime = DateTime.Now;
                                        db.SubmitChanges();

                                        Business.UpdateLoan updateLoan = new Business.UpdateLoan();
                                        updateLoan.updateLoan(loanId);

                                        Business.Journal journal = new Business.Journal();
                                        journal.deleteLoanJournal(Convert.ToInt32(id));

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
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot unlock the loan application because it is not yet locked.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry, but there are no data found in the server to unlock this loan application.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Oops! Something went wrong from the server. Please contact the administrator.");
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
                            String matchPageString = "LoanApplicationDetail";
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
                                var lastCollection = from d in db.trnCollections.OrderByDescending(d => d.Id)
                                                             where d.LoanId == collections.FirstOrDefault().LoanId
                                                             select d;

                                if (lastCollection.Any())
                                {
                                    if (lastCollection.FirstOrDefault().Id == Convert.ToInt32(id))
                                    {
                                        db.trnCollections.DeleteOnSubmit(collections.First());
                                        db.SubmitChanges();

                                        Business.UpdateLoan updateLoan = new Business.UpdateLoan();
                                        updateLoan.updateLoan(collections.FirstOrDefault().LoanId);

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
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
