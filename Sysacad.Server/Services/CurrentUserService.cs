using Sysacad.Server.Data;
using Sysacad.Server.Data.Entities;
using Sysacad.Server.Data.Repositories;
using Sysacad.Server.Services;
using System.Security.Claims;

namespace Sysacad.Server.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EntityManager _entityManager;
        private readonly ILogger<CurrentUserService> _logger;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            EntityManager entityManager,
            ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _entityManager = entityManager;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el ID del usuario actual desde el JWT
        /// </summary>
        public int? GetCurrentUserId()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var userIdClaim = httpContext.User.FindFirst("id")?.Value
                           ?? httpContext.User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("Usuario autenticado pero sin claim de userId");
                return null;
            }

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            _logger.LogWarning("Claim de userId no es un número válido: {UserIdClaim}", userIdClaim);
            return null;
        }

        /// <summary>
        /// Obtiene el usuario actual completo desde la base de datos
        /// </summary>
        public async Task<Usuario?> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return null;
            }

            try
            {
                var repository = _entityManager.GetRepository<UsuarioRepository>();
                return await repository.GetByIdAsync(userId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario actual con ID: {UserId}", userId);
                return null;
            }
        }

        /// <summary>
        /// Verifica si hay un usuario autenticado
        /// </summary>
        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
        }
    }
}