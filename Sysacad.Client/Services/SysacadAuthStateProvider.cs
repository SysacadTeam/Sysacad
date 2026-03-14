using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Sysacad.Client.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sysacad.Client.Services
{
    public class SysacadAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ICookieService _cookieService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SysacadAuthStateProvider> _logger;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public SysacadAuthStateProvider(ICookieService cookieService, HttpClient httpClient, ILogger<SysacadAuthStateProvider> logger)
        {
            _cookieService = cookieService;
            _httpClient = httpClient;
            _logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var principal = await BuildPrincipalFromCookieAsync() ?? _anonymous;
            return new AuthenticationState(principal);
        }

        public async Task<bool> HasValidTokenAsync()
        {
            var principal = await BuildPrincipalFromCookieAsync();
            return principal?.Identity?.IsAuthenticated == true;
        }

        public void NotifyUserAuthentication(string token)
        {
            var principal = TryCreatePrincipal(token) ?? _anonymous;
            if (principal.Identity?.IsAuthenticated == true)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }

        public void NotifyUserLogout()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        private async Task<ClaimsPrincipal?> BuildPrincipalFromCookieAsync()
        {
            var token = await _cookieService.GetCookieAsync<string>(AuthService.TokenCookieName);
            if (string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                return null;
            }

            var principal = TryCreatePrincipal(token);
            if (principal == null)
            {
                await _cookieService.DeleteCookieAsync(AuthService.TokenCookieName);
                _httpClient.DefaultRequestHeaders.Authorization = null;
                return null;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return principal;
        }

        private ClaimsPrincipal? TryCreatePrincipal(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                if (jwt.ValidTo <= DateTime.UtcNow)
                {
                    return null;
                }

                return new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims, "jwt"));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo interpretar el token JWT almacenado.");
                return null;
            }
        }
    }
}