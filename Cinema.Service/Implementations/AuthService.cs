using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.UserDtos;
using Cinema.Service.Dtos.UserDtos.MemberDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Cinema.Service.Services;
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
        private readonly EmailService _emailService;
        private readonly IOrderRepository _orderRepository;

        public AuthService(UserManager<AppUser> userManager, IMapper mapper, IConfiguration configuration, EmailService emailService, IOrderRepository orderRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
            _orderRepository = orderRepository;
        }

        public MemberProfileGetDto GetByIdForUserProfile(string userId)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            

            var orders = _orderRepository.GetAll(
                o => o.UserId == userId,
                includes: new string[] { "Session.Language", "Session.Movie", "Session.Hall", "OrderSeats.Seat" }
            )
            .Select(order => new MemberOrderGetDtoForProfile
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                NumberOfSeats = order.NumberOfSeats,
                TotalPrice = order.TotalPrice,
                MovieName = order.Session.Movie.Title,
                ShowDateTime = order.Session.ShowDateTime,
                HallName = order.Session.Hall.Name,
                Language = order.Session.Language.Name,
                SeatNumbers = order.OrderSeats.Select(os => os.Seat.Number).ToList()
            }).ToList();

            var userProfile = new MemberProfileGetDto
            {
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                HasPassword = _userManager.HasPasswordAsync(user).Result,
                IsGoogleLogin = _userManager.GetLoginsAsync(user).Result.Any(login => login.LoginProvider == "Google"),
                Orders = orders ?? new List<MemberOrderGetDtoForProfile>(),
            };

            return userProfile;
        }

        public async Task UpdateProfile(MemberProfileEditDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new RestException(StatusCodes.Status404NotFound, "UserName", "User not found.");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new RestException(StatusCodes.Status400BadRequest, "Email", "Email is not confirmed.");

            if (_userManager.Users.Any(x => x.Id != user.Id && x.UserName == dto.UserName))
                throw new RestException(StatusCodes.Status400BadRequest, "UserName", "UserName is already taken.");

            user.UserName = dto.UserName;
            user.FullName = dto.FullName;

            if (_userManager.Users.Any(x => x.Id != user.Id && x.NormalizedEmail == dto.Email.ToUpper()))
                throw new RestException(StatusCodes.Status400BadRequest, "Email", "Email is already taken.");

            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                if (dto.IsGoogleLogin || !dto.HasPassword)
                {
                    var addPasswordResult = await _userManager.AddPasswordAsync(user, dto.NewPassword);
                    if (!addPasswordResult.Succeeded)
                    {
                        var errors = string.Join(", ", addPasswordResult.Errors.Select(e => e.Description));
                        throw new RestException(StatusCodes.Status400BadRequest, $"Failed to add new password: {errors}");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dto.CurrentPassword))
                        throw new RestException(StatusCodes.Status400BadRequest, "CurrentPassword", "Current password is required.");

                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                        throw new RestException(StatusCodes.Status400BadRequest, $"Failed to change password: {errors}");
                    }
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to update profile: {errors}");
            }
        }

        public async Task<int> GetMemberCountAsync()
            {
            var users = _userManager.Users.ToList();
            var memberUsers = new List<AppUser>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Member"))
                {
                    memberUsers.Add(user);
                }
            }

            return memberUsers.Count;
        }
        public string CreateAdmin(SuperAdminCreateAdminDto createDto)
        {

            var existingUser = _userManager.FindByNameAsync(createDto.UserName).Result;
            if (existingUser != null)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "UserName", "UserName is already taken");
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
                throw new RestException(StatusCodes.Status401Unauthorized, "UserName or Password is incorrect!");
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
                    throw new RestException(StatusCodes.Status400BadRequest, "CurrentPassword", "Current password is incorrect.");
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
                throw new RestException(StatusCodes.Status400BadRequest, "CurrentPassword", "Current password is incorrect.");
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

        public async Task<string> UserLogin(MemberLoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                throw new RestException(StatusCodes.Status401Unauthorized, "UserName or Email incorrect!");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new RestException(StatusCodes.Status401Unauthorized, "Email", "Email not confirmed.");
            }

            var token = await GenerateJwtToken(user);

            return token;
        }


        public async Task<string> UserRegister(MemberRegisterDto registerDto)
        {

            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Password and ConfirmPassword do not match.");
            }

            if (_userManager.Users.Any(u => u.Email.ToLower() == registerDto.Email.ToLower()))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Email is already taken.");
            }

            if (_userManager.Users.Any(u => u.UserName.ToLower() == registerDto.UserName.ToLower()))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "UserName is already taken.");
            }

            var appUser = new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                NeedsPasswordReset = false
            };

            var result = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to register user: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(appUser, "member");
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to assign role: {errors}");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);


            var confirmationUrl = $"{_configuration["AppSettings:BaseUrl"]}api/account/verifyemail?userId={appUser.Id}&token={Uri.EscapeDataString(token)}";

            var subject = "Email Verification";
            var body = $"Please click <a href=\"{confirmationUrl}\">here</a> for confirm your email.";
            _emailService.Send(appUser.Email, subject, body);


            return appUser.Id;
        }

        public async Task<bool> VerifyEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);


            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Email is not confirmed.");
            }
            var decodedToken = Uri.UnescapeDataString(token);
            var r = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", decodedToken);


            if (!r)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Invalid token.");
            }

            return true;
        }


        public async Task<string> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetUrl = $"{_configuration["AppSettings:BaseUrl"]}api/account/resetpassword?email={user.Email}&token={Uri.EscapeDataString(token)}";

            var subject = "Password Reset";
            var body = $"Please click <a href=\"{resetUrl}\">here</a> to reset your password.";
            _emailService.Send(email, subject, body);

            return "Password reset link has been sent to your email.";
        }


        public async Task ResetPasswordForForgetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var decodedToken = Uri.UnescapeDataString(token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to reset password: {errors}");
            }
        }

        public async Task<bool> ResetPasswordAsync(string userName, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }

        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FullName", user.FullName ?? "Unknown")
             };


            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


            var secret = _configuration.GetSection("JWT:Secret").Value;
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("JWT:Issuer").Value,
                audience: _configuration.GetSection("JWT:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddDays(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}