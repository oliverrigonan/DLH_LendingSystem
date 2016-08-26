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
                                  BranchId = d.BranchId,
                                  Branch = d.mstBranch.Branch,
                                  AccountId = d.AccountId,
                                  Account = d.mstAccount.Account,
                                  ApplicantId = d.ApplicantId,
                                  Applicant = d.mstApplicant.ApplicantFullName,
                                  Particulars = d.Particulars,
                                  PaidAmount = d.PaidAmount,
                                  IsCleared = d.IsCleared,
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
                                 BranchId = d.BranchId,
                                 Branch = d.mstBranch.Branch,
                                 AccountId = d.AccountId,
                                 Account = d.mstAccount.Account,
                                 ApplicantId = d.ApplicantId,
                                 Applicant = d.mstApplicant.ApplicantFullName,
                                 Particulars = d.Particulars,
                                 PaidAmount = d.PaidAmount,
                                 IsCleared = d.IsCleared,
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
                newCollection.BranchId = (from d in db.mstBranches select d.Id).FirstOrDefault();
                newCollection.AccountId = (from d in db.mstAccounts select d.Id).FirstOrDefault();
                newCollection.ApplicantId = (from d in db.mstApplicants select d.Id).FirstOrDefault();
                newCollection.Particulars = "NA";
                newCollection.PaidAmount = 0;
                newCollection.IsCleared = false;
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
                        lockCollection.BranchId = collection.BranchId;
                        lockCollection.AccountId = collection.AccountId;
                        lockCollection.ApplicantId = collection.ApplicantId;
                        lockCollection.Particulars = collection.Particulars;
                        lockCollection.PaidAmount = collection.PaidAmount;
                        lockCollection.IsCleared = collection.IsCleared;
                        lockCollection.IsLocked = true;
                        lockCollection.UpdatedByUserId = userId;
                        lockCollection.UpdatedDateTime = DateTime.Now;
                        db.SubmitChanges();



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
        public HttpResponseMessage unlockCollection(String id, Models.TrnCollection collection)
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
