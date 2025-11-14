using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.Driver;
using api.Dtos.TripStatus;
using api.Helpers;
using api.Models;

namespace api.Mappers
{
    public static class DriverMapping
    {
        public static Driver ToEntity(this CreateDriverDto obj)
        {
            return new Driver
            {
                ExperienceYear = obj.ExperienceYears,
                BaseSalary = obj.BaseSalary,
                LicenseNumber = obj.LicenseNumber,
                LicenseClass = obj.LicenseClass,
                LicenseExpiryDate = obj.LicenseExpiryDate,
                SocialInsuranceNumber = obj.SocialInsuranceNumber,
                //BankId = obj.BankId,
                //BankBranch = obj.BankBranch,
                //BankNumber = obj.BankNumber,
                EmergencyContactName = obj.EmergencyContactName,
                EmergencyContactPhone = obj.EmergencyContactPhone,
                DriverStatusId = obj.DriverStatusId,
                CreatedBy = obj.CreatedBy,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };
        }

        public static Driver ToEntity(this UpdateDriverDto obj, Driver existData)
        {
            existData.BaseSalary = obj.BaseSalary;
            existData.DriverStatusId = obj.DriverStatusId;
            existData.UpdatedBy = obj.UpdatedBy;
            existData.LastModifiedDate = DateTime.Now;

            return existData;
        }

        public static DriverDetailDto ToDto(this Driver entity)
        {
            return new DriverDetailDto
            {
                Id = entity.Id,
                ExperienceYears = entity.ExperienceYear,
                BaseSalary = entity.BaseSalary,
                LicenseNumber = entity.LicenseNumber,
                LicenseClass = entity.LicenseClass,
                LicenseExpiryDate = entity.LicenseExpiryDate,
                SocialInsuranceNumber = entity.SocialInsuranceNumber,
                //BankBranch = entity.BankBranch,
                //BankNumber = entity.BankNumber,
                EmergencyContactName = entity.EmergencyContactName,
                EmergencyContactPhone = entity.EmergencyContactPhone,
                CreatedDate = entity.CreatedDate,
            };
        }

        public static User ToUserEntity(this CreateDriverDto obj)
        {
            return new User
            {
                UserName = obj.Email,
                PasswordHash = PasswordHelper.HashPassword(obj.Password.Trim()),
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Email = obj.Email,
                Gender = obj.Gender,
                BirthDay = obj.DateOfBirth,
                PhoneNumber = obj.PhoneNumber,
                //IdentityNumber = obj.IdentityNumber,
                //AccessFailedCount = 0,
                //LockEnabled = false,
                CreatedDate = DateTime.Now,
                CreatedBy = obj.CreatedBy,
                UserStatusId = (int)Enums.UserStatus.Actived,
            };
        }
    }

}
