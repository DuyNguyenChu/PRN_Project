using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.Configuration;
using iText.Layout.Properties;
using api.Extensions;

namespace api.Helpers
{
    public static class CommonConstants
    {
        public const int ADMIN_USER = 1001;

        public static class MenuType
        {
            public const string ADMIN = "ADMIN";
            public const string CLIENT_HEADER = "CLIENT_HEADER";
            public const string CLIENT_FOOTER = "CLIENT_FOOTER";
        }

        public static readonly HashSet<DataItem<string>> ListMenuType = new()
        {
            new DataItem<string>{
                Id=MenuType.ADMIN,
                Name="Menu danh cho trang admin"
            },
            //new DataItem<string>{
            //    Id=MenuType.CLIENT_HEADER,
            //    Name = "Header cho trang client"
            //},
            //new DataItem<string>{
            //    Id=MenuType.CLIENT_FOOTER,
            //    Name = "Footer cho trang client"
            //}
        };

        public static class Cache
        {
            public const string PERMISSIONS_ALL_KEY = "Permissions:All";
            public const string SYSTEMCONFIG_ALL_KEY = "SystemConfig:All";
        }

        public static class Action
        {
            public const int CREATE = 1001;
            public const int READ = 1002;
            public const int UPDATE = 1003;
            public const int DELETE = 1004;
            public const int EXPORT = 1005;
            public const int APPROVE = 1006;
        }

        public static class ActionCode
        {
            public const string CREATE = "CREATE";
            public const string UPDATE = "UPDATE";
            public const string DELETE = "DELETE";
            public const string EXPORT = "EXPORT";
            public const string REJECT = "REJECT";
            public const string APPROVE = "APPROVE";
        }

        public static readonly Dictionary<string, string> ListAction = new()
        {
            //{ "CREATE", "Tạo mới" },
            //{ "UPDATE", "Cập nhật" },
            //{ "DELETE", "Xóa" },
            //{ "EXPORT", "Xuất dữ liệu" },
            //{ "REJECT", "Từ chối" },
            //{ "APPROVE", "Duyệt" },
            {"Added","Thêm mới"},
            {"Modified","Cập nhật"},
            {"Deleted","Xoá"}
        };

        public static class SystemConfig
        {
            public const string USER_MANUAL_KEY = "USER_MANUAL";
            public const string PRIVACY_POLIVY_KEY = "PRIVACY_POLICY";
            public const string TERMS_OF_SERVICE_KEY = "TERMS_OF_SERVICE";

            public const string HOT_LINE_KEY = "HOT_LINE";
            public const string ADDRESS_KEY = "ADDRESS";
            public const string EMAIL_KEY = "EMAIL";
            public const string LOGO_KEY = "LOGO";
            public const string GOOGLE_MAPS_LINK_KEY = "GOOGLE_MAPS_LINK";
            public const string WORKING_HOURS_KEY = "WORKING_HOURS";
        }

        public static class Role
        {
            public const int ADMIN = 1;
            public const int END_USER = 3;
            public const int DRIVER = 4;
            public const int DISPATCHER = 2;
            public const int EXECUTIVE = 1005;
        }

        public static readonly HashSet<int> ListRoleRegister = new()
        {
            Role.DISPATCHER
        };

        public static readonly HashSet<int> ListRoleForOffice = new()
        {
            Role.DISPATCHER,
        };

        public static readonly HashSet<int> ListRoleForUserManagement = new()
        {
            Role.ADMIN,
            Role.END_USER,
            Role.DISPATCHER,
            Role.EXECUTIVE
        };

        public static class UserVerificationTokenPurpose
        {
            public const string ACCOUNT_ACTIVATION = "ACCOUNT_ACTIVATION";
            public const string RESET_PASSWORD = "RESET_PASSWORD";
            public const string FORGOT_PASSWORD = "FORGOT_PASSWORD";

        }

        public static class TagType
        {
            public const int TAG_TYPE_BLOG = 1001;
        }

        public static class ActivityLogType
        {
            public const string REQUEST = "REQUEST";
            public const string AUDIT = "AUDIT";
        }

        public static readonly Dictionary<string, string> ProvinceTypes = new()
        {
            {
                "city","Thành phố Trung ương"
            },
            {
                "province","Tỉnh"
            }
        };

        public static readonly Dictionary<string, string> WardTypes = new()
        {
            {
                "ward","Phường"
            },
            {
                "commune","Xã"
            }
        };

        public static class ReportPeriod
        {
            public const string LAST_7_DAY = "LAST_7_DAY";
            public const string TODAY = "TODAY";
            public const string WEEK = "WEEK";
            public const string MONTH = "MONTH";
            public const string YEAR = "YEAR";
        }

