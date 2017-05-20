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
    public class ApiCollectionLinesController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // collection lines list by loan id
        [Authorize]
        [HttpGet]
        [Route("api/collectionLines/listByLoanId/{loanId}")]
        public List<Models.TrnCollectionLines> listCollectionLinesByLoanId(String loanId)
        {
            var collectionLines = from d in db.trnCollectionLines.OrderByDescending(d => d.Id)
                                  where d.trnCollection.LoanId == Convert.ToInt32(loanId)
                                  && d.trnCollection.IsLocked == true
                                  select new Models.TrnCollectionLines
                                  {
                                      Id = d.Id,
                                      CollectionId = d.CollectionId,
                                      CollectionNumber = d.trnCollection.CollectionNumber,
                                      PayDate = d.PayDate.ToShortDateString(),
                                      Particulars = d.Particulars,
                                      StatusId = d.StatusId,
                                      Status = d.sysCollectionStatus.Status,
                                      PaidAmount = d.PaidAmount,
                                      PenaltyAmount = d.PenaltyAmount,
                                  };

            return collectionLines.ToList();
        }

        // collection lines list by collection id
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
                                      CollectionNumber = d.trnCollection.CollectionNumber,
                                      PayDate = d.PayDate.ToShortDateString(),
                                      Particulars = d.Particulars,
                                      StatusId = d.StatusId,
                                      Status = d.sysCollectionStatus.Status,
                                      PaidAmount = d.PaidAmount,
                                      PenaltyAmount = d.PenaltyAmount,
                                  };

            return collectionLines.ToList();
        }

        // add collection lines
        [Authorize]
        [HttpPost]
        [Route("api/collectionLines/add")]
        public HttpResponseMessage addCollectionLines(Models.TrnCollectionLines collectionLines)
        {
            try
            {
                var collection = from d in db.trnCollections where d.Id == collectionLines.CollectionId select d;
                if (collection.Any())
                {
                    if (!collection.FirstOrDefault().IsLocked)
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
                                var loan = from d in db.trnLoans
                                           where d.Id == collectionLines.LoanId
                                           select d;

                                if (loan.Any())
                                {
                                    if (!loan.FirstOrDefault().IsReconstructed)
                                    {
                                        if (!loan.FirstOrDefault().IsRenewed)
                                        {
                                            Data.trnCollectionLine newCollectionLine = new Data.trnCollectionLine();
                                            newCollectionLine.CollectionId = collectionLines.CollectionId;
                                            newCollectionLine.PayDate = Convert.ToDateTime(collectionLines.PayDate);
                                            newCollectionLine.Particulars = collectionLines.Particulars;
                                            newCollectionLine.StatusId = collectionLines.StatusId;
                                            newCollectionLine.PaidAmount = collectionLines.PaidAmount;
                                            newCollectionLine.PenaltyAmount = collectionLines.PenaltyAmount;
                                            db.trnCollectionLines.InsertOnSubmit(newCollectionLine);
                                            db.SubmitChanges();

                                            var collectionLineAmount = from d in db.trnCollectionLines
                                                                       where d.CollectionId == Convert.ToInt32(collectionLines.CollectionId)
                                                                       select d;

                                            Decimal totalPaidAmount = 0;
                                            Decimal totalPenaltyAmount = 0;
                                            if (collectionLineAmount.Any())
                                            {
                                                totalPaidAmount = collectionLineAmount.Sum(d => d.PaidAmount);
                                                totalPenaltyAmount = collectionLineAmount.Sum(d => d.PenaltyAmount);
                                            }

                                            var updateCollection = collection.FirstOrDefault();
                                            updateCollection.TotalPaidAmount = totalPaidAmount;
                                            updateCollection.TotalPaidAmount = totalPenaltyAmount;
                                            db.SubmitChanges();

                                            return Request.CreateResponse(HttpStatusCode.OK);
                                        }
                                        else
                                        {
                                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot Pay Renewed Loan.");
                                        }
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot Pay Reconstructed Loan.");
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. Invalid Loan Record.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to delete record.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to delete record.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Record locked.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry. Data not found.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong from the server.");
            }
        }

        // update collection lines
        [Authorize]
        [HttpPut]
        [Route("api/collectionLines/update/{id}")]
        public HttpResponseMessage updateCollectionLines(String id, Models.TrnCollectionLines collectionLines)
        {
            try
            {
                var collection = from d in db.trnCollections where d.Id == collectionLines.CollectionId select d;
                if (collection.Any())
                {
                    if (!collection.FirstOrDefault().IsLocked)
                    {
                        var collectionLine = from d in db.trnCollectionLines where d.Id == Convert.ToInt32(id) select d;
                        if (collectionLine.Any())
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
                                     var loan = from d in db.trnLoans
                                           where d.Id == collectionLines.LoanId
                                           select d;

                                     if (loan.Any())
                                     {
                                         if (!loan.FirstOrDefault().IsReconstructed)
                                         {
                                             if (!loan.FirstOrDefault().IsRenewed)
                                             {

                                                 var updateCollectionLines = collectionLine.FirstOrDefault();
                                                 updateCollectionLines.CollectionId = collectionLines.CollectionId;
                                                 updateCollectionLines.PayDate = Convert.ToDateTime(collectionLines.PayDate);
                                                 updateCollectionLines.Particulars = collectionLines.Particulars;
                                                 updateCollectionLines.StatusId = collectionLines.StatusId;
                                                 updateCollectionLines.PaidAmount = collectionLines.PaidAmount;
                                                 updateCollectionLines.PenaltyAmount = collectionLines.PenaltyAmount;
                                                 db.SubmitChanges();

                                                 var collectionLineAmount = from d in db.trnCollectionLines
                                                                            where d.CollectionId == Convert.ToInt32(collectionLines.CollectionId)
                                                                            select d;

                                                 Decimal totalPaidAmount = 0;
                                                 Decimal totalPenaltyAmount = 0;
                                                 if (collectionLineAmount.Any())
                                                 {
                                                     totalPaidAmount = collectionLineAmount.Sum(d => d.PaidAmount);
                                                     totalPenaltyAmount = collectionLineAmount.Sum(d => d.PenaltyAmount);
                                                 }

                                                 var updateCollection = collection.FirstOrDefault();
                                                 updateCollection.TotalPaidAmount = totalPaidAmount;
                                                 updateCollection.TotalPaidAmount = totalPenaltyAmount;
                                                 db.SubmitChanges();

                                                 return Request.CreateResponse(HttpStatusCode.OK);
                                             }
                                             else
                                             {
                                                 return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot Pay Renewed Loan.");
                                             }
                                         }
                                         else
                                         {
                                             return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot Pay Reconstructed Loan.");
                                         }
                                    }
                                    else
                                     {
                                         return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. Invalid Loan Record.");
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

        // delete collection lines
        [Authorize]
        [HttpDelete]
        [Route("api/collectionLines/delete/{id}/{collectionId}")]
        public HttpResponseMessage deleteCollectionLine(String id, String collectionId)
        {
            try
            {
                var collectionLines = from d in db.trnCollectionLines where d.Id == Convert.ToInt32(id) select d;
                if (collectionLines.Any())
                {
                    var collection = from d in db.trnCollections where d.Id == collectionLines.FirstOrDefault().CollectionId select d;
                    if (collection.Any())
                    {
                        if (!collection.FirstOrDefault().IsLocked)
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
                                    db.trnCollectionLines.DeleteOnSubmit(collectionLines.First());
                                    db.SubmitChanges();

                                    var collectionLineAmount = from d in db.trnCollectionLines
                                                               where d.CollectionId == Convert.ToInt32(collectionId)
                                                               select d;

                                    Decimal totalPaidAmount = 0;
                                    Decimal totalPenaltyAmount = 0;
                                    if (collectionLineAmount.Any())
                                    {
                                        totalPaidAmount = collectionLineAmount.Sum(d => d.PaidAmount);
                                        totalPenaltyAmount = collectionLineAmount.Sum(d => d.PenaltyAmount);
                                    }

                                    var currentCollection = from d in db.trnCollections
                                                            where d.Id == Convert.ToInt32(collectionId)
                                                            select d;

                                    if (currentCollection.Any())
                                    {
                                        var updateCollection = currentCollection.FirstOrDefault();
                                        updateCollection.TotalPaidAmount = totalPaidAmount;
                                        updateCollection.TotalPaidAmount = totalPenaltyAmount;
                                        db.SubmitChanges();
                                    }

                                    return Request.CreateResponse(HttpStatusCode.OK);
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to delete record.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to delete record.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot delete locked record.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry. Data not found.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry. Data not found.");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong from the server.");
            }
        }
    }
}
