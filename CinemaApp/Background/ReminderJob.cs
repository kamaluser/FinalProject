﻿using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Interfaces;
using Cinema.Service.Services;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            var orders = _orderRepository.GetAll(o => o.SessionId == session.Id)
                                         .Select(o => new { o.User.Email, session.ShowDateTime })
                                         .ToList();

            foreach (var order in orders)
            {
                var subject = "Session Reminder";
                var body = $"Your session is starting on {order.ShowDateTime:MMMM d, yyyy h:mm tt}.";
                _emailService.Send(order.Email, subject, body);
            }
        }
    }
}
