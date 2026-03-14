namespace Sysacad.Client.Services.Interfaces
{
    public interface ICookieService
    {
        Task SetCookieAsync(string key, string value, DateTime expires);
        Task<T?> GetCookieAsync<T>(string key);
        Task DeleteCookieAsync(string key);
    }
}