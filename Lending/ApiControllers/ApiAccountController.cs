using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiAccountController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // account list
        [Authorize]
        [HttpGet]
        [Route("api/account/list")]
        public List<Models.MstAccount> listAccount()
        {
            var accounts = from d in db.mstAccounts.OrderByDescending(d => d.Id)
                        select new Models.MstAccount
                        {
                            Id = d.Id,
                            Account = d.Account,
                            Description = d.Description,
                            AccountTransactionTypeId = d.AccountTransactionTypeId,
                            AccountTransactionType = d.sysTransactionType.TransactionType,
                            CreatedByUserId = d.CreatedByUserId,
                            CreatedByUser = d.mstUser.FullName,
                            CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                            UpdatedByUserId = d.UpdatedByUserId,
                            UpdatedByUser = d.mstUser1.FullName,
                            UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                        };

            return accounts.ToList();
        }

        // account list by Account Transaction Type Id
        [Authorize]
        [HttpGet]
        [Route("api/account/listAccountTransactionTypeId/{accountTransactionTypeId}")]
        public List<Models.MstAccount> listAccountByAccountTransactionTypeId(String accountTransactionTypeId)
        {
            var accounts = from d in db.mstAccounts
                           where d.AccountTransactionTypeId == Convert.ToInt32(accountTransactionTypeId)
                           select new Models.MstAccount
                           {
                               Id = d.Id,
                               Account = d.Account,
                               Description = d.Description,
                               AccountTransactionTypeId = d.AccountTransactionTypeId,
                               AccountTransactionType = d.sysTransactionType.TransactionType,
                               CreatedByUserId = d.CreatedByUserId,
                               CreatedByUser = d.mstUser.FullName,
                               CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                               UpdatedByUserId = d.UpdatedByUserId,
                               UpdatedByUser = d.mstUser1.FullName,
                               UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                           };

            return accounts.ToList();
        }

        // add account
        [Authorize]
        [HttpPost]
        [Route("api/account/add")]
        public HttpResponseMessage addAccount(Models.MstAccount account)
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                Data.mstAccount newAccount = new Data.mstAccount();
                newAccount.Account = account.Account;
                newAccount.Description = account.Description;
                newAccount.AccountTransactionTypeId = account.AccountTransactionTypeId;
                newAccount.CreatedByUserId = userId;
                newAccount.CreatedDateTime = DateTime.Now;
                newAccount.UpdatedByUserId = userId;
                newAccount.UpdatedDateTime = DateTime.Now;

                db.mstAccounts.InsertOnSubmit(newAccount);
                db.SubmitChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // update account
        [Authorize]
        [HttpPut]
        [Route("api/account/update/{id}")]
        public HttpResponseMessage updateAccount(String id, Models.MstAccount account)
        {
            try
            {
                var accounts = from d in db.mstAccounts where d.Id == Convert.ToInt32(id) select d;
                if (accounts.Any())
                {
                    var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                    var updateAccount = accounts.FirstOrDefault();
                    updateAccount.Account = account.Account;
                    updateAccount.Description = account.Description;
                    updateAccount.AccountTransactionTypeId = account.AccountTransactionTypeId;
                    updateAccount.UpdatedByUserId = userId;
                    updateAccount.UpdatedDateTime = DateTime.Now;

                    db.SubmitChanges();

                    return Request.CreateResponse(HttpStatusCode.OK);
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

        // delete account
        [Authorize]
        [HttpDelete]
        [Route("api/account/delete/{id}")]
        public HttpResponseMessage deleteAccount(String id)
        {
            try
            {
                var accounts = from d in db.mstAccounts where d.Id == Convert.ToInt32(id) select d;
                if (accounts.Any())
                {
                    db.mstAccounts.DeleteOnSubmit(accounts.First());
                    db.SubmitChanges();

                    return Request.CreateResponse(HttpStatusCode.OK);
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
