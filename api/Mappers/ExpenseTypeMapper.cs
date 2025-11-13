using api.Dtos.ExpenseType;
using api.Models;
using api.Helpers; 

namespace api.Mappers
{
    public static class ExpenseTypeMapper
    {
        public static ExpenseType ToEntity(this CreateExpenseTypeDto obj)
        {
            return new ExpenseType
            {
                CreatedBy = obj.CreatedBy,
                Description = obj.Description,
                Name = obj.Name,
                CreatedDate = DateTime.Now
            };
        }

        public static ExpenseType ToEntity(this UpdateExpenseTypeDto obj, ExpenseType existData)
        {
            existData.UpdatedBy = obj.UpdatedBy;
            existData.Description = obj.Description;
            existData.Name = obj.Name;
            existData.LastModifiedDate = DateTime.Now;

            return existData;
        }

        public static ExpenseTypeDetailDto ToDto(this ExpenseType entity)
        {
            return new ExpenseTypeDetailDto
            {
                Id = entity.Id,
                CreatedDate = entity.CreatedDate,
                Description = entity.Description,
                Name = entity.Name,
            };
        }
    }
}
