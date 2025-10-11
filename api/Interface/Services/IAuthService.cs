using api.Dtos.Auth;
using api.Dtos.User;
using api.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Interface.Services
{
    public interface IAuthService
    {
        Task<ApiResponse> LoginAsync(LoginRequestDto obj);
        Task<ApiResponse> AdminLoginAsync(LoginRequestDto obj);
        Task<ApiResponse> RegisterAsync(UserSignUpDto obj);
        Task<ApiResponse> AdminRegisterAsync(AdminRegisterDto obj);
        Task<ApiResponse> RefreshTokenAsync(RefreshTokenRequestDto obj);
        Task<ApiResponse> LogoutAsync(LogoutRequestDto obj, int userId);
        //Task<ApiResponse> LogoutAllDeviceAsync(int userId);
        //Task<ApiResponse> ForgotPasswordAsync(string email, bool isClientRequest = false);
        Task<ApiResponse> VerifyCodeAsync(VerifyCodeDto dto);
        //Task<ApiResponse> ResetPasswordAsync(ResetPasswordDto dto);
        Task<ApiResponse> GetProfileAsync(int userId);
        Task<ApiResponse> AdminCreateEndUser(CreateEndUserDto obj);
        //Task<ApiResponse> GetCurrentUserDecentralization();
        //Task<ApiResponse> ResendActivationMailAsync(ResendActivationMailDto dto);

    }
}
