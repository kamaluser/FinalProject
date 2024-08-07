using Cinema.Service.Interfaces;
using Quartz;
using System.Threading.Tasks;

namespace CinemaApp.Background
{
    public class ResetReservationsJob : IJob
    {
        private readonly IOrderService _orderService;

        public ResetReservationsJob(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _orderService.ResetExpiredReservationsAsync();
        }
    }
}
