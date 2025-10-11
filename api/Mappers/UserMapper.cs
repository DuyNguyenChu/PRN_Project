using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using api.Dtos.User;
using api.Helpers;
using api.Models;

namespace api.Mappers
{
    public static class UserMapper
    {
        public static User ToEntity(this CreateUserDto obj)
        {
            return new User
            {
                CreatedBy = obj.CreatedBy,
                UserName = obj.Email.ToLower(),
                PasswordHash = obj.PasswordHash,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Email = obj.Email.ToLower(),
                PhoneNumber = obj.PhoneNumber,
                Gender = obj.Gender,
                UserStatusId = (int)Enums.UserStatus.Actived,
                CreatedDate = DateTime.Now
            };
        }

        public static User ToEntity(this UpdateUserDto obj, User existData)
        {
            existData.UpdatedBy = obj.UpdatedBy;
            existData.LastModifiedDate = DateTime.Now;
            existData.UserStatusId = obj.UserStatusId;
            

            return existData;
        }

        public static UserDetailDto ToDto(this User entity)
        {
            return new UserDetailDto
            {
                Id = entity.Id,
                Username = entity.UserName,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Gender = entity.Gender,
                CreatedDate = entity.CreatedDate,
            };
        }

        //Admin Register User
        public static User ToEntity(this AdminRegisterDto obj)
        {
            return new User
            {
                UserName = obj.Username.Trim(),
                PasswordHash = PasswordHelper.HashPassword(obj.Password.Trim()),
                FirstName = obj.FirstName.Trim(),
                LastName = obj.LastName.Trim(),
                Email = obj.Email.Trim(),
                UserStatusId = (int)Enums.UserStatus.NotActivated,
                //OfficeId = obj.OfficeId,
                CreatedDate = DateTime.Now
            };
        }

        //Register User
        public static User ToEntity(this UserSignUpDto obj)
        {
            return new User
            {
                UserName = obj.UserName.Trim(),
                PasswordHash = PasswordHelper.HashPassword(obj.Password.Trim()),
                FirstName = obj.FirstName.Trim(),
                LastName = obj.LastName.Trim(),
                Email = obj.Email.Trim(),
                PhoneNumber = obj.PhoneNumber.Trim(),
                UserStatusId = (int)Enums.UserStatus.NotActivated,
                CreatedDate = DateTime.Now
            };
        }

        //Admin create user
        //Register User
        public static User ToEntity(this CreateEndUserDto obj)
        {
            return new User
            {
                UserName = obj.UserName.Trim(),
                PasswordHash = PasswordHelper.HashPassword(obj.Password.Trim()),
                FirstName = obj.FirstName.Trim(),
                LastName = obj.LastName.Trim(),
                Email = obj.Email.Trim(),
                PhoneNumber = obj.PhoneNumber != null ? obj.PhoneNumber.Trim() : null,
                UserStatusId = (int)Enums.UserStatus.Actived,
                CreatedDate = DateTime.Now,
                CreatedBy = obj.CreatedBy,
                //truyền thêm
                BirthDay = DateTime.ParseExact(obj.DateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal),
                Gender = obj.Gender,
            };
        }

    }
}
