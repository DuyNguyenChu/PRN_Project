using System;
using System.Collections.Generic;

namespace api.Dtos.MaintenanceRecord
{
    public class CreateMaintenanceRecordDto
    {
        public int VehicleId { get; set; }
        public int Odometer { get; set; }
        public string ServiceType { get; set; } = null!;
        public string ServiceProvider { get; set; } = null!;
        public DateTimeOffset StartTime { get; set; } // Đổi tên và kiểu
        public DateTimeOffset? EndTime { get; set; } // Đổi tên và kiểu
        public string Notes { get; set; } = string.Empty; // Model là NOT NULL [cite: 8077]

        // Không còn Attachments

        public List<CreateMaintenanceRecordDetailDto> Details { get; set; } = new List<CreateMaintenanceRecordDetailDto>();
    }
}