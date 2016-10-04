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

        // get total amount in collection lines
        public Decimal getTotalPaidAmount(Int32 collectionId)
        {
            Decimal totalPaidAmount = 0;
            var collectionLines = from d in db.trnCollectionLines where d.CollectionId == collectionId select d;
            if (collectionLines.Any())
            {
                totalPaidAmount = collectionLines.Sum(d => d.Amount);
            }

            return totalPaidAmount;
        }

        // collection list
        [Authorize]
        [HttpGet]
        [Route("api/collection/listByCollectionDate/{collectionDate}")]
        public List<Models.TrnCollection> listCollectionByCollectionDate(String collectionDate)
        {
            var collections = from d in db.trnCollections.OrderByDescending(d => d.Id)
                              where d.CollectionDate == Convert.ToDateTime(collectionDate)
                              select new Models.TrnCollection
                              {
                                  Id = d.Id,
                                  CollectionNumber = d.CollectionNumber,
                                  CollectionDate = d.CollectionDate.ToShortDateString(),
                                  ApplicantId = d.ApplicantId,
                                  Applicant = d.mstApplicant.ApplicantLastName + " " + d.mstApplicant.ApplicantFirstName + ", " + d.mstApplicant.ApplicantMiddleName,
                                  Particulars = d.Particulars,
                                  PaidAmount = getTotalPaidAmount(d.Id),
                                  PreparedByUserId = d.PreparedByUserId,
                                  PreparedByUser = d.mstUser2.FullName,
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

        // collection by id
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
                                 Applicant = d.mstApplicant.ApplicantLastName + " " + d.mstApplicant.ApplicantFirstName + ", " + d.mstApplicant.ApplicantMiddleName,
                                 Particulars = d.Particulars,
                                 PaidAmount = getTotalPaidAmount(d.Id),
                                 PreparedByUserId = d.PreparedByUserId,
                                 PreparedByUser = d.mstUser2.FullName,
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

                String collectionNumber = "0000000001";
                var collection = from d in db.trnCollections.OrderByDescending(d => d.Id) select d;
                if (collection.Any())
                {
                    var newCollectionNumber = Convert.ToInt32(collection.FirstOrDefault().CollectionNumber) + 0000000001;
                    collectionNumber = newCollectionNumber.ToString();
                }

                Data.trnCollection newCollection = new Data.trnCollection();
                newCollection.CollectionNumber = zeroFill(Convert.ToInt32(collectionNumber), 10); ;
                newCollection.CollectionDate = DateTime.Today;
                newCollection.ApplicantId = (from d in db.mstApplicants.OrderByDescending(d => d.Id) select d.Id).FirstOrDefault();
                newCollection.Particulars = "NA";
                newCollection.PreparedByUserId = userId;
                newCollection.IsLocked = false;
                newCollection.CreatedByUserId = userId;
                newCollection.CreatedDateTime = DateTime.Now;
                newCollection.UpdatedByUserId = userId;
                newCollection.UpdatedDateTime = DateTime.Now;

                db.trnCollections.InsertOnSubmit(newCollection);
                db.SubmitChanges();

                return newCollection.Id;
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

                        var lockCollection = collections.FirstOrDefault();
                        lockCollection.CollectionDate = Convert.ToDateTime(collection.CollectionDate);
                        lockCollection.ApplicantId = collection.ApplicantId;
                        lockCollection.Particulars = collection.Particulars;
                        lockCollection.PreparedByUserId = collection.PreparedByUserId;
                        lockCollection.IsLocked = true;
                        lockCollection.UpdatedByUserId = userId;
                        lockCollection.UpdatedDateTime = DateTime.Now;
                        db.SubmitChanges();

                        var collectionLines = from d in db.trnCollectionLines
                                              where d.CollectionId == Convert.ToInt32(id)
                                              select new Models.TrnCollectionLines
                                              {
                                                  Id = d.Id,
                                                  CollectionId = d.CollectionId,
                                                  AccountId = d.AccountId,
                                                  Account = d.mstAccount.Account,
                                                  LoanId = d.LoanId,
                                                  Particulars = d.Particulars,
                                                  Amount = d.Amount
                                              };

                        if (collectionLines.Any())
                        {
                            foreach (var collectionLine in collectionLines)
                            {
                                if (collectionLine.Amount > 0)
                                {
                                    var loanApplications = from d in db.trnLoanApplications where d.Id == collectionLine.LoanId select d;
                                    if (loanApplications.Any())
                                    {
                                        //Decimal paidAmount = 0;
                                        //var loanApplicationForPaidAmount = from d in db.trnLoanApplications where d.Id == collectionLine.LoanId select d;
                                        //if (loanApplicationForPaidAmount.Any())
                                        //{
                                        //    paidAmount = loanApplicationForPaidAmount.FirstOrDefault().PaidAmount;
                                        //}

                                        //var updateLoanAmount = loanApplications.FirstOrDefault();
                                        //updateLoanAmount.PaidAmount = paidAmount + collectionLine.Amount;
                                        //updateLoanAmount.BalanceAmount = loanApplications.FirstOrDefault().LoanAmount - (paidAmount + collectionLine.Amount);
                                        //db.SubmitChanges();
                                    }
                                }
                            }
                        }

                        Business.Journal journal = new Business.Journal();
                        journal.postCollectionJournal(Convert.ToInt32(id));

                        return Request.CreateResponse(HttpStatusCode.OK);
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

                        var unlockCollection = collections.FirstOrDefault();
                        unlockCollection.IsLocked = false;
                        unlockCollection.UpdatedByUserId = userId;
                        unlockCollection.UpdatedDateTime = DateTime.Now;
                        db.SubmitChanges();

                        var collectionLines = from d in db.trnCollectionLines
                                              where d.CollectionId == Convert.ToInt32(id)
                                              select new Models.TrnCollectionLines
                                              {
                                                  Id = d.Id,
                                                  CollectionId = d.CollectionId,
                                                  AccountId = d.AccountId,
                                                  Account = d.mstAccount.Account,
                                                  LoanId = d.LoanId,
                                                  Particulars = d.Particulars,
                                                  Amount = d.Amount
                                              };

                        if (collectionLines.Any())
                        {
                            foreach (var collectionLine in collectionLines)
                            {
                                var loanApplications = from d in db.trnLoanApplications where d.Id == collectionLine.LoanId select d;
                                if (loanApplications.Any())
                                {
                                    //Decimal paidAmount = 0;
                                    //var loanApplicationForPaidAmount = from d in db.trnLoanApplications where d.Id == collectionLine.LoanId select d;
                                    //if (loanApplicationForPaidAmount.Any())
                                    //{
                                    //    paidAmount = loanApplicationForPaidAmount.FirstOrDefault().PaidAmount;
                                    //}

                                    //var updateLoanAmount = loanApplications.FirstOrDefault();
                                    //updateLoanAmount.PaidAmount = paidAmount - collectionLine.Amount;
                                    //updateLoanAmount.BalanceAmount = loanApplications.FirstOrDefault().LoanAmount - (paidAmount - collectionLine.Amount);
                                    //db.SubmitChanges();
                                }
                            }
                        }

                        Business.Journal journal = new Business.Journal();
                        journal.deleteCollectionJournal(Convert.ToInt32(id));

                        return Request.CreateResponse(HttpStatusCode.OK);
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

        // delete collection
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
                        db.trnCollections.DeleteOnSubmit(collections.First());
                        db.SubmitChanges();

                        var collectionLines = from d in db.trnCollectionLines
                                              where d.CollectionId == Convert.ToInt32(id)
                                              select new Models.TrnCollectionLines
                                              {
                                                  Id = d.Id,
                                                  CollectionId = d.CollectionId,
                                                  AccountId = d.AccountId,
                                                  Account = d.mstAccount.Account,
                                                  LoanId = d.LoanId,
                                                  Particulars = d.Particulars,
                                                  Amount = d.Amount
                                              };

                        //if (collectionLines.Any())
                        //{
                        //    foreach (var collectionLine in collectionLines)
                        //    {
                        //        var loanApplications = from d in db.trnLoanApplications where d.Id == collectionLine.LoanId select d;
                        //        if (loanApplications.Any())
                        //        {
                        //            var updateLoanAmount = loanApplications.FirstOrDefault();
                        //            updateLoanAmount.PaidAmount = 0;
                        //            updateLoanAmount.BalanceAmount = loanApplications.FirstOrDefault().LoanAmount - 0;
                        //            db.SubmitChanges();
                        //        }
                        //    }
                        //}

                        Business.Journal journal = new Business.Journal();
                        journal.deleteCollectionJournal(Convert.ToInt32(id));

                        return Request.CreateResponse(HttpStatusCode.OK);
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
