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
                                  CollectionStatusId = d.StatusId,
                                  CollectionStatus = d.sysCollectionStatus.Status,
                                  Particulars = d.Particulars,
                                  TotalPaidAmount = d.TotalPaidAmount,
                                  TotalPenaltyAmount = d.TotalPenaltyAmount,
                                  CollectorStaffId = d.CollectorStaffId,
                                  CollectorStaff = d.mstStaff.Staff,
                                  PreparedByUserId = d.PreparedByUserId,
                                  PreparedByUser = d.mstUser.FullName,
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
                                 CollectionStatusId = d.StatusId,
                                 CollectionStatus = d.sysCollectionStatus.Status,
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
                            var staffs = from d in db.mstStaffs.OrderByDescending(d => d.Id) select d;
                            if (staffs.Any())
                            {
                                var collectionStatus = from d in db.sysCollectionStatus
                                                       select d;

                                if (collectionStatus.Any())
                                {
                                    Data.trnCollection newCollection = new Data.trnCollection();
                                    newCollection.CollectionNumber = zeroFill(Convert.ToInt32(collectionNumber), 10);
                                    newCollection.CollectionDate = DateTime.Today;
                                    newCollection.LoanId = loan.FirstOrDefault().Id;
                                    newCollection.StatusId = collectionStatus.FirstOrDefault().Id;
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
        public void updateLoan(Int32 loanId)
        {
            var collection = from d in db.trnCollections
                             where d.LoanId == loanId
                             && d.IsLocked == true
                             select d;

            Decimal TotalPaidAmount = 0;
            Decimal TotalPenaltyAmount = 0;
            if (collection.Any())
            {
                TotalPaidAmount = collection.Sum(d => d.TotalPaidAmount);
                TotalPenaltyAmount = collection.Sum(d => d.TotalPenaltyAmount);
            }

            var loan = from d in db.trnLoans where d.Id == loanId select d;
            if (loan.Any())
            {
                var updateLoan = loan.FirstOrDefault();
                updateLoan.TotalPaidAmount = TotalPaidAmount;
                updateLoan.TotalPenaltyAmount = TotalPenaltyAmount;
                updateLoan.TotalBalanceAmount = (loan.FirstOrDefault().NetCollectionAmount - TotalPaidAmount) + TotalPenaltyAmount;
                db.SubmitChanges();
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
                                var loan = from d in db.trnLoans
                                           where d.Id == collection.LoanId
                                           select d;

                                if (loan.Any())
                                {
                                    if (loan.FirstOrDefault().IsLocked)
                                    {
                                        var lockCollection = collections.FirstOrDefault();
                                        lockCollection.CollectionDate = Convert.ToDateTime(collection.CollectionDate);
                                        lockCollection.LoanId = collection.LoanId;
                                        lockCollection.StatusId = collection.CollectionStatusId;
                                        lockCollection.Particulars = collection.Particulars;
                                        lockCollection.CollectorStaffId = collection.CollectorStaffId;
                                        lockCollection.PreparedByUserId = collection.PreparedByUserId;
                                        lockCollection.TotalPaidAmount = collection.TotalPaidAmount;
                                        lockCollection.TotalPenaltyAmount = collection.TotalPenaltyAmount;
                                        lockCollection.IsLocked = true;
                                        lockCollection.UpdatedByUserId = userId;
                                        lockCollection.UpdatedDateTime = DateTime.Now;
                                        db.SubmitChanges();
                                        this.updateLoan(Convert.ToInt32(collection.LoanId));

                                        return Request.CreateResponse(HttpStatusCode.OK);
                                    }
                                    else
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot pay unlocked loan record.");
                                    }
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. Invalid loan lecord.");
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
                                var unlockCollection = collections.FirstOrDefault();
                                unlockCollection.IsLocked = false;
                                unlockCollection.UpdatedByUserId = userId;
                                unlockCollection.UpdatedDateTime = DateTime.Now;
                                db.SubmitChanges();
                                this.updateLoan(Convert.ToInt32(collections.FirstOrDefault().LoanId));

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to unlock record.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to unlock record.");
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
                                var loanId = Convert.ToInt32(collections.FirstOrDefault().LoanId);

                                db.trnCollections.DeleteOnSubmit(collections.First());
                                db.SubmitChanges();

                                this.updateLoan(loanId);

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
        public List<Models.TrnCollection> listCollectionByApplicantIdAndLoanId(String applicantId, String loanId)
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
                                  CollectionStatus = d.sysCollectionStatus.Status,
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
            if (areaId.Equals("0"))
            {
                var collections = from d in db.trnCollections
                                  where d.CollectionDate >= Convert.ToDateTime(startCollectionDate)
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
                                      CollectionStatusId = d.StatusId,
                                      CollectionStatus = d.sysCollectionStatus.Status,
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
                                      UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                      Area = d.trnLoan.mstApplicant.mstArea.Area,
                                  };

                return collections.ToList();
            }
            else
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
                                      CollectionStatusId = d.StatusId,
                                      CollectionStatus = d.sysCollectionStatus.Status,
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
                                      UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                      Area = d.trnLoan.mstApplicant.mstArea.Area,
                                  };

                return collections.ToList();
            }
        }

        // collection list - overdue
        [Authorize]
        [HttpGet]
        [Route("api/collections/list/ByCollectionDate/overdue/{areaId}/{startCollectionDate}/{endCollectionDate}")]
        public List<Models.TrnCollection> listCollectionByCollectionDateOverdue(String areaId, String startCollectionDate, String endCollectionDate)
        {
            if (areaId.Equals("0"))
            {
                var collections = from d in db.trnCollections
                                  where d.CollectionDate >= Convert.ToDateTime(startCollectionDate)
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
                                      CollectionStatusId = d.StatusId,
                                      CollectionStatus = d.sysCollectionStatus.Status,
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
                                      UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                      Area = d.trnLoan.mstApplicant.mstArea.Area,
                                  };

                return collections.ToList();
            }
            else
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
                                      CollectionStatusId = d.StatusId,
                                      CollectionStatus = d.sysCollectionStatus.Status,
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
                                      UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(),
                                      Area = d.trnLoan.mstApplicant.mstArea.Area,
                                  };

                return collections.ToList();
            }
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

        // total collection list - active and overdue
        [Authorize]
        [HttpGet]
        [Route("api/totalcollections/list/ByCollectionDate/{startCollectionDate}/{endCollectionDate}")]
        public List<Models.TrnCollection> listTotalCollectionByCollectionDate(String startCollectionDate, String endCollectionDate)
        {
            var collections = from d in db.mstAreas
                              join c in db.trnCollections
                              on d.Id equals c.trnLoan.mstApplicant.AreaId
                              into joinAreaCollections
                              from listAreaCollections in joinAreaCollections.DefaultIfEmpty()
                              select new Models.TrnCollection
                              {
                                  AreaId = d.Id,
                                  Area = d.Area,
                                  Active = joinAreaCollections.Where(a => a.CollectionDate >= Convert.ToDateTime(startCollectionDate)
                                      && a.CollectionDate <= Convert.ToDateTime(endCollectionDate)
                                      && a.IsLocked == true
                                      && a.trnLoan.IsLoanReconstruct != true).Sum(a => a.TotalPaidAmount) != null ? joinAreaCollections.Where(a => a.CollectionDate >= Convert.ToDateTime(startCollectionDate)
                                      && a.CollectionDate <= Convert.ToDateTime(endCollectionDate)
                                      && a.IsLocked == true
                                      && a.trnLoan.IsLoanReconstruct != true).Sum(a => a.TotalPaidAmount) : 0,
                                  Overdue = joinAreaCollections.Where(a => a.CollectionDate >= Convert.ToDateTime(startCollectionDate)
                                      && a.CollectionDate <= Convert.ToDateTime(endCollectionDate)
                                      && a.IsLocked == true
                                      && a.trnLoan.IsLoanReconstruct == true).Sum(a => a.TotalPaidAmount) != null ? joinAreaCollections.Where(a => a.CollectionDate >= Convert.ToDateTime(startCollectionDate)
                                      && a.CollectionDate <= Convert.ToDateTime(endCollectionDate)
                                      && a.IsLocked == true
                                      && a.trnLoan.IsLoanReconstruct == true).Sum(a => a.TotalPaidAmount) : 0
                              };

            var collectionsGroupByArea = from d in collections
                                         group d by new
                                         {
                                             AreaId = d.AreaId,
                                             Area = d.Area,
                                             Active = d.Active,
                                             Overdue = d.Overdue
                                         } into g
                                         select new Models.TrnCollection
                                         {
                                             AreaId = g.Key.AreaId,
                                             Area = g.Key.Area,
                                             Active = g.Key.Active,
                                             Overdue = g.Key.Overdue,
                                             TotalCollection = Convert.ToDecimal(g.Key.Active) + Convert.ToDecimal(g.Key.Overdue)
                                         };

            return collectionsGroupByArea.OrderBy(d => d.Area).ToList();
        }

        // collection list
        [Authorize]
        [HttpGet]
        [Route("api/collections/list/ByLoanId/{loanId}")]
        public List<Models.TrnCollection> listCollectionByLoanId(String loanId)
        {
            var collections = from d in db.trnCollections.OrderByDescending(d => d.Id)
                              where d.LoanId == Convert.ToInt32(loanId)
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
                                  CollectionStatus = d.sysCollectionStatus.Status,
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

    }
}
