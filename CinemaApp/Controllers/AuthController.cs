using Cinema.Core.Entites;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.UserDtos;
using Cinema.Service.Dtos.UserDtos.MemberDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        /*[HttpGet("users")]
        public async Task<IActionResult> CreateUser()
        {
            if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            }
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await _roleManager.RoleExistsAsync("Member"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Member"));
            }

            AppUser user1 = new AppUser
            {
                FullName = "Member",
                UserName = "member",
            };
            await _userManager.CreateAsync(user1, "Member123");

            AppUser user2 = new AppUser
            {
                FullName = "Admin",
                UserName = "admin",
            };
            await _userManager.CreateAsync(user2, "Admin123");

            AppUser user3 = new AppUser
            {
                FullName = "SuperAdmin",
                UserName = "superadmin",
            };
            await _userManager.CreateAsync(user3, "Superadmin123");

            var result1 = await _userManager.AddToRoleAsync(user1, "Member");
            var result2 = await _userManager.AddToRoleAsync(user2, "Admin");
            var result3 = await _userManager.AddToRoleAsync(user3, "SuperAdmin");

            return Ok(user1.Id);
        }*/

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

        [Authorize]
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


        [Authorize(Roles = "SuperAdmin")]
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

        [HttpPost("api/account/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            await _authService.ResetPasswordAsync(resetPasswordDto.UserId, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            return NoContent();
        }
    }
}