using Microsoft.JSInterop;
using Sysacad.Client.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Sysacad.Client.Services
{
    public class CookieService : ICookieService
    {
        private readonly IJSRuntime _jsRuntime;

        public CookieService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetCookieAsync(string key, string value, DateTime expires)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("La clave o el valor del cookie no pueden ser nulos o vacíos.");
            }

            string expiresUtc = expires.ToUniversalTime().ToString("R");
            await _jsRuntime.InvokeVoidAsync("setCookie", key, value, expiresUtc);
        }

        public async Task<T?> GetCookieAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("La clave del cookie no puede ser nula o vacía.");
            }

            T? cookieValue = default;

            try
            {
                cookieValue = await _jsRuntime.InvokeAsync<T>("getCookie", key);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener el cookie.", ex);
            }

            return cookieValue;
        }

        public async Task DeleteCookieAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("La clave del cookie no puede ser nula o vacía.");
            }
            try
            {
                await _jsRuntime.InvokeVoidAsync("deleteCookie", key);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al eliminar el cookie.", ex);
            }
        }
    }
}
