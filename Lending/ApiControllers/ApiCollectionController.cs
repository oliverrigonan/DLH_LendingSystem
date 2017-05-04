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
        [Route("api/collections/list/ByCollectionDate/{startCollectionDate}/{endCollectionDate}")]
        public List<Models.TrnCollection> listCollectionByCollectionDate(String startCollectionDate, String endCollectionDate)
        {
            var collections = from d in db.trnCollections.OrderByDescending(d => d.Id)
                              where d.CollectionDate >= Convert.ToDateTime(startCollectionDate)
                              && d.CollectionDate <= Convert.ToDateTime(endCollectionDate)
                              select new Models.TrnCollection
                              {
                                  Id = d.Id,
                                  CollectionNumber = d.CollectionNumber,
                                  CollectionDate = d.CollectionDate.ToShortDateString(),
                                  ApplicantId = d.trnLoan.ApplicantId,
                                  Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                  LoanId = d.LoanId,
                                  LoanNumberDetail = d.trnLoan.IsLoanApplication == true ? "LN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanReconstruct == true ? "RC-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanRenew == true ? "RN-" + d.trnLoan.LoanNumber : " ",
                                  StatusId = d.StatusId,
                                  Status = d.sysCollectionStatus.Status,
                                  Particulars = d.Particulars,
                                  TotalPaidAmount = d.TotalPaidAmount,
                                  TotalPenaltyAmount = d.TotalPenaltyAmount,
                                  CollectorStaffId = d.CollectorStaffId,
                                  CollectorStaff = d.mstStaff.Staff,
                                  PreparedByUserId = d.PreparedByUserId,
                                  PreparedByUser = d.mstUser.FullName,
                                  IsReconstruct = d.trnLoan.IsReconstruct,
                                  IsRenew = d.trnLoan.IsRenew,
                                  IsLoanApplication = d.trnLoan.IsLoanApplication,
                                  IsLoanReconstruct = d.trnLoan.IsLoanReconstruct,
                                  IsLoanRenew = d.trnLoan.IsLoanRenew,
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
                                 ApplicantId = d.trnLoan.ApplicantId,
                                 Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                 LoanId = d.LoanId,
                                 LoanNumberDetail = d.trnLoan.IsLoanApplication == true ? "LN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanReconstruct == true ? "RC-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanRenew == true ? "RN-" + d.trnLoan.LoanNumber : " ",
                                 StatusId = d.StatusId,
                                 Status = d.sysCollectionStatus.Status,
                                 Particulars = d.Particulars,
                                 TotalPaidAmount = d.TotalPaidAmount,
                                 TotalPenaltyAmount = d.TotalPenaltyAmount,
                                 CollectorStaffId = d.CollectorStaffId,
                                 CollectorStaff = d.mstStaff.Staff,
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

                        var loan = from d in db.trnLoans.OrderByDescending(d => d.Id)
                                   where d.IsLocked == true
                                   select d;

                        if (loan.Any())
                        {
                            var status = from d in db.sysCollectionStatus select d;
                            if (status.Any())
                            {
                                var staffs = from d in db.mstStaffs.OrderByDescending(d => d.Id) select d;
                                if (staffs.Any())
                                {
                                    Data.trnCollection newCollection = new Data.trnCollection();
                                    newCollection.CollectionNumber = zeroFill(Convert.ToInt32(collectionNumber), 10);
                                    newCollection.CollectionDate = DateTime.Today;
                                    newCollection.LoanId = loan.FirstOrDefault().Id;
                                    newCollection.StatusId = status.FirstOrDefault().Id;
                                    newCollection.Particulars = "NA";
                                    newCollection.CollectorStaffId = staffs.FirstOrDefault().Id;
                                    newCollection.PreparedByUserId = userId;
                                    newCollection.TotalPaidAmount = 0;
                                    newCollection.TotalPenaltyAmount = 0;
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
                                            LoanId = d.trnCollection.LoanId,
                                            LoanLinesLoanId = d.trnLoanLine.LoanId,
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
                    if (collectionLine.LoanId == collectionLine.LoanLinesLoanId)
                    {
                        var loanLines = from d in db.trnLoanLines
                                        where d.Id == collectionLine.LoanLinesId
                                        select d;

                        if (loanLines.Any())
                        {
                            // dri nga part sa penalty amount
                            var collectionPaidAmount = loanLines.FirstOrDefault().PaidAmount + collectionLine.PaidAmount;
                            var collectionPenaltyAmount = loanLines.FirstOrDefault().PenaltyAmount + collectionLine.PenaltyAmount;

                            var updateLoanLines = loanLines.FirstOrDefault();
                            updateLoanLines.PaidAmount = collectionPaidAmount;
                            updateLoanLines.PenaltyAmount = collectionPenaltyAmount;
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
                                    updateLoan.TotalBalanceAmount = (allLoanLines.FirstOrDefault().trnLoan.NetCollectionAmount - allLoanLines.Sum(d => d.PaidAmount)) + allLoanLines.Sum(d => d.PenaltyAmount);
                                    db.SubmitChanges();
                                }
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
                                                  LoanId = d.trnCollection.LoanId,
                                                  LoanLinesLoanId = d.trnLoanLine.LoanId,
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
                        if (collectionLine.LoanId == collectionLine.LoanLinesLoanId)
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

                                    var updateLoanLines = loanLines.FirstOrDefault();
                                    updateLoanLines.PaidAmount = collectionPaidAmount;
                                    updateLoanLines.PenaltyAmount = collectionPenaltyAmount;
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
                                            updateLoan.TotalBalanceAmount = (allLoanLines.FirstOrDefault().trnLoan.NetCollectionAmount - allLoanLines.Sum(d => d.PaidAmount)) + allLoanLines.Sum(d => d.PenaltyAmount);
                                            db.SubmitChanges();
                                        }
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
                                if (collections.FirstOrDefault().trnLoan.TotalBalanceAmount > 0)
                                {
                                    var collectionLines = from d in db.trnCollectionLines
                                                          where d.CollectionId == Convert.ToInt32(id)
                                                          select d;

                                    Decimal totalPaidAmount = 0;
                                    Decimal totalPenaltyAmount = 0;
                                    if (collectionLines.Any())
                                    {
                                        totalPaidAmount = collectionLines.Sum(d => d.PaidAmount);
                                        totalPenaltyAmount = collectionLines.Sum(d => d.PenaltyAmount);
                                    }

                                    var lockCollection = collections.FirstOrDefault();
                                    lockCollection.CollectionDate = Convert.ToDateTime(collection.CollectionDate);
                                    lockCollection.LoanId = collection.LoanId;
                                    lockCollection.StatusId = collection.StatusId;
                                    lockCollection.Particulars = collection.Particulars;
                                    lockCollection.CollectorStaffId = collection.CollectorStaffId;
                                    lockCollection.PreparedByUserId = collection.PreparedByUserId;
                                    lockCollection.TotalPaidAmount = totalPaidAmount;
                                    lockCollection.TotalPenaltyAmount = totalPenaltyAmount;
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
                                //var collectionLines = from d in db.trnCollectionLines
                                //                      where d.CollectionId == Convert.ToInt32(id)
                                //                      select new Models.TrnCollectionLines
                                //                      {
                                //                          Id = d.Id,
                                //                          CollectionId = d.CollectionId,
                                //                          LoanLinesId = d.LoanLinesId,
                                //                          LoanLinesDayReference = d.trnLoanLine.DayReference,
                                //                          LoanLinesCollectibleDate = d.trnLoanLine.CollectibleDate.ToShortDateString(),
                                //                          PenaltyId = d.PenaltyId,
                                //                          Penalty = d.mstPenalty.Penalty,
                                //                          PenaltyAmount = d.PenaltyAmount,
                                //                          PaidAmount = d.PaidAmount,
                                //                          IsReconstructed = d.trnLoanLine.trnLoan.IsReconstruct
                                //                      };

                                //if (collectionLines.Any())
                                //{
                                //    //var isReconstructed = false;
                                //    //foreach (var collectionLine in collectionLines)
                                //    //{
                                //    //    if (collectionLine.IsReconstructed)
                                //    //    {
                                //    //        isReconstructed = true;
                                //    //        break;
                                //    //    }
                                //    //}

                                //    //if (!isReconstructed)
                                //    //{
                                var unlockCollection = collections.FirstOrDefault();
                                unlockCollection.IsLocked = false;
                                unlockCollection.UpdatedByUserId = userId;
                                unlockCollection.UpdatedDateTime = DateTime.Now;
                                db.SubmitChanges();

                                this.updateLoan(Convert.ToInt32(id));

                                return Request.CreateResponse(HttpStatusCode.OK);
                                //}
                                //else
                                //{
                                //    return Request.CreateResponse(HttpStatusCode.BadRequest, "");
                                //}
                                //}
                                //else
                                //{
                                //    var unlockCollection = collections.FirstOrDefault();
                                //    unlockCollection.IsLocked = false;
                                //    unlockCollection.UpdatedByUserId = userId;
                                //    unlockCollection.UpdatedDateTime = DateTime.Now;
                                //    db.SubmitChanges();

                                //    this.updateLoan(Convert.ToInt32(id));

                                //    return Request.CreateResponse(HttpStatusCode.OK);
                                //}
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
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Already Unlocked.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry. Data not found");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong from the server.");
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
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. Cannot delete locked record.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry. Data not found");
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong from the server.");
            }
        }

        // collection list
        [Authorize]
        [HttpGet]
        [Route("api/collections/list/ByApplicantId/{applicantId}/{loanId}")]
        public List<Models.TrnCollection> listCollectionByApplicantId(String applicantId, String loanId)
        {
            var collections = from d in db.trnCollections
                              where d.trnLoan.ApplicantId == Convert.ToInt32(applicantId)
                              && d.LoanId == Convert.ToInt32(loanId)
                              && d.IsLocked == true
                              select new Models.TrnCollection
                              {
                                  Id = d.Id,
                                  CollectionNumber = d.CollectionNumber,
                                  CollectionDate = d.CollectionDate.ToShortDateString(),
                                  ApplicantId = d.trnLoan.ApplicantId,
                                  Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                  LoanId = d.LoanId,
                                  LoanNumberDetail = d.trnLoan.IsLoanApplication == true ? "LN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanReconstruct == true ? "RC-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanRenew == true ? "RN-" + d.trnLoan.LoanNumber : " ",
                                  StatusId = d.StatusId,
                                  Status = d.sysCollectionStatus.Status,
                                  Particulars = d.Particulars,
                                  TotalPaidAmount = d.TotalPaidAmount,
                                  TotalPenaltyAmount = d.TotalPenaltyAmount,
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

        // collection list - active
        [Authorize]
        [HttpGet]
        [Route("api/collections/list/ByCollectionDate/active/{areaId}/{startCollectionDate}/{endCollectionDate}")]
        public List<Models.TrnCollection> listCollectionByCollectionDateActive(String areaId, String startCollectionDate, String endCollectionDate)
        {
            var collections = from d in db.trnCollections
                              where d.trnLoan.mstApplicant.AreaId == Convert.ToInt32(areaId)
                              && d.CollectionDate >= Convert.ToDateTime(startCollectionDate)
                              && d.CollectionDate <= Convert.ToDateTime(endCollectionDate)
                              && d.trnLoan.IsLoanReconstruct != true
                              && d.IsLocked == true
                              select new Models.TrnCollection
                              {
                                  Id = d.Id,
                                  CollectionNumber = d.CollectionNumber,
                                  CollectionDate = d.CollectionDate.ToShortDateString(),
                                  ApplicantId = d.trnLoan.ApplicantId,
                                  Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                  LoanId = d.LoanId,
                                  LoanNumberDetail = d.trnLoan.IsLoanApplication == true ? "LN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanReconstruct == true ? "RC-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanRenew == true ? "RN-" + d.trnLoan.LoanNumber : " ",
                                  StatusId = d.StatusId,
                                  Status = d.sysCollectionStatus.Status,
                                  Particulars = d.Particulars,
                                  TotalPaidAmount = d.TotalPaidAmount,
                                  TotalPenaltyAmount = d.TotalPenaltyAmount,
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

        // collection list - overdue
        [Authorize]
        [HttpGet]
        [Route("api/collections/list/ByCollectionDate/overdue/{areaId}/{startCollectionDate}/{endCollectionDate}")]
        public List<Models.TrnCollection> listCollectionByCollectionDateOverdue(String areaId, String startCollectionDate, String endCollectionDate)
        {
            var collections = from d in db.trnCollections
                              where d.trnLoan.mstApplicant.AreaId == Convert.ToInt32(areaId)
                              && d.CollectionDate >= Convert.ToDateTime(startCollectionDate)
                              && d.CollectionDate <= Convert.ToDateTime(endCollectionDate)
                              && d.trnLoan.IsLoanReconstruct == true
                              && d.IsLocked == true
                              select new Models.TrnCollection
                              {
                                  Id = d.Id,
                                  CollectionNumber = d.CollectionNumber,
                                  CollectionDate = d.CollectionDate.ToShortDateString(),
                                  ApplicantId = d.trnLoan.ApplicantId,
                                  Applicant = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " "),
                                  LoanId = d.LoanId,
                                  LoanNumberDetail = d.trnLoan.IsLoanApplication == true ? "LN-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanReconstruct == true ? "RC-" + d.trnLoan.LoanNumber : d.trnLoan.IsLoanRenew == true ? "RN-" + d.trnLoan.LoanNumber : " ",
                                  StatusId = d.StatusId,
                                  Status = d.sysCollectionStatus.Status,
                                  Particulars = d.Particulars,
                                  TotalPaidAmount = d.TotalPaidAmount,
                                  TotalPenaltyAmount = d.TotalPenaltyAmount,
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

        // daily collection remittance API List
        [Authorize]
        [HttpGet]
        [Route("api/collections/list/dailyCollectionRemittance/{startDate}/{endDate}")]
        public List<Models.RepDailyCollectionRemittance> listDailyCollectionRemittance(String startDate, String endDate)
        {
            var dailyCollectionRemittanceList = from d in db.mstAreas.OrderBy(d => d.Area)
                                                join c in db.trnCollections
                                                on d.Id equals c.trnLoan.mstApplicant.AreaId
                                                into joinAreaCollections
                                                from listAreaCollections in joinAreaCollections.DefaultIfEmpty().GroupBy(c => c.trnLoan.mstApplicant.AreaId)
                                                join r in db.trnRemittances
                                                on d.Id equals r.AreaId
                                                into joinAreaRemittances
                                                from listAreaRemittances in joinAreaRemittances.DefaultIfEmpty().GroupBy(r => r.AreaId)
                                                select new Models.RepDailyCollectionRemittance
                                                {
                                                    Area = d.Area,
                                                    GrossCollection = joinAreaCollections.Where(c => c.CollectionDate >= Convert.ToDateTime(startDate) && c.CollectionDate <= Convert.ToDateTime(endDate) && c.IsLocked == true).Sum(c => c.TotalPaidAmount) != null ? joinAreaCollections.Where(c => c.CollectionDate >= Convert.ToDateTime(startDate) && c.CollectionDate <= Convert.ToDateTime(endDate) && c.IsLocked == true).Sum(c => c.TotalPaidAmount) : 0,
                                                    NetRemitted = joinAreaRemittances.Where(r => r.RemittanceDate >= Convert.ToDateTime(startDate) && r.RemittanceDate <= Convert.ToDateTime(endDate) && r.IsLocked == true).Sum(r => r.RemitAmount) != null ? joinAreaRemittances.Where(r => r.RemittanceDate >= Convert.ToDateTime(startDate) && r.RemittanceDate <= Convert.ToDateTime(endDate) && r.IsLocked == true).Sum(r => r.RemitAmount) : 0,
                                                    Remarks = " "
                                                };

            return dailyCollectionRemittanceList.ToList();
        }
    }
}
