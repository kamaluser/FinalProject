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

    public ReminderJob(IOrderRepository orderRepository, EmailService emailService, ISessionService sessionService)
    {
        _orderRepository = orderRepository;
        _emailService = emailService;
        _sessionService = sessionService;
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
                var body = $"Your session is starting on {order.ShowDateTime:MMMM d, yyyy h:mm tt}.";
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