using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiDisbursementController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // disbursement list
        [Authorize]
        [HttpGet]
        [Route("api/disbursement/listByDisbursementDate/{disbursementDate}")]
        public List<Models.TrnDisbursement> listDisbursementByDisbursementDate(String disbursementDate)
        {
            var disbursements = from d in db.trnDisbursements.OrderByDescending(d => d.Id)
                                where d.DisbursementDate == Convert.ToDateTime(disbursementDate)
                                select new Models.TrnDisbursement
                                {
                                    Id = d.Id,
                                    DisbursementNumber = d.DisbursementNumber,
                                    DisbursementDate = d.DisbursementDate.ToShortDateString(),
                                    BranchId = d.BranchId,
                                    Branch = d.mstBranch.Branch,
                                    AccountId = d.AccountId,
                                    Account = d.mstAccount.Account,
                                    Payee = d.Payee,
                                    Particulars = d.Particulars,
                                    Amount = d.Amount,
                                    PreparedByUserId = d.PreparedByUserId,
                                    PreparedByUser = d.mstUser.FullName,
                                    VerifiedByUserId = d.VerifiedByUserId,
                                    VerifiedByUser = d.mstUser1.FullName,
                                    IsLocked = d.IsLocked,
                                    CreatedByUserId = d.CreatedByUserId,
                                    CreatedByUser = d.mstUser2.FullName,
                                    CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                    UpdatedByUserId = d.UpdatedByUserId,
                                    UpdatedByUser = d.mstUser3.FullName,
                                    UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                                };

            return disbursements.ToList();
        }

        // disbursement by id
        [Authorize]
        [HttpGet]
        [Route("api/disbursement/getById/{id}")]
        public Models.TrnDisbursement getDisbursementById(String id)
        {
            var disbursement = from d in db.trnDisbursements
                               where d.Id == Convert.ToInt32(id)
                               select new Models.TrnDisbursement
                               {
                                   Id = d.Id,
                                   DisbursementNumber = d.DisbursementNumber,
                                   DisbursementDate = d.DisbursementDate.ToShortDateString(),
                                   BranchId = d.BranchId,
                                   Branch = d.mstBranch.Branch,
                                   AccountId = d.AccountId,
                                   Account = d.mstAccount.Account,
                                   Payee = d.Payee,
                                   Particulars = d.Particulars,
                                   Amount = d.Amount,
                                   PreparedByUserId = d.PreparedByUserId,
                                   PreparedByUser = d.mstUser.FullName,
                                   VerifiedByUserId = d.VerifiedByUserId,
                                   VerifiedByUser = d.mstUser1.FullName,
                                   IsLocked = d.IsLocked,
                                   CreatedByUserId = d.CreatedByUserId,
                                   CreatedByUser = d.mstUser2.FullName,
                                   CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                   UpdatedByUserId = d.UpdatedByUserId,
                                   UpdatedByUser = d.mstUser3.FullName,
                                   UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                               };

            return (Models.TrnDisbursement)disbursement.FirstOrDefault();
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

        // add disbursement
        [Authorize]
        [HttpPost]
        [Route("api/disbursement/add")]
        public Int32 addDisbursement()
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                String disbursementNumber = "0000000001";
                var disbursement = from d in db.trnDisbursements.OrderByDescending(d => d.Id) select d;
                if (disbursement.Any())
                {
                    var newDisbursementNumber = Convert.ToInt32(disbursement.FirstOrDefault().DisbursementNumber) + 0000000001;
                    disbursementNumber = newDisbursementNumber.ToString();
                }

                Data.trnDisbursement newDisbursement = new Data.trnDisbursement();
                newDisbursement.DisbursementNumber = zeroFill(Convert.ToInt32(disbursementNumber), 10);
                newDisbursement.DisbursementDate = DateTime.Today;
                newDisbursement.BranchId = (from d in db.mstBranches select d.Id).FirstOrDefault();
                newDisbursement.AccountId = (from d in db.mstAccounts where d.AccountTransactionTypeId == 3 select d.Id).FirstOrDefault();
                newDisbursement.Payee = "NA";
                newDisbursement.Particulars = "NA";
                newDisbursement.Amount = 0;
                newDisbursement.PreparedByUserId = userId;
                newDisbursement.VerifiedByUserId = userId;
                newDisbursement.IsLocked = false;
                newDisbursement.CreatedByUserId = userId;
                newDisbursement.CreatedDateTime = DateTime.Now;
                newDisbursement.UpdatedByUserId = userId;
                newDisbursement.UpdatedDateTime = DateTime.Now;

                db.trnDisbursements.InsertOnSubmit(newDisbursement);
                db.SubmitChanges();

                return newDisbursement.Id;
            }
            catch
            {
                return 0;
            }
        }

        // lock disbursement
        [Authorize]
        [HttpPut]
        [Route("api/disbursement/lock/{id}")]
        public HttpResponseMessage lockDisbursement(String id, Models.TrnDisbursement disbursement)
        {
            try
            {
                var disbursements = from d in db.trnDisbursements where d.Id == Convert.ToInt32(id) select d;
                if (disbursements.Any())
                {
                    if (!disbursements.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                        var lockDisbursement = disbursements.FirstOrDefault();
                        lockDisbursement.DisbursementDate = Convert.ToDateTime(disbursement.DisbursementDate);
                        lockDisbursement.BranchId = disbursement.BranchId;
                        lockDisbursement.AccountId = disbursement.AccountId;
                        lockDisbursement.Payee = disbursement.Payee;
                        lockDisbursement.Particulars = disbursement.Particulars;
                        lockDisbursement.Amount = disbursement.Amount;
                        lockDisbursement.PreparedByUserId = disbursement.PreparedByUserId;
                        lockDisbursement.VerifiedByUserId = disbursement.VerifiedByUserId;
                        lockDisbursement.IsLocked = true;
                        lockDisbursement.UpdatedByUserId = userId;
                        lockDisbursement.UpdatedDateTime = DateTime.Now;
                        db.SubmitChanges();

                        Business.Journal journal = new Business.Journal();
                        journal.postDisbursementJournal(Convert.ToInt32(id));

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

        // unlock disbursement
        [Authorize]
        [HttpPut]
        [Route("api/disbursement/unlock/{id}")]
        public HttpResponseMessage unlockDisbursement(String id)
        {
            try
            {
                var disbursements = from d in db.trnDisbursements where d.Id == Convert.ToInt32(id) select d;
                if (disbursements.Any())
                {
                    if (disbursements.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                        var unlockDisbursement = disbursements.FirstOrDefault();
                        unlockDisbursement.IsLocked = false;
                        unlockDisbursement.UpdatedByUserId = userId;
                        unlockDisbursement.UpdatedDateTime = DateTime.Now;
                        db.SubmitChanges();

                        Business.Journal journal = new Business.Journal();
                        journal.deleteDisbursementJournal(Convert.ToInt32(id));

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

        // delete disbursement
        [Authorize]
        [HttpDelete]
        [Route("api/disbursement/delete/{id}")]
        public HttpResponseMessage deleteDisbursement(String id)
        {
            try
            {
                var disbursements = from d in db.trnDisbursements where d.Id == Convert.ToInt32(id) select d;
                if (disbursements.Any())
                {
                    if (!disbursements.FirstOrDefault().IsLocked)
                    {
                        db.trnDisbursements.DeleteOnSubmit(disbursements.First());
                        db.SubmitChanges();

                        Business.Journal journal = new Business.Journal();
                        journal.deleteDisbursementJournal(Convert.ToInt32(id));

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
