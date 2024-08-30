using Cinema.Core.Entites;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.UserDtos;
using Cinema.Service.Dtos.UserDtos.MemberDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace CinemaApp.Controllers
{
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
           private readonly IConfiguration configuration;


        public AuthController(IAuthService authService, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IConfiguration _configuration)
        {
            _authService = authService;
            _roleManager = roleManager;
            _userManager = userManager;
            configuration = _configuration;
        }
        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/signin-google")]
        public async Task<IActionResult> GoogleLogin()
        {

            var response = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (response.Principal == null)
                return BadRequest("Google authentication failed.");


            var email = response.Principal.FindFirstValue(ClaimTypes.Email);
            var fullName = response.Principal.FindFirstValue(ClaimTypes.Name);
            var userName = email;


            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(userName))
                return BadRequest("Incomplete Google account information.");


            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {

                return BadRequest("A user with this email address already exists.");
            }


            user = new AppUser
            {
                Email = email,
                UserName = userName,
                FullName = fullName,
                EmailConfirmed = true,
            };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                return BadRequest("User creation failed.");
            }


            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded)
            {
                return BadRequest("Failed to assign role to user.");
            }

            var token = await GenerateJwtToken(user);
            if (token == null)
                return BadRequest("Login failed. Please try again.");


            var redirectUrl = $"{configuration["Client:URL"]}/account/ExternalLoginCallback?token={token}";
            return Redirect(redirectUrl);
        }
      
        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/login-google")]
        public IActionResult Login()
        {
            var props = new AuthenticationProperties { RedirectUri = "api/signin-google" };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/layout")]
        public ActionResult ProfileForLayout()
        {
            var userName = User.Identity.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return Ok(new UserProfileDto { UserName = userName, Role = role });
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [Authorize(Roles = "Member")]
        [HttpGet("api/GetUserProfile")]
        public ActionResult<MemberProfileGetDto> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();
            try
            {
                var profile = _authService.GetByIdForUserProfile(userId);
                return Ok(profile);
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, new { message = ex.Message });
            }
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [Authorize(Roles = "Member")]
        [HttpPost("api/profile/update")]
        public async Task<IActionResult> UpdateProfile([FromBody] MemberProfileEditDto dto)
        {
            await _authService.UpdateProfile(dto);
            return Ok(new { message = "Profile updated successfully!" });
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/members/count")]
        public async Task<IActionResult> GetMemberCount()
        {
            try
            {
                var count = await _authService.GetMemberCountAsync();
                return Ok(new { Count = count });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("api/admin/Auth/CreateAdmin")]
        public IActionResult Create(SuperAdminCreateAdminDto dto)
        {
            return StatusCode(201, new { Id = _authService.CreateAdmin(dto) });
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPost("api/admin/Auth/login")]
        public ActionResult Login(AdminLoginDto loginDto)
        {
            var token = _authService.Login(loginDto);
            return Ok(new { token });
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("api/admin/Auth/profile")]
        public ActionResult Profile()
        {
            var userName = User.Identity.Name;
            var user = _userManager.FindByNameAsync(userName).Result;

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userDto = new AdminGetDto
            {
                Id = user.Id,
                UserName = user.UserName
            };

            return Ok(userDto);
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("api/admin/Auth/GetAll")]
        public ActionResult<List<AdminGetDto>> GetAll(string? search = null)
        {
            var admins = _authService.GetAll(search);
            return Ok(admins);
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("api/admin/Auth/GetAllByPage")]
        public ActionResult<PaginatedList<AdminPaginatedGetDto>> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var paginatedAdmins = _authService.GetAllByPage(search, page, size);
            return Ok(paginatedAdmins);
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut("api/admin/Auth/update/{id}")]
        public IActionResult Update(string id, AdminEditDto updateDto)
        {
            _authService.Update(id, updateDto);
            return NoContent();
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("api/admin/Auth/updatePassword")]
        public async Task<IActionResult> UpdatePassword(AdminEditDto updatePasswordDto)
        {

            await _authService.UpdatePasswordAsync(updatePasswordDto);
            return NoContent();

        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("api/login")]
        public async Task<IActionResult> UserLogin([FromBody] MemberLoginDto loginDto)
        {
            var token = await _authService.UserLogin(loginDto);
            return Ok(new { Result = token });
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("api/register")]
        public async Task<ActionResult> UserRegister([FromBody] MemberRegisterDto registerDto)
        {
            var r = await _authService.UserRegister(registerDto);
            return Ok(new { Id = r });
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/account/verifyemail")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Invalid email verification request.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully!");
            }

            throw new RestException(StatusCodes.Status404NotFound, "Failed to confirm email.");
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("api/account/forgetpassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
        {
            var message = await _authService.ForgetPasswordAsync(forgetPasswordDto.Email);
            return Ok(new { Message = message });
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("api/account/resetpasswordforforgetpassword")]
        public async Task<IActionResult> ResetPasswordForForgetPassword([FromBody] ResetPasswordForForgetPasswordDto resetPasswordDto)
        {
            await _authService.ResetPasswordForForgetPasswordAsync(resetPasswordDto.Email, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            return NoContent();
        }
        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FullName", user.FullName)
             };


            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


            var secret = configuration.GetSection("JWT:Secret").Value;
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: configuration.GetSection("JWT:Issuer").Value,
                audience: configuration.GetSection("JWT:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}