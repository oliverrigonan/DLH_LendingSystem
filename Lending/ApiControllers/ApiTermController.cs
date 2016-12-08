using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiTermController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // Terms list
        [Authorize]
        [HttpGet]
        [Route("api/term/list")]
        public List<Models.MstTerm> listTerm()
        {
            var term = from d in db.mstTerms
                            select new Models.MstTerm
                            {
                                Id = d.Id,
                                Term = d.Term,
                                Description = d.Description,
                                NoOfDays = d.NoOfDays,
                                CreatedByUserId = d.CreatedByUserId,
                                CreatedByUser = d.mstUser.FullName,
                                CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                UpdatedByUserId = d.UpdatedByUserId,
                                UpdatedByUser = d.mstUser1.FullName,
                                UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                            };

            return term.ToList();
        }

        // add Terms 
        [Authorize]
        [HttpPost]
        [Route("api/term/add")]
        public HttpResponseMessage addTerm(Models.MstTerm loanType)
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
                    String matchPageString = "SystemTables";
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
                        Data.mstTerm newTerm = new Data.mstTerm();

                        newTerm.Term = loanType.Term;
                        newTerm.Description = loanType.Description;
                        newTerm.NoOfDays = loanType.NoOfDays;
                        newTerm.CreatedByUserId = userId;
                        newTerm.CreatedDateTime = DateTime.Now;
                        newTerm.UpdatedByUserId = userId;
                        newTerm.UpdatedDateTime = DateTime.Now;

                        db.mstTerms.InsertOnSubmit(newTerm);
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
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // update Terms 
        [Authorize]
        [HttpPut]
        [Route("api/term/update/{id}")]
        public HttpResponseMessage updateTerm(String id, Models.MstTerm loanType)
        {
            try
            {
                var term = from d in db.mstTerms where d.Id == Convert.ToInt32(id) select d;
                if (term.Any())
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
                        String matchPageString = "SystemTables";
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
                            var updateTerm = term.FirstOrDefault();

                            updateTerm.Term = loanType.Term;
                            updateTerm.Description = loanType.Description;
                            updateTerm.NoOfDays = loanType.NoOfDays;
                            updateTerm.UpdatedByUserId = userId;
                            updateTerm.UpdatedDateTime = DateTime.Now;
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

        // delete Terms
        [Authorize]
        [HttpDelete]
        [Route("api/term/delete/{id}")]
        public HttpResponseMessage deleteTerm(String id)
        {
            try
            {
                var term = from d in db.mstTerms where d.Id == Convert.ToInt32(id) select d;
                if (term.Any())
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
                        String matchPageString = "SystemTables";
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
                            db.mstTerms.DeleteOnSubmit(term.First());
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