        public static class VehicleStatus
        {
            public const int AVAILABLE = 1001;
            public const int IN_USE = 1002;
            public const int IN_MAINTENANCE = 1003;
            public const int BROKEN = 1004;
            public const int UNAVAILABLE = 1005;
        }

        public static class DriverStatus
        {
            public const int AVAILABLE = 1001;
            public const int ON_TRIP = 1002;
            public const int ON_LEAVE = 1003;
            public const int UNAVAILABLE = 1004;
        }

        public static class TripRequestStatus
        {
            public const int PENDING = 1001;
            public const int APPROVED = 1002;
            public const int REJECTED = 1003;
            public const int CANCELLED = 1004;
            public const int DRAFT = 1005;
            public static readonly int[] CreateAccept = { PENDING, DRAFT };

        }
        public static class TripStatus
        {
            public const int APPROVED = 1001;
            public const int DISPATCHED = 1002;
            public const int DRIVER_CONFIRMED = 1003;
            public const int REJECTED_BY_DRIVER = 1004;
            public const int MOVING_TO_PICKUP = 1005;
            public const int ARRIVED_AT_PICKUP = 1006;
            public const int MOVING_TO_DESTINATION = 1007;
            public const int ARRIVED_AT_DESTINATION = 1008;
            public const int COMPLETED = 1009;
            public const int CANCELLED_BY_USER = 1010;
            public const int CANCELLED_BY_ADMIN = 1011;
            public const int CANCELLED_BY_DRIVER = 1012;
        }

        public static class NotificationCategory
        {
            public const int TRIP_REQUEST = 1001;
            public const int TRIP = 1002;
            public const int FUEL_LOG = 1003;
            public const int TRIP_EXPENSE = 1004;
            public const int MAINTENANCE_RECORD = 1005;
        }
        public static class NotificationType
        {
            public const int SYSTEM = 1001;
        }

        public static class BlogPostStatus
        {
            public const int DRAFT = 1001;
            public const int SCHEDULED = 1002;
            public const int PUBLISHED = 1003;
            public const int INACTIVE = 1004;
        }

        public static class ExpenseType
        {
            public const int TOLL_FEE = 1001; // cầu đường
            public const int PARKING_FEE = 1002; // đỗ xe
            public const int ACCOMMODATION = 1003; // lưu trú
            public const int MEAL = 1004; // ăn uống
            public const int OTHER = 1005; // khác

        }

        public static class DriverSalaryStatus
        {
            public const int PENDING = 0;
            public const int APPROVED = 1;
            public const int REJECTED = 2;
            public const int PAID = 3;
            public static readonly int[] All = { PENDING, APPROVED, REJECTED, PAID };
        }

        public static readonly Dictionary<string, string> VehicleFuelType = new()
        {
            {
                "GASOLINE","Xăng"
            },
            {
                "DIESEL","Dầu diesel"
            },
            {
                "ELECTRIC","Điện"
            }
        };

        public static readonly Dictionary<string, string> FuelType = new()
        {
            {
                "RON_95_III","Xăng RON 95-III"
            },
            {
                "RON_95_IV","Xăng RON 95-IV"
            },
            {
                "RON_95_V","Xăng RON 95-V"
            },
            {
                "E5_RON_92","Xăng E5 RON 92"
            },
            {
                "DO","Dầu diesel"
            },
            {
                "ELECTRIC","Điện"
            }
        };

        public static string GetFuelTypeName(string fuelCode) => FuelType.TryGetValue(fuelCode, out var name) ? name : "";

        public static readonly Dictionary<string, string> ServiceType = new()
        {
            {
                "MAINTENANCE","Bảo trì"
            },
            {
                "REPAIR","Sửa chữa"
            }
        };

        public static readonly HashSet<DetailStatusDto<int>> ApprovalStatuses = new()
        {
            new DetailStatusDto<int>{
                Id=0,
                Name="Chờ duyệt",
                Color="#1B84FF"
            },
            new DetailStatusDto<int>{
                Id=1,
                Name="Đã duyệt",
                Color="#17C653"
            },
            new DetailStatusDto<int>{
                Id=2,
                Name="Từ chối",
                Color="#F8285A"
            }
        };

        public static readonly Dictionary<string, string> AccidentType = new()
        {
            {
                "ACCIDENT","Tai nạn"
            },
            {
                "INCIDENT","Sự cố"
            }
        };

