using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.TripExpense;
using api.Helpers;
using api.Models;

namespace api.Mappers
{
    public static class TripExpenseMapping
    {
        public static TripExpense ToEntity(this CreateTripExpenseDto obj)
        {
            return new TripExpense()
            {
                TripId = obj.TripId,
                ExpenseTypeId = obj.ExpenseTypeId,
                Amount = obj.Amount,
                OccurenceDate = obj.ExpenseDate,
                Notes = obj.Notes,
                CreatedBy = obj.CreatedBy,
                Status = (int)ApprovalStatus.Pending,
                CreatedDate = DateTime.Now,
            };
        }

        public static TripExpense ToEntity(this TripCreateTripExpenseDto obj, int tripId, int createdBy)
        {
            return new TripExpense()
            {
                TripId = tripId,
                ExpenseTypeId = obj.ExpenseTypeId,
                Amount = obj.Amount,
                OccurenceDate = obj.ExpenseDate,
                Notes = obj.Notes,
                CreatedBy = createdBy,
                Status = (int)ApprovalStatus.Pending,
                CreatedDate = DateTime.Now,
            };
        }

        public static TripExpense ToEntity(this UpdateTripExpenseDto obj, TripExpense existData)
        {
            existData.ExpenseTypeId = obj.ExpenseTypeId;
            existData.Amount = obj.Amount;
            existData.OccurenceDate = obj.ExpenseDate;
            existData.Notes = obj.Notes;
            existData.UpdatedBy = obj.UpdatedBy;
            existData.LastModifiedDate = DateTime.Now;
            return existData;
        }

        public static TripExpense ToEntity(this TripUpdateTripExpenseDto obj, TripExpense existData, int updatedBy)
        {
            existData.ExpenseTypeId = obj.ExpenseTypeId;
            existData.Amount = obj.Amount;
            existData.OccurenceDate = obj.ExpenseDate;
            existData.Notes = obj.Notes;
            existData.UpdatedBy = updatedBy;
            existData.LastModifiedDate = DateTime.Now;
            return existData;
        }

        public static TripExpense ToEntity(this ApproveTripExpenseDto obj, TripExpense existData)
        {
            existData.ApprovedDate = DateTime.Now;
            existData.ApprovedBy = obj.ApprovedBy;
            existData.Status = (int)ApprovalStatus.Approved;
            existData.UpdatedBy = obj.ApprovedBy;
            existData.LastModifiedDate = DateTime.Now;
            return existData;
        }

        public static TripExpense ToEntity(this RejectTripExpenseDto obj, TripExpense existData)
        {
            existData.RejectReason = obj.RejectReason;
            existData.Status = (int)ApprovalStatus.Rejected;
            existData.UpdatedBy = obj.RejectBy;
            existData.LastModifiedDate = DateTime.Now;
            return existData;
        }

    }
}
