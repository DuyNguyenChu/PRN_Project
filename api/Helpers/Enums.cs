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
            DASHBOARD = 11,
            TRIP_REQUEST_LIST = 26,
            TRIP_LIST = 16,
            USER_LIST = 6,
            USER_STATUS = 4,
            ROLE = 5,
            MENU_LIST = 8,
            TRIP_REQUEST_STATUS = 13,
            TRIP_STATUS = 14,
            VEHICLE_BRAND = 19,
            VEHICLE_MODEL = 18,
            VEHICLE_STATUS = 15,
            VEHICLE_TYPE = 20,
            TRIP_EXPENSE = 17,
            DRIVER_LIST = 28,
            VEHICLE_LIST = 10,
            ACTION = 7,
        }

        public enum Role
        {
            ADMIN = 1,
            USER = 3,
            DRIVER = 4,
            DISPATCHER = 2,
            EXECUTIVE
        }

    }
}
