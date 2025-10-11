using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.UserStatus;
using api.Models;

namespace api.Mappers
{
    public static class UserStatusMapper
    {
        public static UserStatus ToEntity(this CreateUserStatusDto dto)
        {
            return new UserStatus
            {
                Name = dto.Name,
                Color = dto.Color,
                CreatedBy = dto.CreatedBy,
                CreatedDate = DateTimeOffset.Now,
                IsDeleted = false,
            };
        }
        public static UserStatus ToEntity(this UpdateUserStatusDto dto, UserStatus existData)
        {
            existData.Color = dto.Color;
            existData.Name = dto.Name;
            existData.UpdatedBy = dto.UpdatedBy;
            existData.LastModifiedDate = DateTimeOffset.Now;
            return existData;
        }

    }
}
