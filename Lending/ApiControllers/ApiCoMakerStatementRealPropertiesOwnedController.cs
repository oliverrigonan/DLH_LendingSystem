using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lending.ApiControllers
{
    public class ApiCoMakerStatementRealPropertiesOwnedController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // co maker real properties owned list by co-maker id
        [Authorize]
        [HttpGet]
        [Route("api/coMakerStatementRealPropertiesOwned/listByCoMakerId/{coMakerId}")]
        public List<Models.MstCoMakerStatementRealPropertiesOwned> listCoMakerStatementRealPropertiesOwnedByCoMakerId(String coMakerId)
        {
            var coMakerStatementRealPropertiesOwneds = from d in db.mstCoMakerStatementRealPropertiesOwneds
                                                       where d.CoMakerId == Convert.ToInt32(coMakerId)
                                                       select new Models.MstCoMakerStatementRealPropertiesOwned
                                                       {
                                                           Id = d.Id,
                                                           CoMakerId = d.CoMakerId,
                                                           CoMaker = d.mstCoMakerStatement.CoMakerLastName + ", " + d.mstCoMakerStatement.CoMakerFirstName + " " + d.mstCoMakerStatement.CoMakerMiddleName,
                                                           Real = d.Real,
                                                           Location = d.Location,
                                                           PresentValue = d.PresentValue,
                                                           EcumberedTo = d.EcumberedTo
                                                       };

            return coMakerStatementRealPropertiesOwneds.ToList();
        }

        // add co maker real properties owned
        [Authorize]
        [HttpPost]
        [Route("api/coMakerStatementRealPropertiesOwned/add")]
        public HttpResponseMessage addCoMakerStatementRealPropertiesOwned(Models.MstCoMakerStatementRealPropertiesOwned coMakersRealPropertiesOwned)
        {
            try
            {
                var applicants = from d in db.mstApplicants where d.mstCoMakerStatements.FirstOrDefault().Id == coMakersRealPropertiesOwned.CoMakerId select d;
                if (applicants.Any())
                {
                    if (!applicants.FirstOrDefault().IsLocked)
                    {
                        Data.mstCoMakerStatementRealPropertiesOwned newCoMakerRealPropertiesOwned = new Data.mstCoMakerStatementRealPropertiesOwned();
                        newCoMakerRealPropertiesOwned.CoMakerId = coMakersRealPropertiesOwned.CoMakerId;
                        newCoMakerRealPropertiesOwned.Real = coMakersRealPropertiesOwned.Real;
                        newCoMakerRealPropertiesOwned.Location = coMakersRealPropertiesOwned.Location;
                        newCoMakerRealPropertiesOwned.PresentValue = coMakersRealPropertiesOwned.PresentValue;
                        newCoMakerRealPropertiesOwned.EcumberedTo = coMakersRealPropertiesOwned.EcumberedTo;

                        db.mstCoMakerStatementRealPropertiesOwneds.InsertOnSubmit(newCoMakerRealPropertiesOwned);
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

        // update  co maker real properties owned
        [Authorize]
        [HttpPut]
        [Route("api/coMakerStatementRealPropertiesOwned/update/{id}")]
        public HttpResponseMessage updateCoMakerStatementRealPropertiesOwned(String id, Models.MstCoMakerStatementRealPropertiesOwned coMakersRealPropertiesOwned)
        {
            try
            {
                var applicants = from d in db.mstApplicants where d.mstCoMakerStatements.FirstOrDefault().Id == coMakersRealPropertiesOwned.CoMakerId select d;
                if (applicants.Any())
                {
                    if (!applicants.FirstOrDefault().IsLocked)
                    {
                        var coMakeRealPropertiesOwneds = from d in db.mstCoMakerStatementRealPropertiesOwneds where d.Id == Convert.ToInt32(id) select d;
                        if (coMakeRealPropertiesOwneds.Any())
                        {
                            var updateCoMakerRealPropertiesOwned = coMakeRealPropertiesOwneds.FirstOrDefault();
                            updateCoMakerRealPropertiesOwned.CoMakerId = coMakersRealPropertiesOwned.CoMakerId;
                            updateCoMakerRealPropertiesOwned.Real = coMakersRealPropertiesOwned.Real;
                            updateCoMakerRealPropertiesOwned.Location = coMakersRealPropertiesOwned.Location;
                            updateCoMakerRealPropertiesOwned.PresentValue = coMakersRealPropertiesOwned.PresentValue;
                            updateCoMakerRealPropertiesOwned.EcumberedTo = coMakersRealPropertiesOwned.EcumberedTo;

                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
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

        // delete co maker real properties owned
        [Authorize]
        [HttpDelete]
        [Route("api/coMakerStatementRealPropertiesOwned/delete/{id}")]
        public HttpResponseMessage deleteCoMakerStatementRealPropertiesOwned(String id)
        {
            try
            {
                var coMakeRealPropertiesOwneds = from d in db.mstCoMakerStatementRealPropertiesOwneds where d.Id == Convert.ToInt32(id) select d;
                if (coMakeRealPropertiesOwneds.Any())
                {
                    var applicants = from d in db.mstApplicants where d.mstCoMakerStatements.FirstOrDefault().Id == coMakeRealPropertiesOwneds.FirstOrDefault().CoMakerId select d;
                    if (applicants.Any())
                    {
                        if (!applicants.FirstOrDefault().IsLocked)
                        {
                            db.mstCoMakerStatementRealPropertiesOwneds.DeleteOnSubmit(coMakeRealPropertiesOwneds.First());
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
