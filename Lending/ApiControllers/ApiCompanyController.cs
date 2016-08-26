using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiCompanyController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // company list
        [Authorize]
        [HttpGet]
        [Route("api/company/list")]
        public List<Models.MstCompany> listCompany()
        {
            var companies = from d in db.mstCompanies.OrderByDescending(d => d.Id)
                            select new Models.MstCompany
                            {
                                Id = d.Id,
                                Company = d.Company,
                                Address = d.Address,
                                ContactNumber = d.ContactNumber,
                                IsLocked = d.IsLocked,
                                CreatedByUserId = d.CreatedByUserId,
                                CreatedByUser = d.mstUser.FullName,
                                CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                UpdatedByUserId = d.UpdatedByUserId,
                                UpdatedByUser = d.mstUser1.FullName,
                                UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                            };

            return companies.ToList();
        }

        // company by id
        [Authorize]
        [HttpGet]
        [Route("api/company/getById/{id}")]
        public Models.MstCompany getCompanyById(String id)
        {
            var company = from d in db.mstCompanies
                          where d.Id == Convert.ToInt32(id)
                          select new Models.MstCompany
                          {
                              Id = d.Id,
                              Company = d.Company,
                              Address = d.Address,
                              ContactNumber = d.ContactNumber,
                              IsLocked = d.IsLocked,
                              CreatedByUserId = d.CreatedByUserId,
                              CreatedByUser = d.mstUser.FullName,
                              CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                              UpdatedByUserId = d.UpdatedByUserId,
                              UpdatedByUser = d.mstUser1.FullName,
                              UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                          };

            return (Models.MstCompany)company.FirstOrDefault();
        }

        // add company
        [Authorize]
        [HttpPost]
        [Route("api/company/add")]
        public Int32 addCompany()
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                Data.mstCompany newCompany = new Data.mstCompany();
                newCompany.Company = "NA";
                newCompany.Address = "NA";
                newCompany.ContactNumber = "NA";
                newCompany.CreatedByUserId = userId;
                newCompany.CreatedDateTime = DateTime.Now;
                newCompany.UpdatedByUserId = userId;
                newCompany.UpdatedDateTime = DateTime.Now;

                db.mstCompanies.InsertOnSubmit(newCompany);
                db.SubmitChanges();

                return newCompany.Id;
            }
            catch
            {
                return 0;
            }
        }

        // lock company
        [Authorize]
        [HttpPut]
        [Route("api/company/lock/{id}")]
        public HttpResponseMessage lockCompany(String id, Models.MstCompany company)
        {
            try
            {
                var companies = from d in db.mstCompanies where d.Id == Convert.ToInt32(id) select d;
                if (companies.Any())
                {
                    if (!companies.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                        var lockCompany = companies.FirstOrDefault();
                        lockCompany.Company = company.Company;
                        lockCompany.Address = company.Address;
                        lockCompany.ContactNumber = company.ContactNumber;
                        lockCompany.IsLocked = true;
                        lockCompany.UpdatedByUserId = userId;
                        lockCompany.UpdatedDateTime = DateTime.Now;

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

        // unlock company
        [Authorize]
        [HttpPut]
        [Route("api/company/unlock/{id}")]
        public HttpResponseMessage unlockCompany(String id, Models.MstCompany company)
        {
            try
            {
                var companies = from d in db.mstCompanies where d.Id == Convert.ToInt32(id) select d;
                if (companies.Any())
                {
                    if (companies.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                        var unlockCompany = companies.FirstOrDefault();
                        unlockCompany.IsLocked = false;
                        unlockCompany.UpdatedByUserId = userId;
                        unlockCompany.UpdatedDateTime = DateTime.Now;

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

        // delete company
        [Authorize]
        [HttpDelete]
        [Route("api/company/delete/{id}")]
        public HttpResponseMessage deleteCompany(String id)
        {
            try
            {
                var companies = from d in db.mstCompanies where d.Id == Convert.ToInt32(id) select d;
                if (companies.Any())
                {
                    if (!companies.FirstOrDefault().IsLocked)
                    {
                        db.mstCompanies.DeleteOnSubmit(companies.First());
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
    }
}
