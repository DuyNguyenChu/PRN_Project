using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.Role;
using api.Models;

namespace api.Mappers
{
    public static class RoleMapper
    {
        public static Role ToEntity(this CreateRoleDto obj)
        {
            return new Role
            {
                CreatedBy = obj.CreatedBy,
                Description = obj.Description,
                Name = obj.Name,
                CreatedDate = DateTime.Now
            };
        }

        public static Role ToEntity(this UpdateRoleDto obj, Role existData)
        {
            existData.UpdatedBy = obj.UpdatedBy;
            existData.Description = obj.Description;
            existData.Name = obj.Name;
            existData.LastModifiedDate = DateTime.Now;

            return existData;
        }

        public static RoleDetailDto ToDto(this Role entity)
        {
            return new RoleDetailDto
            {
                Id = entity.Id,
                CreatedDate = entity.CreatedDate,
                Description = entity.Description,
                Name = entity.Name,
            };
        }

    }
}
