using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using UserAPI.Business.Interfaces;
using UserAPI.Business.Services;
using UserAPI.Model.Dto;
using UserAPI.Model.Entity;

public class SessionManager : ISessionManager
{
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _configuration;
    private readonly TokenService _tokenService;

    public SessionManager(IDistributedCache cache, IConfiguration configuration, TokenService tokenService)
    {
        _cache = cache;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public async Task<UserSessionDto> CreateSessionAsync(string userId, string deviceInfo) // TokenService sonradan eklendi, burasının güncellenmesi gerekiyor.
    {
        var session = new UserSession
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Token = _tokenService.GenerateToken(new LoginDto { }), //Hata giderme amacıyla yapıldı. Doldurulacak.
            DeviceInfo = deviceInfo,
            CreatedAt = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow,
            IsActive = true
        };

        await SaveSessionToCache(session);

        return new UserSessionDto
        {
            Id = session.Id,
            UserId = session.UserId,
            Token = session.Token,
            DeviceInfo = session.DeviceInfo,
            CreatedAt = session.CreatedAt,
            LastActivity = session.LastActivity,
            IsActive = session.IsActive
        };
    }

    public async Task<bool> ValidateSessionAsync(string token)
    {
        var sessionKey = $"session_{token}";
        var sessionJson = await _cache.GetStringAsync(sessionKey);

        if (string.IsNullOrEmpty(sessionJson))
            return false;

        var session = JsonSerializer.Deserialize<UserSession>(sessionJson);
        if (!session.IsActive)
            return false;

        // Gereksiz güncellemeleri önlemek için, belirli bir süre içinde güncellemeye gerek kalmadığını kontrol edebilirsiniz.
        if ((DateTime.UtcNow - session.LastActivity).TotalMinutes > 10)
        {
            session.LastActivity = DateTime.UtcNow;
            await SaveSessionToCache(session);
        }

        return true;
    }

    public async Task<IEnumerable<UserSessionDto>> GetUserSessionsAsync(string userId)
    {
        var sessions = new List<UserSession>();
        var sessionPattern = $"session_*";

        var allKeys = await GetAllKeys(sessionPattern);

        foreach (var key in allKeys)
        {
            var sessionJson = await _cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(sessionJson))
            {
                var session = JsonSerializer.Deserialize<UserSession>(sessionJson);
                if (session.UserId == userId && session.IsActive)
                {
                    sessions.Add(session);
                }
            }
        }

        return sessions.Select(s => new UserSessionDto
        {
            Id = s.Id,
            UserId = s.UserId,
            Token = s.Token,
            DeviceInfo = s.DeviceInfo,
            CreatedAt = s.CreatedAt,
            LastActivity = s.LastActivity,
            IsActive = s.IsActive
        });
    }

    public async Task<bool> InvalidateSessionAsync(string sessionId)
    {
        var sessionKey = $"session_{sessionId}";
        var sessionJson = await _cache.GetStringAsync(sessionKey);

        if (string.IsNullOrEmpty(sessionJson))
            return false;

        var session = JsonSerializer.Deserialize<UserSession>(sessionJson);
        session.IsActive = false;
        await SaveSessionToCache(session);
        return true;
    }

    public async Task<bool> InvalidateAllSessionsAsync(string userId)
    {
        var sessions = await GetUserSessionsAsync(userId);
        foreach (var session in sessions)
        {
            await InvalidateSessionAsync(session.Id);
        }
        return true;
    }

    private async Task SaveSessionToCache(UserSession session)
    {
        var sessionKey = $"session_{session.Token}";
        var sessionJson = JsonSerializer.Serialize(session);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
        };

        await _cache.SetStringAsync(sessionKey, sessionJson, cacheOptions);
    }

    private async Task<IEnumerable<string>> GetAllKeys(string pattern)
    {
        // Cacheten anahtarları çekip liste olarak döndürmelisin.
        return new List<string>();
    }
}
