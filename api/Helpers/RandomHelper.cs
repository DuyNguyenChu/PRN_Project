using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace api.Helpers
{
    public static class RandomHelper
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        public static string GenerateRandomString(int length = 10)
        {
            var bytes = new byte[length];
            Rng.GetBytes(bytes);
            var result = new StringBuilder(length);

            foreach (var b in bytes)
            {
                result.Append(Chars[b % Chars.Length]);
            }

            return result.ToString();
        }

        public static string GenerateOtpCode(int length = 6)
        {
            var digits = "0123456789";
            var bytes = new byte[length];
            Rng.GetBytes(bytes);
            var otp = new StringBuilder(length);

            foreach (var b in bytes)
            {
                otp.Append(digits[b % digits.Length]);
            }

            return otp.ToString();
        }
    }

}
