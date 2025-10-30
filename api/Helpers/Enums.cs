using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Helpers
{
    public static class Enums
    {
        public enum UserStatus
        {
            /// <summary>
            /// Chưa kích hoạt
            /// </summary>
            NotActivated = 1,
            /// <summary>
            /// Đã kích hoạt
            /// </summary>
            Actived = 2,
            /// <summary>
            /// Bị khoá
            /// </summary>
            Locked = 3,
            /// <summary>
            /// Ngưng hoạt động
            /// </summary>
            Deactivated = 4
        }

        public enum Action
        {
            CREATE = 3,
            READ = 2,
            UPDATE = 4,
            DELETE = 9,
            EXPORT,
            APPROVE
        }

        public enum Gender
        {
            FEMALE = 0,
            MALE = 1,
            OTHER = 2
        }

        public enum Menu
        {
            DASHBOARD = 1,
            TRIP_MANAGEMENT,
            TRIP_REQUEST_LIST,
            TRIP_LIST,
            REPORT,
            BLOG_POST,
            BLOG_POST_LIST,
            BLOG_POST_DETAIL,
            BLOG_POST_CATEGORY,
            BLOG_POST_TAG,
            USER,
            USER_LIST,
            DRIVER_MANAGEMENT,
            USER_STATUS,
            ROLE,
            SYSTEM_SETTINGS,
            MENU_LIST,
            BANK_LIST,
            TRIP_REQUEST_STATUS,
            TRIP_STATUS,
            VEHICLE_BRAND,
            VEHICLE_MODEL,
            VEHICLE_STATUS,
            DRIVER_STATUS,
            VEHICLE_TYPE,
            VIOLATION_TYPE,
            EXPENSE_TYPE,
            VEHICLE_MANAGEMENT,
            TRIP_EXPENSE,
            FUEL_LOG,
            MAINTENANCE_RECORD,
            DRIVER_SALARY,
            OFFICE,
            NOTIFICATION_CATEGORY,
            TAG,
            SYSTEM_CONFIG,
            DRIVER_LIST,
            VEHICLE_LIST,
            ACTIVITY_LOGS,
            VEHICLE_REPORT,
            MAINTENANCE_RECORD_REPORT,
            OFFICE_REPORT,
            DISTANCE_REPORT,
            TAG_TYPE,
            NOTIFICATION,
            NOTIFICATION_LIST,
            NOTIFICATION_TYPE,
            NOTIFICATION_TAG,
            MANAGE_TRANSACTION,
            PAYMENT_METHOD,
            PAYMENT_STATUS,
            PAYMENT_TRANSACTION,
            PROVINCE,
            OFFICE_STATUS,
        }

        public enum Role
        {
            ADMIN = 1,
            USER,
            DRIVER,
            DISPATCHER,
            EXECUTIVE
        }

    }
}
