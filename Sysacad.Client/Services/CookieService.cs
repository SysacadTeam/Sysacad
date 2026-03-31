using Microsoft.JSInterop;
using Sysacad.Client.Attributes;
using Sysacad.Client.Services.Interfaces;

namespace Sysacad.Client.Services
{
    [RegisterService(ServiceLifetime.Scoped, typeof(ICookieService))]
    public class CookieService : ICookieService
    {
        private readonly IJSRuntime _runtime;

        public CookieService(IJSRuntime jSRuntime)
        {
            _runtime = jSRuntime;
        }

        public async Task DeleteCookieAsync(string cookieName)
        {
            if (string.IsNullOrWhiteSpace(cookieName))
            {
                throw new ArgumentException("Cookie name cannot be null or empty.", nameof(cookieName));
            }

                await _runtime.InvokeVoidAsync("cookieInterop.delete", cookieName);
            }

        public async Task<T?> GetCookieAsync<T>(string cookieName)
        {
            if (string.IsNullOrWhiteSpace(cookieName))
            {
                throw new ArgumentException("Cookie name cannot be null or empty.", nameof(cookieName));
            }

            var cookieValue = await _runtime.InvokeAsync<string?>("cookieInterop.get", cookieName);

            if (cookieValue == null)
            {
                return default;
            }

            return (T?)Convert.ChangeType(cookieValue, typeof(T));
        }

        public async Task SetCookieAsync(string cookieName, string value, DateTime expiry)
        {
            if (string.IsNullOrWhiteSpace(cookieName))
            {
                throw new ArgumentException("Cookie name cannot be null or empty.", nameof(cookieName));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Cookie value cannot be null or empty.", nameof(value));
            }

            string expires = expiry.ToUniversalTime().ToString("R");
                await _runtime.InvokeVoidAsync("cookieInterop.set", cookieName, value, expires);
            }
    }
}
