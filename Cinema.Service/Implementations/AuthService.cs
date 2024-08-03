using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.UserDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<AppUser> userManager, IMapper mapper, IConfiguration configuration)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
        }
        public string CreateAdmin(SuperAdminCreateAdminDto createDto)
        {

            var existingUser = _userManager.FindByNameAsync(createDto.UserName).Result;
            if (existingUser != null)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "UserName", "UserName already taken");
            }

            var user = new AppUser
            {
                UserName = createDto.UserName,
                NeedsPasswordReset = true

            };

            var result = _userManager.CreateAsync(user, createDto.Password).Result;
            if (!result.Succeeded)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Password", "Failed to create Admin user.");
            }


            var roleResult = _userManager.AddToRoleAsync(user, "Admin").Result;


            if (!roleResult.Succeeded)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "UserName", "Failed to assign role to Admin user.");
            }


            return user.Id;
        }

        public void Delete(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var result = _userManager.DeleteAsync(user).Result;

            if (!result.Succeeded)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Failed to delete Admin user.");
            }
        }

        public List<AdminGetDto> GetAll(string? search = null)
        {

            var users = _userManager.Users.ToList();

            var adminUsers = _mapper.Map<List<AdminGetDto>>(users);

            var filteredAdminUsers = adminUsers.Where(adminUser =>
            {
                var user = users.FirstOrDefault(u => u.Id == adminUser.Id);
                var roles = _userManager.GetRolesAsync(user).Result;
                return roles.Contains("Admin");
            }).ToList();


            if (!string.IsNullOrEmpty(search))
            {
                filteredAdminUsers = filteredAdminUsers
                    .Where(u => u.UserName.Contains(search))
                    .ToList();
            }

            return filteredAdminUsers;
        }

        public PaginatedList<AdminPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {

            var users = _userManager.Users.ToList();


            var adminUsers = _mapper.Map<List<AdminPaginatedGetDto>>(users);


            var filteredAdminUsers = new List<AdminPaginatedGetDto>();

            foreach (var adminUser in adminUsers)
            {
                var user = users.FirstOrDefault(u => u.Id == adminUser.Id);
                var roles = _userManager.GetRolesAsync(user).Result;
                if (roles.Contains("Admin"))
                {
                    filteredAdminUsers.Add(adminUser);
                }
            }


            if (!string.IsNullOrEmpty(search))
            {
                filteredAdminUsers = filteredAdminUsers
                    .Where(u => u.UserName.Contains(search))
                    .ToList();
            }


            var paginatedResult = PaginatedList<AdminPaginatedGetDto>.Create(filteredAdminUsers.AsQueryable(), page, size);

            return paginatedResult;
        }

        public async Task EditPasswordAsync(AdminEditDto editPasswordDto)
        {
            var user = await _userManager.FindByNameAsync(editPasswordDto.UserName);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, editPasswordDto.CurrentPassword);
            if (!passwordCheck)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Current password is incorrect.");
            }

            if (editPasswordDto.NewPassword != editPasswordDto.ConfirmPassword)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "New password and confirm password do not match.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, editPasswordDto.CurrentPassword, editPasswordDto.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to change password: {errors}");
            }

            user.NeedsPasswordReset = false;
            var editResult = await _userManager.UpdateAsync(user);
            if (!editResult.Succeeded)
            {
                var errors = string.Join(", ", editResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to edit user: {errors}");
            }
        }

        public LoginDto Login(AdminLoginDto loginDto)
        {
            AppUser? user = _userManager.FindByNameAsync(loginDto.UserName).Result;

            if (user == null || !_userManager.CheckPasswordAsync(user, loginDto.Password).Result)
            {
                throw new RestException(StatusCodes.Status401Unauthorized, "UserName or Password incorrect!");
            }

            if (user.NeedsPasswordReset)
            {

                string resetToken = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                return new LoginDto { Token = resetToken, NeedsPasswordReset = true };
            }

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim("FullName", user.FullName ?? "Unknown"));


            var roles = _userManager.GetRolesAsync(user).Result;

            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList());

            string secret = _configuration.GetSection("JWT:Secret").Value;

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                issuer: _configuration.GetSection("JWT:Issuer").Value,
                audience: _configuration.GetSection("JWT:Audience").Value,
                expires: DateTime.Now.AddDays(2)
            );

            string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginDto { Token = tokenStr, NeedsPasswordReset = false };
        }

        public void Update(string id, AdminEditDto updateDto)
        {

            var user = _userManager.FindByIdAsync(id).Result;

            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var existingUser = _userManager.FindByNameAsync(updateDto.UserName).Result;
            if (existingUser != null && existingUser.UserName != updateDto.UserName)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "UserName", "UserName already taken");
            }

            user.UserName = updateDto.UserName;

            if (!string.IsNullOrEmpty(updateDto.CurrentPassword) && !string.IsNullOrEmpty(updateDto.NewPassword))
            {

                var passwordCheck = _userManager.CheckPasswordAsync(user, updateDto.CurrentPassword).Result;
                if (!passwordCheck)
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "Current password is incorrect.");
                }


                if (updateDto.NewPassword != updateDto.ConfirmPassword)
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "New password and confirm password do not match.");
                }

                var changePasswordResult = _userManager.ChangePasswordAsync(user, updateDto.CurrentPassword, updateDto.NewPassword).Result;

                if (!changePasswordResult.Succeeded)
                {
                    var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                    throw new RestException(StatusCodes.Status400BadRequest, $"Failed to change password: {errors}");
                }
                user.NeedsPasswordReset = false;
            }

            var updateResult = _userManager.UpdateAsync(user).Result;

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to update user: {errors}");
            }
        }

        public async Task UpdatePasswordAsync(AdminEditDto updatePasswordDto)
        {
            var user = await _userManager.FindByNameAsync(updatePasswordDto.UserName);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, updatePasswordDto.CurrentPassword);
            if (!passwordCheck)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Current password is incorrect.");
            }

            if (updatePasswordDto.NewPassword != updatePasswordDto.ConfirmPassword)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "New password and confirm password do not match.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, updatePasswordDto.CurrentPassword, updatePasswordDto.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to change password: {errors}");
            }

            user.NeedsPasswordReset = false;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to update user: {errors}");
            }
        }
    }
}
