namespace AZM.Domain.Interfaces
{
    public interface ILocationCacheService
    {
        Task SetLocationAsync(string sessionId, string userId, double lat, double lng);
        Task<Dictionary<string, (double lat, double lng)>> GetAllLocationsAsync(string sessionId);
    }
}