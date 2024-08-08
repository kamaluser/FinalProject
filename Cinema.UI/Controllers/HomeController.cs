using Cinema.Service.Interfaces;
using Cinema.UI.Models.HomeModels;
using Cinema.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cinema.Data;
using Microsoft.AspNetCore.Identity;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IOrderService _orderService;
    private readonly ISessionService _sessionService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _context;

    public HomeController(
        ILogger<HomeController> logger,
        IOrderService orderService,
        ISessionService sessionService,
        UserManager<IdentityUser> userManager,
        AppDbContext context)
    {
        _logger = logger;
        _orderService = orderService;
        _sessionService = sessionService;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

        var totalOrdersCount = await _orderService.GetTotalOrdersCountAsync(oneMonthAgo, DateTime.UtcNow);

        var memberUsersCount = await _userManager.Users
            .Where(u => _userManager.IsInRoleAsync(u, "Member").Result)
            .CountAsync();

        var todaysSessionsCount = await _sessionService.GetTodaysSessionsCountAsync(DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1).AddTicks(-1));

        var totalOrderPrice = await _orderService.GetTotalOrderPriceAsync(oneMonthAgo, DateTime.UtcNow);

        var viewModel = new HomeViewModel
        {
            TotalOrdersCount = totalOrdersCount,
            MemberUsersCount = memberUsersCount,
            TodaysSessionsCount = todaysSessionsCount,
            TotalOrderPrice = totalOrderPrice
        };

        return View(viewModel);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
