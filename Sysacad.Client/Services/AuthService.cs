using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Sysacad.Client.Attributes;
using Sysacad.Client.Services.Interfaces;
using Sysacad.Shared.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using LoginRequest = Sysacad.Shared.Auth.LoginRequest;


namespace Sysacad.Client.Services
{
    [RegisterService(ServiceLifetime.Scoped, typeof(IAuthService))]
    public class AuthService : IAuthService
    {
        internal const string TokenCookieName = "sysacad_access_token";

        private readonly HttpClient _httpClient;
        private readonly ICookieService _cookieService;
        private readonly SysacadAuthStateProvider _authenticationStateProvider;
        private readonly ILogger<AuthService> _logger;
        private readonly Uri? _apiBaseUri;
        private readonly int _jwtExpiryMinutes;

        public event Action? AuthenticationStateChanged;

        public AuthService(HttpClient httpClient, ICookieService cookieService, AuthenticationStateProvider authenticationStateProvider, ILogger<AuthService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _cookieService = cookieService;
            _logger = logger;
            var configuredBaseUrl = configuration["API_BASE_URL"] ?? configuration["Api:BaseUrl"];
            _apiBaseUri = NormalizeBaseUri(configuredBaseUrl) ?? _httpClient.BaseAddress;
            _authenticationStateProvider = authenticationStateProvider as SysacadAuthStateProvider
                ?? throw new InvalidOperationException("El proveedor de autenticación configurado no es válido.");

            var expiryConfig = configuration["Jwt:ExpiryInMinutes"] ?? Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES") ?? "60";
            _jwtExpiryMinutes = int.TryParse(expiryConfig, out var minutes) ? minutes : 60;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;

            try
            {
                var requestUri = BuildApiUri("api/Auth/Login");
                response = await _httpClient.PostAsJsonAsync(requestUri, request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo contactar al endpoint de login");
                return new LoginResponse
                {
                    Success = false,
                    Message = "No se pudo contactar a la API de autenticación."
                };
            }

            var payload = await DeserializeResponseAsync(response);
            payload.Success = response.IsSuccessStatusCode && payload.Success;

            if (!payload.Success)
            {
                return payload;
            }

            if (string.IsNullOrWhiteSpace(payload.Token))
            {
                payload.Success = false;
                payload.Message = "La API no entregó un token válido.";
                return payload;
            }

            await PersistTokenAsync(payload.Token);
            return payload;
        }

        public async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            var token = await _cookieService.GetCookieAsync<string>(TokenCookieName);

            if (!string.IsNullOrWhiteSpace(token))
            {
                var request = new HttpRequestMessage(HttpMethod.Post, BuildApiUri("api/Auth/LogOut"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                try
                {
                    await _httpClient.SendAsync(request, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo notificar el logout a la API");
                }
            }

            await _cookieService.DeleteCookieAsync(TokenCookieName);
            _authenticationStateProvider.NotifyUserLogout();
            AuthenticationStateChanged?.Invoke();
        }

        public Task<bool> IsAuthenticatedAsync()
        {
            return _authenticationStateProvider.HasValidTokenAsync();
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _cookieService.GetCookieAsync<string>(TokenCookieName);
        }

        public async Task<bool> ValidateSessionAsync(CancellationToken cancellationToken = default)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                _authenticationStateProvider.NotifyUserLogout();
                AuthenticationStateChanged?.Invoke();
                return false;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, BuildApiUri("api/Auth/Refresh"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var payload = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);
                    if (!string.IsNullOrWhiteSpace(payload?.Token))
                    {
                        await PersistTokenAsync(payload.Token);
                    }
                    else
                    {
                        _authenticationStateProvider.NotifyUserAuthentication(token);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo validar/refrescar la sesión actual");
            }

            await _cookieService.DeleteCookieAsync(TokenCookieName);
            _authenticationStateProvider.NotifyUserLogout();
            AuthenticationStateChanged?.Invoke();
            return false;
        }

        private async Task PersistTokenAsync(string token)
        {
            JwtSecurityToken? jwtToken = null;
            try
            {
                jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token JWT inválido recibido durante el login.");
            }

            var expiry = jwtToken?.ValidTo > DateTime.UtcNow
                ? jwtToken!.ValidTo
                : DateTime.UtcNow.AddMinutes(_jwtExpiryMinutes);

            await _cookieService.SetCookieAsync(TokenCookieName, token, expiry);
            _authenticationStateProvider.NotifyUserAuthentication(token);
            AuthenticationStateChanged?.Invoke();
        }

        private static async Task<LoginResponse> DeserializeResponseAsync(HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (content != null)
                {
                    return content;
                }
            }
            catch
            {
                // Ignored - fall through to default response
            }

            return new LoginResponse
            {
                Success = false,
                Message = response.IsSuccessStatusCode
                    ? "Respuesta vacía de la API de autenticación."
                    : "Usuario o contraseña inválidos."
            };
        }

        private Uri BuildApiUri(string relativePath)
        {
            var trimmed = relativePath?.TrimStart('/') ?? string.Empty;

            if (_apiBaseUri != null)
            {
                return new Uri(_apiBaseUri, trimmed);
            }

            return new Uri(trimmed, UriKind.Relative);
        }

        private static Uri? NormalizeBaseUri(string? baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return null;
            }

            var normalized = baseUrl.EndsWith('/') ? baseUrl : baseUrl + '/';

            return Uri.TryCreate(normalized, UriKind.Absolute, out var uri)
                ? uri
                : null;
        }
    }
}