using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace api.Helpers
{
    public static class PhoneHelper
    {
        // Số điện thoại Việt Nam hợp lệ: 10 số, bắt đầu từ 03, 05, 07, 08, 09
        private static readonly Regex VietnamPhoneRegex = new Regex(
            @"^(0[3|5|7|8|9])+([0-9]{8})$",
            RegexOptions.Compiled
        );

        public static bool IsValidVietnamPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return VietnamPhoneRegex.IsMatch(phone);
        }
    }

}
