using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Extensions
{
    public static class HttpContextExtensions
    {
        public static int GetCurrentUserId(this HttpContext context)
        {
            return Convert.ToInt32(context.User.FindFirst(x => x.Type == ClaimNames.ID)?.Value);
        }

        //public static int? GetCurrentOfficeId(this HttpContext context)
        //{
        //    var officeId = context.User.FindFirst(x => x.Type == ClaimNames.OFFICE_ID)?.Value;
        //    return string.IsNullOrEmpty(officeId) || officeId == "0" ? null : Convert.ToInt32(officeId);
        //}

        public static List<int> GetCurrentRoleIds(this HttpContext context)
        {
            var roleIds = context.User.FindFirst(x => x.Type == ClaimNames.ROLE_IDS)?.Value;
            return string.IsNullOrEmpty(roleIds) ? new List<int>() : roleIds.Split(",").Select(x => Convert.ToInt32(x)).ToList();
        }

        public static int? GetCurrentDriverId(this HttpContext context)
        {
            var driverId = context.User.FindFirst(x => x.Type == ClaimNames.DRIVER_ID)?.Value;
            return string.IsNullOrEmpty(driverId) || driverId == "0" ? null : Convert.ToInt32(driverId);
        }

    }
}
