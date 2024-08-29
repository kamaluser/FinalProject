using Cinema.Core.Entites;
using Cinema.Data;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Interfaces;
using Cinema.Service.Services;
using Quartz;

public class ReminderJob : IJob
{
    private readonly IOrderRepository _orderRepository;
    private readonly EmailService _emailService;
    private readonly ISessionService _sessionService;
    private readonly IWebHostEnvironment _hostingEnvironment; 

    public ReminderJob(IOrderRepository orderRepository, EmailService emailService, ISessionService sessionService, IWebHostEnvironment hostingEnvironment)
    {
        _orderRepository = orderRepository;
        _emailService = emailService;
        _sessionService = sessionService;
        _hostingEnvironment = hostingEnvironment;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var now = DateTime.Now;
        var reminderWindow = TimeSpan.FromMinutes(30);

        var sessions = _sessionService.GetSessionsForReminder(now, reminderWindow);

        foreach (var session in sessions)
        {
            var orders = _orderRepository.GetAll(o => o.SessionId == session.Id && !o.IsReminderSent)
                                          .Select(o => new { o.Id, o.User.Email, session.ShowDateTime })
                                          .ToList();

            foreach (var order in orders)
            {
                var subject = "Session Reminder";

                var templatePath = Path.Combine(_hostingEnvironment.WebRootPath, "Templates", "EmailTemplates", "ReminderTemplate.html");
                var bodyTemplate = File.ReadAllText(templatePath);
                var body = bodyTemplate.Replace("@Model.ShowDateTime", order.ShowDateTime.ToString("MMMM d, yyyy h:mm tt"));

                _emailService.Send(order.Email, subject, body);

                var orderToUpdate = _orderRepository.Get(o => o.Id == order.Id);
                if (orderToUpdate != null)
                {
                    orderToUpdate.IsReminderSent = true;
                    _orderRepository.Save();
                }
            }
        }
    }
}