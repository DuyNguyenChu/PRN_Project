using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.Permission;
using api.Models;

namespace api.Mappers
{
    public static class PermissionMapper
    {
        public static Permission ToEntity(this CreatePermissionDto dto)
        {
            return new Permission
            {
                ActionId = dto.ActionId,
                CreatedBy = dto.CreatedBy,
                CreatedDate = DateTime.Now,
                MenuId = dto.MenuId,
                RoleId = dto.RoleId,
            };
        }

        public static Permission ToEntity(this UpdatePermissionDto dto)
        {
            return new Permission
            {
                ActionId = dto.ActionId,
                CreatedBy = dto.UpdatedBy,
                CreatedDate = DateTime.Now,
                UpdatedBy = dto.UpdatedBy,
                LastModifiedDate = DateTime.Now,
                MenuId = dto.MenuId,
                RoleId = dto.RoleId,
            };
        }

    }
}