        public static readonly HashSet<DetailStatusDto<string>> DamageLevel = new()
        {
            new DetailStatusDto<string>{
                Id="MINOR",
                Name="Nhẹ",
                Description="Trầy xước nhẹ, không ảnh hưởng vận hành, sửa chữa đơn giản",
                Color="#17C653"
            },
            new DetailStatusDto<string>{
                Id="MODERATE",
                Name="Trung bình",
                Description="Hư hỏng nhỏ đến vừa, ảnh hưởng một phần, cần sửa trong ngày",
                Color="#1B84FF"
            },
            new DetailStatusDto<string>{
                Id="MAJOR",
                Name="Nặng",
                Description="Gây ảnh hưởng đáng kể, cần đưa vào garage, sửa vài ngày",
                Color="#7239EA"
            },
            new DetailStatusDto<string>{
                Id="CRITICAL",
                Name="Rất nặng",
                Description="Không thể sử dụng xe, phải kéo về gara, chi phí lớn",
                Color="#DEAD00"
            },
            new DetailStatusDto<string>{
                Id="TOTALLOSS",
                Name="Thiệt hại hoàn toàn",
                Description="Xe hư hỏng hoàn toàn, không thể sửa chữa\r\n",
                Color="#D81A48"
            }
        };

        public static readonly Dictionary<string, string> LicenseClass = new()
        {
            { "A1", "Hạng A1" },
            { "A2", "Hạng A2" },
            { "A3", "Hạng A3" },
            { "A4", "Hạng A4" },
            { "B1", "Hạng B1" },
            { "B2", "Hạng B2" },
            { "C", "Hạng C" },
            { "D", "Hạng D" },
            { "E", "Hạng E" },
            { "F", "Hạng F" }
        };

        public static class Fare
        {
            public const int REVENUE = 16800;
            public const int DRIVER = 14800;
            public const int MinutesPerKilometer = 2;
            public const decimal VAT_RATE_REVENUE = 0.08m;
        }

        //Hình thức chạy
        public static class PackageFeeType
        {
            public const string FIXED = "FIXED";
            public const string PER_KM = "PER_KM";
            public const string COMBINED = "COMBINED";
            public static readonly string[] All = { FIXED, PER_KM, COMBINED };
        }

        public static readonly Dictionary<string, string> PackageFeeTypes = new()
        {
            { "FIXED", "Phí trọn gói" },
            { "PER_KM", "Phí tính theo km" },
            { "COMBINED", "Phí trọn gói + thêm theo km"}
        };

        public static readonly List<string> MonthNames = new List<string>
        {
            "Tháng 01", "Tháng 02", "Tháng 03", "Tháng 04",
            "Tháng 05", "Tháng 06", "Tháng 07", "Tháng 08",
            "Tháng 09", "Tháng 10", "Tháng 11", "Tháng 12"
        };

        public static readonly Dictionary<string, string> EntityDisplayMap = new()
        {
            { "User", "Người dùng" },
            { "TripRequest", "Yêu cầu chuyến xe" },
            { "Trip", "Chuyến đi"},
            { "Driver", "Lái xe" },
            { "DriverSalary", "Lương lái xe" },
            { "Vehicle", "Xe"},
            { "FuelLog", "Lịch sử nạp nhiên liệu"},
            { "MaintenanceRecord", "Lịch sử sửa chữa/bảo dưỡng"},
            { "MaintenanceRecordDetail", "Chi tiết lịch sử sửa chữa/bảo dưỡng"},
            { "TripExpense", "Chi phí chuyến"},
            { "Office", "Phòng giao dịch"},
        };

        public static class RouteSegmentCode
        {
            public const string GENERATED_CONNECTING_ROUTE = "GENERATED_CONNECTING_ROUTE";
            public const string GENERATED = "GENERATED";
            // 2 cái này tự sinh nếu chuyến đặt là thường xuyên chuyến đầu ngày/cuối ngày
            public const string START_OF_DAY = "START_OF_DAY";
            public const string END_OF_DAY = "END_OF_DAY";
            public static readonly string[] All = { GENERATED_CONNECTING_ROUTE, GENERATED, START_OF_DAY, END_OF_DAY };
        }

        public static readonly Dictionary<string, string> RouteSegmentCodes = new()
        {
            { "GENERATED_CONNECTING_ROUTE", "Cuốc phát sinh nối tuyến" },
            { "GENERATED", "Cuốc phát sinh" },
            { "START_OF_DAY", "Cuốc đầu ngày"},
            { "END_OF_DAY", "Cuốc cuối ngày"},
        };

        public static class TripSegmentStatus
        {
            public const int PENDING = 1001;
            public const int MOVING = 1002;
            public const int COMPLETED = 1003;
            public const int CANCEL = 1004;
        }

        public static class RouteType
        {
            public const string FIXED = "FIXED";
            public const string GENERATED = "GENERATED";
        }

    }
}
