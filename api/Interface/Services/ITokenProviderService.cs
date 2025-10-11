using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.User;

namespace api.Interface.Services
{
    public interface ITokenProviderService
    {
        string GenerateToken(UserToken user);
        (string, DateTime) GenerateRefreshToken();
        bool ValidateToken(string authToken, bool requireExpirationTime);
        JwtSecurityToken ParseToken(string tokenString);

    }
}
