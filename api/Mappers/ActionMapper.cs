using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.Action;

namespace api.Mappers
{
    public static class ActionMapper
    {
        public static Models.Action ToEntity(this CreateActionDto obj)
        {
            return new Models.Action
            {
                CreatedBy = obj.CreatedBy,
                CreatedDate = DateTimeOffset.Now,
                Description = obj.Description,
                Name = obj.Name,
            };
        }

        public static Models.Action ToEntity(this UpdateActionDto obj, Models.Action existData)
        {
            existData.UpdatedBy = obj.UpdatedBy;
            existData.LastModifiedDate = DateTimeOffset.Now;
            existData.Name = obj.Name;
            existData.Description = obj.Description;

            return existData;
        }

        public static ActionDetailDto ToDto(this Models.Action obj)
        {
            return new ActionDetailDto
            {
                Id = obj.Id,
                Name = obj.Name,
                Description = obj.Description,
                CreatedDate = obj.CreatedDate
            };
        }

    }
}
