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
    public class ApiApplicantController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // applicant list
        [Authorize]
        [HttpGet]
        [Route("api/applicant/list")]
        public List<Models.tblApplicant> listApplicant()
        {
            var applicants = from d in db.tblApplicants.OrderByDescending(d => d.Id)
                             select new Models.tblApplicant
                             {
                                 Id = d.Id,
                                 ApplicantCode = d.ApplicantCode,
                                 FullName = d.FullName,
                                 BirthDate = d.BirthDate.ToShortDateString(),
                                 CivilStatusId = d.CivilStatusId,
                                 CivilStatus = d.tblApplicantCivilStatus.CivilStatus,
                                 CityAddress = d.CityAddress,
                                 ProvinceAddress = d.ProvinceAddress,
                                 ResidenceTypeId = d.ResidenceTypeId,
                                 ResidenceType = d.tblApplicantResidenceType.ResidenceType,
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
                                 Schools = d.Schools,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.tblUser.FirstName + " " + d.tblUser.MiddleName + " " + d.tblUser.LastName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.tblUser1.FirstName + " " + d.tblUser1.MiddleName + " " + d.tblUser1.LastName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            return applicants.ToList();
        }

        // get applicant
        [Authorize]
        [HttpGet]
        [Route("api/applicant/getById/{id}")]
        public Models.tblApplicant listApplicant(String id)
        {
            var applicants = from d in db.tblApplicants
                             where d.Id == Convert.ToInt32(id)
                             select new Models.tblApplicant
                             {
                                 Id = d.Id,
                                 ApplicantCode = d.ApplicantCode,
                                 FullName = d.FullName,
                                 BirthDate = d.BirthDate.ToShortDateString(),
                                 CivilStatusId = d.CivilStatusId,
                                 CivilStatus = d.tblApplicantCivilStatus.CivilStatus,
                                 CityAddress = d.CityAddress,
                                 ProvinceAddress = d.ProvinceAddress,
                                 ResidenceTypeId = d.ResidenceTypeId,
                                 ResidenceType = d.tblApplicantResidenceType.ResidenceType,
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
                                 Schools = d.Schools,
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUser = d.tblUser.FirstName + " " + d.tblUser.MiddleName + " " + d.tblUser.LastName,
                                 CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                                 UpdatedByUserId = d.UpdatedByUserId,
                                 UpdatedByUser = d.tblUser1.FirstName + " " + d.tblUser1.MiddleName + " " + d.tblUser1.LastName,
                                 UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                             };

            if (applicants.Any())
            {
                return (Models.tblApplicant)applicants.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        // zero fill for code numbers
        public String zeroFill(String number, Int32 length)
        {
            var result = number;
            var pad = length - result.Length;
            while (pad > 0)
            {
                result = '0' + result;
                pad--;
            }
            return result;
        }

        // add applicant 
        [Authorize]
        [HttpPost]
        [Route("api/applicant/add")]
        public Int32 addApplicant()
        {
            try
            {
                var userId = (from d in db.tblUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                var lastApplicantCode = from d in db.tblApplicants.OrderByDescending(d => d.Id) select d;
                var applicantCode = "0000000001";

                if (lastApplicantCode.Any())
                {
                    var computeCode = Convert.ToInt32(lastApplicantCode.FirstOrDefault().ApplicantCode) + 0000000001;
                    applicantCode = zeroFill(Convert.ToString(computeCode), 10);
                }

                Data.tblApplicant newApplicant = new Data.tblApplicant();
                newApplicant.ApplicantCode = applicantCode;
                newApplicant.FullName = "NA";
                newApplicant.BirthDate = DateTime.Today;
                newApplicant.CivilStatusId = (from d in db.tblApplicantCivilStatus select d.Id).FirstOrDefault();
                newApplicant.CityAddress = "NA";
                newApplicant.ProvinceAddress = "NA";
                newApplicant.ResidenceTypeId = (from d in db.tblApplicantResidenceTypes select d.Id).FirstOrDefault();
                newApplicant.LengthOfStay = "NA";
                newApplicant.BusinessAddress = "NA";
                newApplicant.BusinessKaratulaName = "NA";
                newApplicant.BusinessTelephoneNumber = "NA";
                newApplicant.BusinessYear = "NA";
                newApplicant.BusinessMerchandise = "NA";
                newApplicant.BusinessStockValues = 0;
                newApplicant.BusinessBeginningCapital = 0;
                newApplicant.BusinessLowSalesPeriod = "NA";
                newApplicant.BusinessLowestDailySales = 0;
                newApplicant.BusinessAverageDailySales = 0;
                newApplicant.EmployedCompany = "NA";
                newApplicant.EmployedCompanyAddress = "NA";
                newApplicant.EmployedPositionOccupied = "NA";
                newApplicant.EmployedServiceLength = "NA";
                newApplicant.EmployedTelephoneNumber = "NA";
                newApplicant.SpouseFullName = "NA";
                newApplicant.SpouseEmployerBusiness = "NA";
                newApplicant.SpouseEmployerBusinessAddress = "NA";
                newApplicant.SpouseBusinessTelephoneNumber = "NA";
                newApplicant.SpousePositionOccupied = "NA";
                newApplicant.SpouseMonthlySalary = 0;
                newApplicant.SpouseLengthOfService = "NA";
                newApplicant.NumberOfChildren = "NA";
                newApplicant.Studying = "NA";
                newApplicant.Schools = "NA";
                newApplicant.CreatedByUserId = userId;
                newApplicant.CreatedDateTime = DateTime.Now;
                newApplicant.UpdatedByUserId = userId;
                newApplicant.UpdatedDateTime = DateTime.Now;

                db.tblApplicants.InsertOnSubmit(newApplicant);
                db.SubmitChanges();

                return newApplicant.Id;
            }
            catch
            {
                return 0;
            }
        }

        // update applicant
        [Authorize]
        [HttpPut]
        [Route("api/applicant/update/{id}")]
        public HttpResponseMessage updateApplicant(String id, Models.tblApplicant applicant)
        {
            try
            {
                var applicants = from d in db.tblApplicants where d.Id == Convert.ToInt32(id) select d;
                if (applicants.Any())
                {
                    var userId = (from d in db.tblUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();

                    var updateApplicant = applicants.FirstOrDefault();
                    updateApplicant.FullName = applicant.FullName;
                    updateApplicant.BirthDate = Convert.ToDateTime(applicant.BirthDate);
                    updateApplicant.CivilStatusId = applicant.CivilStatusId;
                    updateApplicant.CityAddress = applicant.CityAddress;
                    updateApplicant.ProvinceAddress = applicant.ProvinceAddress;
                    updateApplicant.ResidenceTypeId = applicant.ResidenceTypeId;
                    updateApplicant.LengthOfStay = applicant.LengthOfStay;
                    updateApplicant.BusinessAddress = applicant.BusinessAddress;
                    updateApplicant.BusinessKaratulaName = applicant.BusinessKaratulaName;
                    updateApplicant.BusinessTelephoneNumber = applicant.BusinessTelephoneNumber;
                    updateApplicant.BusinessYear = applicant.BusinessYear;
                    updateApplicant.BusinessMerchandise = applicant.BusinessMerchandise;
                    updateApplicant.BusinessStockValues = applicant.BusinessStockValues;
                    updateApplicant.BusinessBeginningCapital = applicant.BusinessBeginningCapital;
                    updateApplicant.BusinessLowSalesPeriod = applicant.BusinessLowSalesPeriod;
                    updateApplicant.BusinessLowestDailySales = applicant.BusinessLowestDailySales;
                    updateApplicant.BusinessAverageDailySales = applicant.BusinessAverageDailySales;
                    updateApplicant.EmployedCompany = applicant.EmployedCompany;
                    updateApplicant.EmployedCompanyAddress = applicant.EmployedCompanyAddress;
                    updateApplicant.EmployedPositionOccupied = applicant.EmployedPositionOccupied;
                    updateApplicant.EmployedServiceLength = applicant.EmployedServiceLength;
                    updateApplicant.EmployedTelephoneNumber = applicant.EmployedTelephoneNumber;
                    updateApplicant.SpouseFullName = applicant.SpouseFullName;
                    updateApplicant.SpouseEmployerBusiness = applicant.SpouseEmployerBusiness;
                    updateApplicant.SpouseEmployerBusinessAddress = applicant.SpouseEmployerBusinessAddress;
                    updateApplicant.SpouseBusinessTelephoneNumber = applicant.SpouseBusinessTelephoneNumber;
                    updateApplicant.SpousePositionOccupied = applicant.SpousePositionOccupied;
                    updateApplicant.SpouseMonthlySalary = applicant.SpouseMonthlySalary;
                    updateApplicant.SpouseLengthOfService = applicant.SpouseLengthOfService;
                    updateApplicant.NumberOfChildren = applicant.NumberOfChildren;
                    updateApplicant.Studying = applicant.Studying;
                    updateApplicant.Schools = applicant.Schools;
                    updateApplicant.UpdatedByUserId = userId;
                    updateApplicant.UpdatedDateTime = DateTime.Now;

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

        // delete applicant
        [Authorize]
        [HttpDelete]
        [Route("api/applicant/delete/{id}")]
        public HttpResponseMessage deleteApplicant(String id)
        {
            try
            {
                var applicants = from d in db.tblApplicants where d.Id == Convert.ToInt32(id) select d;
                if (applicants.Any())
                {
                    db.tblApplicants.DeleteOnSubmit(applicants.First());
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
