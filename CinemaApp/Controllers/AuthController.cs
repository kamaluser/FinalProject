using Cinema.Core.Entites;
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

        [HttpGet("users")]
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
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDto loginDto)
        {
            var token = "Bearer " + await _authService.Login(loginDto);
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("profile")]
        public ActionResult Profile()
        {
            return Ok(User.Identity.Name);
        }
    }
}
