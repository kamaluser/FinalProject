using Cinema.UI.Models;
using Cinema.UI.Models.OrderModels;
using Cinema.UI.Models.UserModels;

namespace Cinema.UI.Services
{
    public interface ICrudService
    {
        Task<PaginatedResponse<TResponse>> GetAllPaginated<TResponse>(string path, int page);
        Task<List<T>> GetAll<T>(string endpoint);
        Task<TResponse> Get<TResponse>(string path);
        Task<CreateResponse> Create<TRequest>(TRequest request, string path);
        Task Update<TRequest>(TRequest request, string path);
        Task Delete(string path);

        Task<CreateResponse> CreateFromForm<TRequest>(TRequest request, string path);
        Task UpdateFromForm<TRequest>(TRequest request, string path);

        Task<AdminCreateResponse> CreateAdmin<TRequest>(TRequest request, string path);

        Task<byte[]> ExportAsync();
        Task<int> GetOrderCountLastMonthAsync();
        Task<int> GetOrderCountLastYearAsync();
        Task<int> GetSessionCountLastMonthAsync();
        Task<YearlyOrderResponse> GetMonthlyOrderCountForCurrentYearAsync();
        Task<int> GetTotalOrderedSeatsCountAsync();
        Task<int> GetMembersCountAsync();
        Task<decimal> GetMonthlyRevenueAsync();
        Task<YearlyRevenueResponse> GetMonthlyRevenueForCurrentYearAsync();
        //Task<List<OrderDetailResponse>> GetAllOrderDetailsAsync();
    }
}
