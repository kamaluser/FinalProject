using Cinema.Service.Dtos;
using Cinema.Service.Dtos.UserDtos;
using Cinema.Service.Dtos.UserDtos.MemberDtos;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Interfaces
{
    public interface IAuthService
    {
        LoginDto Login(AdminLoginDto loginDto);
        string CreateAdmin(SuperAdminCreateAdminDto createDto);
        PaginatedList<AdminPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<AdminGetDto> GetAll(string? search = null);
        void Update(string id, AdminEditDto updateDto);
        void Delete(string id);
        Task UpdatePasswordAsync(AdminEditDto updatePasswordDto);

        Task<string> UserLogin(MemberLoginDto loginDto);
        Task<string> UserRegister(MemberRegisterDto registerDto);
        Task<bool> VerifyEmail(string email, string token);
        Task<string> ForgetPasswordAsync(string email);
        Task ResetPasswordForForgetPasswordAsync(string userId, string token, string newPassword);
        Task<bool> ResetPasswordAsync(string userName, string currentPassword, string newPassword);
        Task<int> GetMemberCountAsync();
        MemberProfileGetDto GetByIdForUserProfile(string userId);
        Task UpdateProfile(MemberProfileEditDto profileEditDto);
    }  
}
