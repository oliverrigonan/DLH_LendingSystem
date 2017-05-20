using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiDeductionsController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // deductions list
        [Authorize]
        [HttpGet]
        [Route("api/deductions/list")]
        public List<Models.MstDeductions> listDeductions()
        {
            var deductions = from d in db.mstDeductions.OrderByDescending(d => d.Id)
                           select new Models.MstDeductions
                           {
                               Id = d.Id,
                               Deduction = d.Deduction,
                               Description = d.Description,
                               PercentageRate = d.PercentageRate,
                               DeductionAmount = d.DeductionAmount,
                               CreatedByUserId = d.CreatedByUserId,
                               CreatedByUser = d.mstUser.FullName,
                               CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                               UpdatedByUserId = d.UpdatedByUserId,
                               UpdatedByUser = d.mstUser1.FullName,
                               UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                           };

            return deductions.ToList();
        }

        // add deductions 
        [Authorize]
        [HttpPost]
        [Route("api/deductions/add")]
        public HttpResponseMessage addDeductions(Models.MstDeductions deductions)
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
                        Data.mstDeduction newDeductions = new Data.mstDeduction();
                        newDeductions.Deduction = deductions.Deduction;
                        newDeductions.Description = deductions.Description;
                        newDeductions.PercentageRate = deductions.PercentageRate;
                        newDeductions.DeductionAmount = deductions.DeductionAmount;
                        newDeductions.CreatedByUserId = userId;
                        newDeductions.CreatedDateTime = DateTime.Now;
                        newDeductions.UpdatedByUserId = userId;
                        newDeductions.UpdatedDateTime = DateTime.Now;
                        db.mstDeductions.InsertOnSubmit(newDeductions);
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

        // update deductions 
        [Authorize]
        [HttpPut]
        [Route("api/deductions/update/{id}")]
        public HttpResponseMessage updateDeductions(String id, Models.MstDeductions deduction)
        {
            try
            {
                var deductions = from d in db.mstDeductions where d.Id == Convert.ToInt32(id) select d;
                if (deductions.Any())
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
                            var updateDeductions = deductions.FirstOrDefault();
                            updateDeductions.Deduction = deduction.Deduction;
                            updateDeductions.Description = deduction.Description;
                            updateDeductions.PercentageRate = deduction.PercentageRate;
                            updateDeductions.DeductionAmount = deduction.DeductionAmount;
                            updateDeductions.UpdatedByUserId = userId;
                            updateDeductions.UpdatedDateTime = DateTime.Now;
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

        // delete deductions
        [Authorize]
        [HttpDelete]
        [Route("api/deductions/delete/{id}")]
        public HttpResponseMessage deleteDeductions(String id)
        {
            try
            {
                var deductions = from d in db.mstDeductions where d.Id == Convert.ToInt32(id) select d;
                if (deductions.Any())
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
                            db.mstDeductions.DeleteOnSubmit(deductions.First());
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
