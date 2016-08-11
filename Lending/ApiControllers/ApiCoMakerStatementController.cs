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
    public class ApiCoMakerStatementController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // co - makers statement list by applicant id
        [Authorize]
        [HttpGet]
        [Route("api/coMakerStatement/getByApplicantId/{applicantId}")]
        public List<Models.MstCoMakerStatement> listCoMakerStatement(String applicantId)
        {
            var coMakerStatements = from d in db.mstCoMakerStatements.OrderByDescending(d => d.Id)
                                    where d.ApplicantId == Convert.ToInt32(applicantId)
                                    select new Models.MstCoMakerStatement
                                    {
                                        Id = d.Id,
                                        ApplicantId = d.ApplicantId,
                                        Applicant = d.mstApplicant.FullName,
                                        FullName = d.FullName,
                                        BirthDate = d.BirthDate.ToShortDateString(),
                                        CivilStatusId = d.CivilStatusId,
                                        CivilStatus = d.mstCivilStatus.CivilStatus,
                                        CityAddress = d.CityAddress,
                                        ProvinceAddress = d.ProvinceAddress,
                                        ResidenceTypeId = d.ResidenceTypeId,
                                        ResidenceType = d.mstResidenceType.ResidenceType,
                                        ResidenceMonthlyRentAmount = d.ResidenceMonthlyRentAmount,
                                        LandResidenceTypeId = d.LandResidenceTypeId,
                                        LandResidenceType = d.mstResidenceType1.ResidenceType,
                                        LandResidenceMonthlyRentAmount = d.LandResidenceMonthlyRentAmount,
                                        LengthOfStay = d.LengthOfStay,
                                        BusinessAddress = d.BusinessAddress,
                                        BusinessKaratulaName = d.BusinessKaratulaName,
                                        BusinessTelephoneNumber = d.BusinessTelephoneNumber,
                                        BusinessYear = d.BusinessYear,
                                        BusinessMerchandise = d.BusinessMerchandise,
                                        BusinessStockValues = d.BusinessStockValues,
                                        BusinessBeginningCapital = d.BusinessBeginningCapital,
                                        BusinessLowSalesPeriod = d.BusinessLowSalesPeriod,
                                        BusinessLowestDailySales = d.BusinessLowestDailySales,
                                        BusinessAverageDailySales = d.BusinessAverageDailySales,
                                        EmployedCompany = d.EmployedCompany,
                                        EmployedCompanyAddress = d.EmployedCompanyAddress,
                                        EmployedPositionOccupied = d.EmployedPositionOccupied,
                                        EmployedServiceLength = d.EmployedServiceLength,
                                        EmployedTelephoneNumber = d.EmployedTelephoneNumber,
                                        SpouseFullName = d.SpouseFullName,
                                        SpouseEmployerBusiness = d.SpouseEmployerBusiness,
                                        SpouseEmployerBusinessAddress = d.SpouseEmployerBusinessAddress,
                                        SpouseBusinessTelephoneNumber = d.SpouseBusinessTelephoneNumber,
                                        SpousePositionOccupied = d.SpousePositionOccupied,
                                        SpouseMonthlySalary = d.SpouseMonthlySalary,
                                        SpouseLengthOfService = d.SpouseLengthOfService,
                                        NumberOfChildren = d.NumberOfChildren,
                                        Studying = d.Studying,
                                        Schools = d.Schools
                                    };

            return coMakerStatements.ToList();
        }

        // add co - makers statement 
        [Authorize]
        [HttpPost]
        [Route("api/coMakerStatement/add")]
        public HttpResponseMessage addCoMakerStatement(Models.MstCoMakerStatement coMakerStatement)
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                Data.mstCoMakerStatement newCoMakerStatement = new Data.mstCoMakerStatement();
                newCoMakerStatement.ApplicantId = coMakerStatement.ApplicantId;
                newCoMakerStatement.FullName = coMakerStatement.FullName;
                newCoMakerStatement.BirthDate = Convert.ToDateTime(coMakerStatement.BirthDate);
                newCoMakerStatement.CivilStatusId = coMakerStatement.CivilStatusId;
                newCoMakerStatement.CityAddress = coMakerStatement.CityAddress;
                newCoMakerStatement.ProvinceAddress = coMakerStatement.ProvinceAddress;
                newCoMakerStatement.ResidenceTypeId = coMakerStatement.ResidenceTypeId;
                newCoMakerStatement.ResidenceMonthlyRentAmount = coMakerStatement.ResidenceMonthlyRentAmount;
                newCoMakerStatement.LandResidenceTypeId = coMakerStatement.LandResidenceTypeId;
                newCoMakerStatement.LandResidenceMonthlyRentAmount = coMakerStatement.LandResidenceMonthlyRentAmount;
                newCoMakerStatement.LengthOfStay = coMakerStatement.LengthOfStay;
                newCoMakerStatement.BusinessAddress = coMakerStatement.BusinessAddress;
                newCoMakerStatement.BusinessKaratulaName = coMakerStatement.BusinessKaratulaName;
                newCoMakerStatement.BusinessTelephoneNumber = coMakerStatement.BusinessTelephoneNumber;
                newCoMakerStatement.BusinessYear = coMakerStatement.BusinessYear;
                newCoMakerStatement.BusinessMerchandise = coMakerStatement.BusinessMerchandise;
                newCoMakerStatement.BusinessStockValues = coMakerStatement.BusinessStockValues;
                newCoMakerStatement.BusinessBeginningCapital = coMakerStatement.BusinessBeginningCapital;
                newCoMakerStatement.BusinessLowSalesPeriod = coMakerStatement.BusinessLowSalesPeriod;
                newCoMakerStatement.BusinessLowestDailySales = coMakerStatement.BusinessLowestDailySales;
                newCoMakerStatement.BusinessAverageDailySales = coMakerStatement.BusinessAverageDailySales;
                newCoMakerStatement.EmployedCompany = coMakerStatement.EmployedCompany;
                newCoMakerStatement.EmployedCompanyAddress = coMakerStatement.EmployedCompanyAddress;
                newCoMakerStatement.EmployedPositionOccupied = coMakerStatement.EmployedPositionOccupied;
                newCoMakerStatement.EmployedServiceLength = coMakerStatement.EmployedServiceLength;
                newCoMakerStatement.EmployedTelephoneNumber = coMakerStatement.EmployedTelephoneNumber;
                newCoMakerStatement.SpouseFullName = coMakerStatement.SpouseFullName;
                newCoMakerStatement.SpouseEmployerBusiness = coMakerStatement.SpouseEmployerBusiness;
                newCoMakerStatement.SpouseEmployerBusinessAddress = coMakerStatement.SpouseEmployerBusinessAddress;
                newCoMakerStatement.SpouseBusinessTelephoneNumber = coMakerStatement.SpouseBusinessTelephoneNumber;
                newCoMakerStatement.SpousePositionOccupied = coMakerStatement.SpousePositionOccupied;
                newCoMakerStatement.SpouseMonthlySalary = coMakerStatement.SpouseMonthlySalary;
                newCoMakerStatement.SpouseLengthOfService = coMakerStatement.SpouseLengthOfService;
                newCoMakerStatement.NumberOfChildren = coMakerStatement.NumberOfChildren;
                newCoMakerStatement.Studying = coMakerStatement.Studying;
                newCoMakerStatement.Schools = coMakerStatement.Schools;

                db.mstCoMakerStatements.InsertOnSubmit(newCoMakerStatement);
                db.SubmitChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // update co - makers statement
        [Authorize]
        [HttpPut]
        [Route("api/coMakerStatement/update/{id}")]
        public HttpResponseMessage updateCoMakerStatement(String id, Models.MstCoMakerStatement coMakerStatement)
        {
            try
            {
                var coMakerStatements = from d in db.mstCoMakerStatements where d.Id == Convert.ToInt32(id) select d;
                if (coMakerStatements.Any())
                {
                    var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                    var updateCoMakerStatement = coMakerStatements.FirstOrDefault();
                    updateCoMakerStatement.ApplicantId = coMakerStatement.ApplicantId;
                    updateCoMakerStatement.FullName = coMakerStatement.FullName;
                    updateCoMakerStatement.BirthDate = Convert.ToDateTime(coMakerStatement.BirthDate);
                    updateCoMakerStatement.CivilStatusId = coMakerStatement.CivilStatusId;
                    updateCoMakerStatement.CityAddress = coMakerStatement.CityAddress;
                    updateCoMakerStatement.ProvinceAddress = coMakerStatement.ProvinceAddress;
                    updateCoMakerStatement.ResidenceTypeId = coMakerStatement.ResidenceTypeId;
                    updateCoMakerStatement.ResidenceMonthlyRentAmount = coMakerStatement.ResidenceMonthlyRentAmount;
                    updateCoMakerStatement.LandResidenceTypeId = coMakerStatement.LandResidenceTypeId;
                    updateCoMakerStatement.LandResidenceMonthlyRentAmount = coMakerStatement.LandResidenceMonthlyRentAmount;
                    updateCoMakerStatement.LengthOfStay = coMakerStatement.LengthOfStay;
                    updateCoMakerStatement.BusinessAddress = coMakerStatement.BusinessAddress;
                    updateCoMakerStatement.BusinessKaratulaName = coMakerStatement.BusinessKaratulaName;
                    updateCoMakerStatement.BusinessTelephoneNumber = coMakerStatement.BusinessTelephoneNumber;
                    updateCoMakerStatement.BusinessYear = coMakerStatement.BusinessYear;
                    updateCoMakerStatement.BusinessMerchandise = coMakerStatement.BusinessMerchandise;
                    updateCoMakerStatement.BusinessStockValues = coMakerStatement.BusinessStockValues;
                    updateCoMakerStatement.BusinessBeginningCapital = coMakerStatement.BusinessBeginningCapital;
                    updateCoMakerStatement.BusinessLowSalesPeriod = coMakerStatement.BusinessLowSalesPeriod;
                    updateCoMakerStatement.BusinessLowestDailySales = coMakerStatement.BusinessLowestDailySales;
                    updateCoMakerStatement.BusinessAverageDailySales = coMakerStatement.BusinessAverageDailySales;
                    updateCoMakerStatement.EmployedCompany = coMakerStatement.EmployedCompany;
                    updateCoMakerStatement.EmployedCompanyAddress = coMakerStatement.EmployedCompanyAddress;
                    updateCoMakerStatement.EmployedPositionOccupied = coMakerStatement.EmployedPositionOccupied;
                    updateCoMakerStatement.EmployedServiceLength = coMakerStatement.EmployedServiceLength;
                    updateCoMakerStatement.EmployedTelephoneNumber = coMakerStatement.EmployedTelephoneNumber;
                    updateCoMakerStatement.SpouseFullName = coMakerStatement.SpouseFullName;
                    updateCoMakerStatement.SpouseEmployerBusiness = coMakerStatement.SpouseEmployerBusiness;
                    updateCoMakerStatement.SpouseEmployerBusinessAddress = coMakerStatement.SpouseEmployerBusinessAddress;
                    updateCoMakerStatement.SpouseBusinessTelephoneNumber = coMakerStatement.SpouseBusinessTelephoneNumber;
                    updateCoMakerStatement.SpousePositionOccupied = coMakerStatement.SpousePositionOccupied;
                    updateCoMakerStatement.SpouseMonthlySalary = coMakerStatement.SpouseMonthlySalary;
                    updateCoMakerStatement.SpouseLengthOfService = coMakerStatement.SpouseLengthOfService;
                    updateCoMakerStatement.NumberOfChildren = coMakerStatement.NumberOfChildren;
                    updateCoMakerStatement.Studying = coMakerStatement.Studying;
                    updateCoMakerStatement.Schools = coMakerStatement.Schools;
                    db.SubmitChanges();

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // update co - makers statement
        [Authorize]
        [HttpDelete]
        [Route("api/coMakerStatement/delete/{id}")]
        public HttpResponseMessage deleteCoMakerStatement(String id)
        {
            try
            {
                var coMakerStatements = from d in db.mstCoMakerStatements where d.Id == Convert.ToInt32(id) select d;
                if (coMakerStatements.Any())
                {
                    db.mstCoMakerStatements.DeleteOnSubmit(coMakerStatements.First());
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
