using Cinema.Core.Entites;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.UserDtos;
using Cinema.Service.Dtos.UserDtos.MemberDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CinemaApp.Controllers
{
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(IAuthService authService, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize]
        [HttpGet("api/profileLayout")]
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
            {
                return Unauthorized();
            }
            try
            {
                var userProfile = _authService.GetByIdForUserProfile(userId);
                return Ok(userProfile);
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, new { message = ex.Message });
            }
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [Authorize(Roles = "Member")]
        [HttpPost("api/profile/update")]
        public async Task<IActionResult> UpdateProfile([FromBody] MemberProfileEditDto profileEditDto)
        {
            await _authService.UpdateProfile(profileEditDto);
            return Ok(new { message = "Profile updated successfully!" });
        }

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

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("api/admin/Auth/CreateAdmin")]
        public IActionResult Create(SuperAdminCreateAdminDto createDto)
        {
            return StatusCode(201, new { Id = _authService.CreateAdmin(createDto) });
        }

        [HttpPost("api/admin/Auth/login")]
        public ActionResult Login(AdminLoginDto loginDto)
        {
            var token = _authService.Login(loginDto);
            return Ok(new { token });
        }

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

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("api/admin/Auth/GetAll")]
        public ActionResult<List<AdminGetDto>> GetAll(string? search = null)
        {
            var admins = _authService.GetAll(search);
            return Ok(admins);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("api/admin/Auth/GetAllByPage")]
        public ActionResult<PaginatedList<AdminPaginatedGetDto>> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var paginatedAdmins = _authService.GetAllByPage(search, page, size);
            return Ok(paginatedAdmins);
        }


        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut("api/admin/Auth/update/{id}")]
        public IActionResult Update(string id, AdminEditDto updateDto)
        {
            _authService.Update(id, updateDto);
            return NoContent();
        }

        [HttpPut("api/admin/Auth/updatePassword")]
        public async Task<IActionResult> UpdatePassword(AdminEditDto updatePasswordDto)
        {

            await _authService.UpdatePasswordAsync(updatePasswordDto);
            return NoContent();

        }

        [HttpPost("api/login")]
        public async Task<IActionResult> UserLogin([FromBody] MemberLoginDto loginDto)
        {
            var token = await _authService.UserLogin(loginDto);
            return Ok(new { Result = token });
        }


        [HttpPost("api/register")]
        public ActionResult UserRegister([FromBody] MemberRegisterDto registerDto)
        {
            var r = _authService.UserRegister(registerDto);
            return Ok(new { Result = r });
        }

        [HttpGet("api/account/verifyemail")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid email verification request.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully!");
            }

            return BadRequest("Failed to confirm email.");
        }

        [HttpPost("api/account/forgetpassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
        {
            var message = await _authService.ForgetPasswordAsync(forgetPasswordDto.Email);
            return Ok(new { Message = message });
        }

        [HttpPost("api/account/resetpasswordforforgetpassword")]
        public async Task<IActionResult> ResetPasswordForForgetPassword([FromBody] ResetPasswordForForgetPasswordDto resetPasswordDto)
        {
            await _authService.ResetPasswordForForgetPasswordAsync(resetPasswordDto.UserId, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            return NoContent();
        }


        [HttpPost("api/account/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ResetPasswordAsync(
                resetPasswordDto.UserName,
                resetPasswordDto.CurrentPassword,
                resetPasswordDto.NewPassword);

            if (result)
            {
                return NoContent(); 
            }
            else
            {
                return BadRequest("Password reset failed. Ensure the current password is correct and try again.");
            }
        }


    }
}