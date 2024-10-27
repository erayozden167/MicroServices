using UserAPI.Model.Dto;
namespace UserAPI.Business.Interfaces
{
    public interface ISessionManager
    {
        Task<UserSessionDto> CreateSessionAsync(string userId, string deviceInfo);
        Task<bool> ValidateSessionAsync(string token);
        Task<IEnumerable<UserSessionDto>> GetUserSessionsAsync(string userId);
        Task<bool> InvalidateSessionAsync(string sessionId);
        Task<bool> InvalidateAllSessionsAsync(string userId);
    }
}
