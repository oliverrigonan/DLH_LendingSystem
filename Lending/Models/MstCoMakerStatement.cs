using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lending.Models
{
    public class MstCoMakerStatement
    {
        [Key]
        public Int32 Id { get; set; }
        public byte[] Photo { get; set; }
        public Int32 ApplicantId { get; set; }
        public String Applicant { get; set; }
        public String FullName { get; set; }
        public String BirthDate { get; set; }
        public Int32 CivilStatusId { get; set; }
        public String CivilStatus { get; set; }
        public String CityAddress { get; set; }
        public String ProvinceAddress { get; set; }
        public Int32 ResidenceTypeId { get; set; }
        public String ResidenceType { get; set; }
        public Decimal ResidenceMonthlyRentAmount { get; set; }
        public Int32 LandResidenceTypeId { get; set; }
        public String LandResidenceType { get; set; }
        public Decimal LandResidenceMonthlyRentAmount { get; set; }
        public String LengthOfStay { get; set; }
        public String BusinessAddress { get; set; }
        public String BusinessKaratulaName { get; set; }
        public String BusinessTelephoneNumber { get; set; }
        public String BusinessYear { get; set; }
        public String BusinessMerchandise { get; set; }
        public Decimal BusinessStockValues { get; set; }
        public Decimal BusinessBeginningCapital { get; set; }
        public String BusinessLowSalesPeriod { get; set; }
        public Decimal BusinessLowestDailySales { get; set; }
        public Decimal BusinessAverageDailySales { get; set; }
        public String EmployedCompany { get; set; }
        public String EmployedCompanyAddress { get; set; }
        public String EmployedPositionOccupied { get; set; }
        public String EmployedServiceLength { get; set; }
        public String EmployedTelephoneNumber { get; set; }
        public String SpouseFullName { get; set; }
        public String SpouseEmployerBusiness { get; set; }
        public String SpouseEmployerBusinessAddress { get; set; }
        public String SpouseBusinessTelephoneNumber { get; set; }
        public String SpousePositionOccupied { get; set; }
        public Decimal SpouseMonthlySalary { get; set; }
        public String SpouseLengthOfService { get; set; }
        public String NumberOfChildren { get; set; }
        public String Studying { get; set; }
        public String Schools { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUser { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedByUserId { get; set; }
        public String UpdatedByUser { get; set; }
        public String UpdatedDateTime { get; set; }
    }
}