using Cinema.Core.Entites;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.UserDtos;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CinemaApp.Controllers
{
    [Route("api/admin/[controller]")]
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
        [HttpPost("CreateAdmin")]
        public IActionResult Create(SuperAdminCreateAdminDto createDto)
        {
            return StatusCode(201, new { Id = _authService.CreateAdmin(createDto) });
        }

        [HttpPost("login")]
        public ActionResult Login(AdminLoginDto loginDto)
        {
            var token = _authService.Login(loginDto);
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("profile")]
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
        [HttpGet("GetAll")]
        public ActionResult<List<AdminGetDto>> GetAll(string? search = null)
        {
            var admins = _authService.GetAll(search);
            return Ok(admins);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("GetAllByPage")]
        public ActionResult<PaginatedList<AdminPaginatedGetDto>> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var paginatedAdmins = _authService.GetAllByPage(search, page, size);
            return Ok(paginatedAdmins);
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("update/{id}")]
        public IActionResult Update(string id, AdminEditDto updateDto)
        {
            _authService.Update(id, updateDto);
            return NoContent();
        }

        [HttpPut("updatePassword")]
        public async Task<IActionResult> UpdatePassword(AdminEditDto updatePasswordDto)
        {

            await _authService.UpdatePasswordAsync(updatePasswordDto);
            return NoContent();

        }
    }
}
