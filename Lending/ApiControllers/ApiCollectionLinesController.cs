using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiCollectionLinesController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // collection Lines list
        [Authorize]
        [HttpGet]
        [Route("api/collectionLines/listByCollectionId/{collectionId}")]
        public List<Models.TrnCollectionLines> listCollectionLinesByCollectionId(String collectionId)
        {
            var collectionLines = from d in db.trnCollectionLines.OrderByDescending(d => d.Id)
                                       where d.CollectionId == Convert.ToInt32(collectionId)
                                       select new Models.TrnCollectionLines
                                       {
                                           Id = d.Id,
                                           CollectionId = d.CollectionId,
                                           AccountId = d.AccountId,
                                           Account = d.mstAccount.Account,
                                           LoanId = d.LoanId,
                                           PaytypeId = d.PaytypeId,
                                           Paytype = d.mstPayType.PayType,
                                           CheckNumber = d.CheckNumber,
                                           CheckDate = d.CheckDate.ToShortDateString(),
                                           CheckBank = d.CheckBank,
                                           Particulars = d.Particulars,
                                           Amount = d.Amount
                                       };

            return collectionLines.ToList();
        }

        // add collection Lines
        [Authorize]
        [HttpPost]
        [Route("api/collectionLines/add")]
        public HttpResponseMessage addCollectionLines(Models.TrnCollectionLines collectionLine)
        {
            try
            {
                var collection = from d in db.trnCollections where d.Id == collectionLine.CollectionId select d;
                if (collection.Any())
                {
                    if (!collection.FirstOrDefault().IsLocked)
                    {
                        Data.trnCollectionLine newCollectionLine = new Data.trnCollectionLine();
                        newCollectionLine.CollectionId = collectionLine.CollectionId;
                        newCollectionLine.AccountId = collectionLine.AccountId;
                        newCollectionLine.LoanId = collectionLine.LoanId;
                        newCollectionLine.PaytypeId = collectionLine.PaytypeId;
                        newCollectionLine.CheckNumber = collectionLine.CheckNumber;
                        newCollectionLine.CheckDate = Convert.ToDateTime(collectionLine.CheckDate);
                        newCollectionLine.CheckBank = collectionLine.CheckBank;
                        newCollectionLine.Particulars = collectionLine.Particulars;
                        newCollectionLine.Amount = collectionLine.Amount;
                        db.trnCollectionLines.InsertOnSubmit(newCollectionLine);
                        db.SubmitChanges();

                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                        Decimal totalCollectionPaidAmount = 0;
                        var collectionLinesForTotalPaidAmount = from d in db.trnCollectionLines where d.CollectionId == collectionLine.CollectionId select d;
                        if (collectionLinesForTotalPaidAmount.Any())
                        {
                            totalCollectionPaidAmount = collectionLinesForTotalPaidAmount.Sum(d => d.Amount);
                        }

                        var updateCollectionPaidAmount = collection.FirstOrDefault();
                        updateCollectionPaidAmount.PaidAmount = totalCollectionPaidAmount;
                        updateCollectionPaidAmount.UpdatedByUserId = userId;
                        updateCollectionPaidAmount.UpdatedDateTime = DateTime.Now;
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

        // update collection Lines
        [Authorize]
        [HttpPut]
        [Route("api/collectionLines/update/{id}")]
        public HttpResponseMessage updateCollectionLines(String id, Models.TrnCollectionLines collectionLine)
        {
            try
            {
                var collection = from d in db.trnCollections where d.Id == collectionLine.CollectionId select d;
                if (collection.Any())
                {
                    if (!collection.FirstOrDefault().IsLocked)
                    {
                        var collectionLines = from d in db.trnCollectionLines where d.Id == Convert.ToInt32(id) select d;
                        if (collectionLines.Any())
                        {
                            var updateCollectionApplicationLine = collectionLines.FirstOrDefault();
                            updateCollectionApplicationLine.CollectionId = collectionLine.CollectionId;
                            updateCollectionApplicationLine.AccountId = collectionLine.AccountId;
                            updateCollectionApplicationLine.LoanId = collectionLine.LoanId;
                            updateCollectionApplicationLine.PaytypeId = collectionLine.PaytypeId;
                            updateCollectionApplicationLine.CheckNumber = collectionLine.CheckNumber;
                            updateCollectionApplicationLine.CheckDate = Convert.ToDateTime(collectionLine.CheckDate);
                            updateCollectionApplicationLine.CheckBank = collectionLine.CheckBank;
                            updateCollectionApplicationLine.Particulars = collectionLine.Particulars;
                            updateCollectionApplicationLine.Amount = collectionLine.Amount;
                            db.SubmitChanges();

                            var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                            Decimal totalCollectionPaidAmount = 0;
                            var collectionLinesForTotalPaidAmount = from d in db.trnCollectionLines where d.CollectionId == collectionLine.CollectionId select d;
                            if (collectionLinesForTotalPaidAmount.Any())
                            {
                                totalCollectionPaidAmount = collectionLinesForTotalPaidAmount.Sum(d => d.Amount);
                            }

                            var updateCollectionPaidAmount = collection.FirstOrDefault();
                            updateCollectionPaidAmount.PaidAmount = totalCollectionPaidAmount;
                            updateCollectionPaidAmount.UpdatedByUserId = userId;
                            updateCollectionPaidAmount.UpdatedDateTime = DateTime.Now;
                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound);
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

        // delete collection Lines
        [Authorize]
        [HttpDelete]
        [Route("api/collectionLines/delete/{id}/{collectionId}")]
        public HttpResponseMessage deleteCollectionLines(String id, String collectionId)
        {
            try
            {
                var collection = from d in db.trnCollections where d.Id == Convert.ToInt32(collectionId) select d;
                if (collection.Any())
                {
                    if (!collection.FirstOrDefault().IsLocked)
                    {
                        var collectionLines = from d in db.trnCollectionLines where d.Id == Convert.ToInt32(id) select d;
                        if (collectionLines.Any())
                        {
                            db.trnCollectionLines.DeleteOnSubmit(collectionLines.First());
                            db.SubmitChanges();

                            var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                            Decimal totalCollectionPaidAmount = 0;
                            var collectionLinesForTotalPaidAmount = from d in db.trnCollectionLines where d.CollectionId == Convert.ToInt32(collectionId) select d;
                            if (collectionLinesForTotalPaidAmount.Any())
                            {
                                totalCollectionPaidAmount = collectionLinesForTotalPaidAmount.Sum(d => d.Amount);
                            }

                            var updateCollectionPaidAmount = collection.FirstOrDefault();
                            updateCollectionPaidAmount.PaidAmount = totalCollectionPaidAmount;
                            updateCollectionPaidAmount.UpdatedByUserId = userId;
                            updateCollectionPaidAmount.UpdatedDateTime = DateTime.Now;
                            db.SubmitChanges();
                        }

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
