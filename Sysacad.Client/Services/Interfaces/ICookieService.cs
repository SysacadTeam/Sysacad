namespace Sysacad.Client.Services.Interfaces
{
    public interface ICookieService
    {
        Task<T?> GetCookieAsync<T>(string cookieName);
        Task SetCookieAsync(string cookieName, string value, DateTime expiryMinutes);
        Task DeleteCookieAsync(string cookieName);
    }
}