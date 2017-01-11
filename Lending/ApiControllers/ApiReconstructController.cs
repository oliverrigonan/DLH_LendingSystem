using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiReconstructController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // reconstruct list by reconstruct date
        [Authorize]
        [HttpGet]
        [Route("api/reconstruct/list/byReconstructDate/{reconstructDate}")]
        public List<Models.TrnReconstruct> listReconstructByReconstructDate(String reconstructDate)
        {
            var reconstructs = from d in db.trnReconstructs
                               where d.ReconstructDate == Convert.ToDateTime(reconstructDate)
                               select new Models.TrnReconstruct
                               {
                                   Id = d.Id,
                                   ReconstructNumber = d.ReconstructNumber,
                                   ReconstructDate = d.ReconstructDate.ToShortDateString(),
                                   MaturityDate = d.MaturityDate.ToShortDateString(),
                                   Particulars = d.Particulars,
                                   LoanId = d.LoanId,
                                   LoanDetail = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " ") + " - " + d.trnLoan.LoanNumber + " (from " + d.trnLoan.LoanDate + " to " + d.trnLoan.MaturityDate + ")",
                                   LoanBalanceAmount = d.LoanBalanceAmount,
                                   PreviousLoanEndDate = d.PreviousLoanEndDate.ToShortDateString(),
                                   InterestId = d.InterestId,
                                   InterestRate = d.InterestRate,
                                   InterestAmount = d.InterestAmount,
                                   ReconstructAmount = d.ReconstructAmount,
                                   PreparedByUserId = d.PreparedByUserId,
                                   PreparedByUser = d.mstUser.FullName,
                                   IsLocked = d.IsLocked,
                                   CreatedByUserId = d.CreatedByUserId,
                                   CreatedByUser = d.mstUser1.FullName,
                                   CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                   UpdatedByUserId = d.UpdatedByUserId,
                                   UpdatedByUser = d.mstUser2.FullName,
                                   UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                               };

            return reconstructs.ToList();
        }

        // reconstruct list by id
        [Authorize]
        [HttpGet]
        [Route("api/reconstruct/get/byId/{id}")]
        public Models.TrnReconstruct getReconstructById(String id)
        {
            var reconstruct = from d in db.trnReconstructs
                              where d.Id == Convert.ToInt32(id)
                              select new Models.TrnReconstruct
                              {
                                  Id = d.Id,
                                  ReconstructNumber = d.ReconstructNumber,
                                  ReconstructDate = d.ReconstructDate.ToShortDateString(),
                                  MaturityDate = d.MaturityDate.ToShortDateString(),
                                  Particulars = d.Particulars,
                                  LoanId = d.LoanId,
                                  LoanDetail = d.trnLoan.mstApplicant.ApplicantLastName + ", " + d.trnLoan.mstApplicant.ApplicantFirstName + " " + (d.trnLoan.mstApplicant.ApplicantMiddleName != null ? d.trnLoan.mstApplicant.ApplicantMiddleName : " ") + " - " + d.trnLoan.LoanNumber + " (from " + d.trnLoan.LoanDate + " to " + d.trnLoan.MaturityDate + ")",
                                  LoanBalanceAmount = d.LoanBalanceAmount,
                                  PreviousLoanEndDate = d.PreviousLoanEndDate.ToShortDateString(),
                                  InterestId = d.InterestId,
                                  InterestRate = d.InterestRate,
                                  InterestAmount = d.InterestAmount,
                                  ReconstructAmount = d.ReconstructAmount,
                                  PreparedByUserId = d.PreparedByUserId,
                                  PreparedByUser = d.mstUser.FullName,
                                  IsLocked = d.IsLocked,
                                  CreatedByUserId = d.CreatedByUserId,
                                  CreatedByUser = d.mstUser1.FullName,
                                  CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                  UpdatedByUserId = d.UpdatedByUserId,
                                  UpdatedByUser = d.mstUser2.FullName,
                                  UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                              };

            return (Models.TrnReconstruct)reconstruct.FirstOrDefault();
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

        // add reconstruct
        [Authorize]
        [HttpPost]
        [Route("api/reconstruct/add")]
        public Int32 addReconstruct()
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
                    String matchPageString = "LoanApplicationList";
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
                        String reconstructNumber = "0000000001";
                        var reconstruct = from d in db.trnReconstructs.OrderByDescending(d => d.Id) select d;
                        if (reconstruct.Any())
                        {
                            var newReconstructNumber = Convert.ToInt32(reconstruct.FirstOrDefault().ReconstructNumber) + 0000000001;
                            reconstructNumber = newReconstructNumber.ToString();
                        }

                        var loan = from d in db.trnLoans where d.IsLocked == true select d;
                        if (loan.Any())
                        {
                            var collection = from d in db.trnCollections where d.LoanId == loan.FirstOrDefault().Id select d;
                            if (collection.Any())
                            {
                                var interest = from d in db.mstInterests select d;

                                Decimal interestAmount = (loan.FirstOrDefault().BalanceAmount / 100) * interest.FirstOrDefault().Rate;
                                Decimal reconstructAmount = loan.FirstOrDefault().BalanceAmount + interestAmount;

                                Data.trnReconstruct newReconsruct = new Data.trnReconstruct();
                                newReconsruct.ReconstructNumber = zeroFill(Convert.ToInt32(reconstructNumber), 10);
                                newReconsruct.ReconstructDate = DateTime.Today;
                                newReconsruct.MaturityDate = DateTime.Today.AddDays(60);
                                newReconsruct.Particulars = "NA";
                                newReconsruct.LoanId = loan.FirstOrDefault().Id;
                                newReconsruct.LoanBalanceAmount = loan.FirstOrDefault().BalanceAmount;
                                newReconsruct.PreviousLoanEndDate = loan.FirstOrDefault().LoanEndDate;
                                newReconsruct.InterestId = interest.FirstOrDefault().Id;
                                newReconsruct.InterestRate = interest.FirstOrDefault().Rate;
                                newReconsruct.InterestAmount = interestAmount;
                                newReconsruct.ReconstructAmount = reconstructAmount;
                                newReconsruct.PreparedByUserId = userId;
                                newReconsruct.IsLocked = false;
                                newReconsruct.CreatedByUserId = userId;
                                newReconsruct.CreatedDateTime = DateTime.Now;
                                newReconsruct.UpdatedByUserId = userId;
                                newReconsruct.UpdatedDateTime = DateTime.Now;
                                db.trnReconstructs.InsertOnSubmit(newReconsruct);
                                db.SubmitChanges();

                                return newReconsruct.Id;
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

        // lock reconstruct
        [Authorize]
        [HttpPut]
        [Route("api/reconstruct/lock/{id}")]
        public HttpResponseMessage lockReconstruct(String id, Models.TrnReconstruct reconstruct)
        {
            try
            {
                var reconstructs = from d in db.trnReconstructs where d.Id == Convert.ToInt32(id) select d;
                if (reconstructs.Any())
                {
                    if (!reconstructs.FirstOrDefault().IsLocked)
                    {
                        var collection = from d in db.trnCollections
                                         where d.LoanId == reconstructs.FirstOrDefault().LoanId
                                         select d;

                        if (!collection.Any())
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
                                    if (Convert.ToDateTime(reconstruct.ReconstructDate) > Convert.ToDateTime(reconstruct.MaturityDate))
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                                    }
                                    else
                                    {
                                        var loan = from d in db.trnLoans where d.Id == reconstruct.LoanId select d;
                                        var interest = from d in db.mstInterests select d;

                                        Decimal interestAmount = (loan.FirstOrDefault().BalanceAmount / 100) * interest.FirstOrDefault().Rate;
                                        Decimal reconstructAmount = loan.FirstOrDefault().BalanceAmount + interestAmount;

                                        var lockReconstruct = reconstructs.FirstOrDefault();
                                        lockReconstruct.ReconstructDate = Convert.ToDateTime(reconstruct.ReconstructDate);
                                        lockReconstruct.MaturityDate = Convert.ToDateTime(reconstruct.MaturityDate);
                                        lockReconstruct.Particulars = reconstruct.Particulars;
                                        lockReconstruct.LoanId = loan.FirstOrDefault().Id;
                                        lockReconstruct.LoanBalanceAmount = loan.FirstOrDefault().BalanceAmount;
                                        lockReconstruct.PreviousLoanEndDate = loan.FirstOrDefault().LoanEndDate;
                                        lockReconstruct.InterestId = interest.FirstOrDefault().Id;
                                        lockReconstruct.InterestRate = interest.FirstOrDefault().Rate;
                                        lockReconstruct.InterestAmount = interestAmount;
                                        lockReconstruct.ReconstructAmount = reconstructAmount;
                                        lockReconstruct.PreparedByUserId = userId;
                                        lockReconstruct.IsLocked = true;
                                        lockReconstruct.UpdatedByUserId = userId;
                                        lockReconstruct.UpdatedDateTime = DateTime.Now;
                                        db.SubmitChanges();

                                        Business.UpdateLoan updateLoan = new Business.UpdateLoan();
                                        updateLoan.updateLoan(loan.FirstOrDefault().Id);

                                        return Request.CreateResponse(HttpStatusCode.OK);
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

        // unlock reconstruct
        [Authorize]
        [HttpPut]
        [Route("api/reconstruct/unlock/{id}")]
        public HttpResponseMessage unlockReconstruct(String id)
        {
            try
            {
                var reconstructs = from d in db.trnReconstructs where d.Id == Convert.ToInt32(id) select d;
                if (reconstructs.Any())
                {
                    if (reconstructs.FirstOrDefault().IsLocked)
                    {
                        var collection = from d in db.trnCollections
                                         where d.LoanId == reconstructs.FirstOrDefault().LoanId
                                         select d;

                        if (!collection.Any())
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
                                    var unlockReconstruct = reconstructs.FirstOrDefault();
                                    unlockReconstruct.IsLocked = false;
                                    unlockReconstruct.UpdatedByUserId = userId;
                                    unlockReconstruct.UpdatedDateTime = DateTime.Now;
                                    db.SubmitChanges();

                                    Business.UpdateLoan updateLoan = new Business.UpdateLoan();
                                    updateLoan.updateLoan(reconstructs.FirstOrDefault().LoanId);

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

        // delete reconstruct
        [Authorize]
        [HttpDelete]
        [Route("api/reconstruct/delete/{id}")]
        public HttpResponseMessage deleteReconstruct(String id)
        {
            try
            {
                var reconstructs = from d in db.trnReconstructs where d.Id == Convert.ToInt32(id) select d;
                if (reconstructs.Any())
                {
                    if (!reconstructs.FirstOrDefault().IsLocked)
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
                                Int32 loanId = reconstructs.FirstOrDefault().LoanId;

                                db.trnReconstructs.DeleteOnSubmit(reconstructs.First());
                                db.SubmitChanges();

                                Business.UpdateLoan updateLoan = new Business.UpdateLoan();
                                updateLoan.updateLoan(loanId);

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
