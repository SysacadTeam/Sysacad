using Microsoft.IdentityModel.Tokens;
using Sysacad.Server.Data;
using Sysacad.Server.Data.Entities;
using Sysacad.Server.Data.Repositories;
using Sysacad.Server.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sysacad.Server.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;
        private readonly EntityManager _entityManager;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger, EntityManager entityManager)
        {
            _configuration = configuration;
            _logger = logger;
            _entityManager = entityManager;
        }

        public string GenerateToken(int userId)
        {
            var usuarioRepository = _entityManager.GetRepository<UsuarioRepository>();
            var usuario = usuarioRepository.GetByIdAsync(userId).Result
                          ?? throw new InvalidOperationException($"Usuario con ID {userId} no encontrado");
            var roles = usuario.Perfiles?.SelectMany(perfil => perfil.Permisos ?? new List<Permiso>()).Select(p => p.Codigo).ToList();
            var jwtSettings = _configuration.GetSection("Jwt");
            var environmentKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            var Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? jwtSettings["Issuer"];
            var Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? jwtSettings["Audience"];
            var secretKey = environmentKey
                            ?? jwtSettings["SecretKey"]
                            ?? throw new InvalidOperationException("JWT SecretKey no configurada");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("id", usuario.Id.ToString()),
                new Claim("username", usuario.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            if (roles != null)
            {
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            }

            var expirationValue = Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES")
                                  ?? jwtSettings["ExpiryInMinutes"]
                                  ?? "60";
            var expirationMinutes = int.Parse(expirationValue);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("Token JWT generado para usuario ID: {UserId}", userId);

            return tokenString;
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("Jwt");
                var Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? jwtSettings["Issuer"];
                var Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? jwtSettings["Audience"];
                var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                                ?? jwtSettings["SecretKey"]
                                ?? throw new InvalidOperationException("JWT SecretKey no configurada");

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token JWT inválido");
                return null;
            }
        }

        public bool ShouldRefreshToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var expirationTime = jwtToken.ValidTo;
                var keepAliveMinutesValue = Environment.GetEnvironmentVariable("JWT_KEEP_ALIVE_MINUTES")
                                           ?? _configuration.GetSection("Jwt")["RefreshTokenTime"]
                                           ?? "15";
                var keepAliveMinutes = int.Parse(keepAliveMinutesValue);
                var refreshThreshold = DateTime.UtcNow.AddMinutes(keepAliveMinutes);

                return expirationTime <= refreshThreshold;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error verificando si el token necesita refresh");
                return false;
            }
        }

        public string? RefreshToken(string token)
        {
            try
            {
                var principal = ValidateToken(token);
                if (principal == null)
                {
                    _logger.LogWarning("No se puede refrescar un token inválido");
                    return null;
                }

                var userIdClaim = principal.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogWarning("Token sin userId válido");
                    return null;
                }

                var newToken = GenerateToken(userId);

                _logger.LogInformation("Token refrescado para usuario ID: {UserId}", userId);

                return newToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refrescando token");
                return null;
            }
        }
    }
}