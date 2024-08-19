using Cinema.UI.Models.UserModels;

namespace Cinema.UI.Services
{
    public interface ILayoutService
    {
        Task<UserProfileResponse> GetProfile();
    }
}
