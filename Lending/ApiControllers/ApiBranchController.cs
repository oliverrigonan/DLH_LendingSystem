using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiBranchController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // branch list
        [Authorize]
        [HttpGet]
        [Route("api/branch/list")]
        public List<Models.MstBranch> listBranch()
        {
            var branches = from d in db.mstBranches
                           select new Models.MstBranch
                           {
                               Id = d.Id,
                               CompanyId = d.CompanyId,
                               Company = d.mstCompany.Company,
                               Branch = d.Branch,
                               Address = d.Address,
                               ContactNumber = d.ContactNumber,
                           };

            return branches.ToList();
        }

        // branch list
        [Authorize]
        [HttpGet]
        [Route("api/branch/listByCompanyId/{companyId}")]
        public List<Models.MstBranch> listBranchByCompanyId(String companyId)
        {
            var branches = from d in db.mstBranches
                           where d.CompanyId == Convert.ToInt32(companyId)
                           select new Models.MstBranch
                           {
                               Id = d.Id,
                               CompanyId = d.CompanyId,
                               Company = d.mstCompany.Company,
                               Branch = d.Branch,
                               Address = d.Address,
                               ContactNumber = d.ContactNumber,
                           };

            return branches.ToList();
        }

        // add branch
        [Authorize]
        [HttpPost]
        [Route("api/branch/add")]
        public HttpResponseMessage addBranch(Models.MstBranch branch)
        {
            try
            {
                var companies = from d in db.mstCompanies where d.Id == branch.CompanyId select d;
                if (companies.Any())
                {
                    if (!companies.FirstOrDefault().IsLocked)
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
                            String matchPageString = "CompanyDetail";
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
                                Data.mstBranch newBranch = new Data.mstBranch();
                                newBranch.CompanyId = branch.CompanyId;
                                newBranch.Branch = branch.Branch;
                                newBranch.Address = branch.Address;
                                newBranch.ContactNumber = branch.ContactNumber;

                                db.mstBranches.InsertOnSubmit(newBranch);
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

        // update branch
        [Authorize]
        [HttpPut]
        [Route("api/branch/update/{id}")]
        public HttpResponseMessage updateBranch(String id, Models.MstBranch branch)
        {
            try
            {
                var companies = from d in db.mstCompanies where d.Id == branch.CompanyId select d;
                if (companies.Any())
                {
                    if(!companies.FirstOrDefault().IsLocked) 
                    {
                        var branches = from d in db.mstBranches where d.Id == Convert.ToInt32(id) select d;
                        if (branches.Any())
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
                                String matchPageString = "CompanyDetail";
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
                                    var updateBranch = branches.FirstOrDefault();
                                    updateBranch.CompanyId = branch.CompanyId;
                                    updateBranch.Branch = branch.Branch;
                                    updateBranch.Address = branch.Address;
                                    updateBranch.ContactNumber = branch.ContactNumber;

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

        // delete branch
        [Authorize]
        [HttpDelete]
        [Route("api/branch/delete/{id}")]
        public HttpResponseMessage deleteBranch(String id)
        {
            try
            {
                var branches = from d in db.mstBranches where d.Id == Convert.ToInt32(id) select d;
                if (branches.Any())
                {
                    var companies = from d in db.mstCompanies where d.Id == branches.FirstOrDefault().CompanyId select d;
                    if (companies.Any())
                    {
                        if (!companies.FirstOrDefault().IsLocked)
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
                                String matchPageString = "CompanyDetail";
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
                                    db.mstBranches.DeleteOnSubmit(branches.First());
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
