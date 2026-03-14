using Microsoft.EntityFrameworkCore;
using Npgsql;
using Sysacad.Server.Data;
using Sysacad.Server.Data.Repositories;
using System.Text.RegularExpressions;

namespace Sysacad.Server.Services
{
    public class SessionService : ISessionService
    {
        private readonly ILogger<SessionService> _logger;
        private readonly EntityManager _entityManager;
        private readonly IPasswordHasher _passwordHasher;
        private static readonly Regex BcryptPattern = new Regex(@"^\$2[ayb]\$.{56}$", RegexOptions.Compiled);

        public SessionService(ILogger<SessionService> logger, EntityManager entityManager, IPasswordHasher passwordHasher)
        {
            _logger = logger;
            _entityManager = entityManager;
            _passwordHasher = passwordHasher;
        }

        public async Task<(int? userId, string? errorMessage)> ValidateCredentialsAsync(string username, string password)
        {
            try
            {
                var repository = _entityManager.GetRepository<UsuarioRepository>();

                var usuario = await repository.GetByUsernameAsync(username);

                if (usuario == null)
                {
                    _logger.LogWarning("Intento de login fallido. Usuario no encontrado: {Username}", username);
                    return (null, "Usuario o contraseña incorrectos");
                }

                bool dbPasswordIsHashed = IsPasswordHashed(usuario.PasswordHash);

                if (!dbPasswordIsHashed)
                {
                    _logger.LogWarning("Password en DB no está hasheado para usuario: {Username}. Hasheando automáticamente...", username);

                    usuario.PasswordHash = _passwordHasher.HashPassword(usuario.PasswordHash);
                    await repository.UpdateAsync(usuario, usuario);

                    _logger.LogInformation("Password hasheado y actualizado para usuario: {Username}", username);
                }

                bool frontendPasswordIsHashed = IsPasswordHashed(password);

                bool isPasswordValid;

                if (frontendPasswordIsHashed)
                {
                    _logger.LogWarning("El frontend envió un password hasheado para usuario: {Username}", username);
                    isPasswordValid = password == usuario.PasswordHash;
                }
                else
                {
                    isPasswordValid = _passwordHasher.VerifyPassword(password, usuario.PasswordHash);
                }

                if (!isPasswordValid)
                {
                    _logger.LogWarning("Intento de login fallido. Contraseña incorrecta para usuario: {Username}", username);
                    return (null, "Usuario o contraseña incorrectos");
                }

                usuario.LastLogin = DateTime.UtcNow;
                await repository.UpdateAsync(usuario, usuario);

                _logger.LogInformation("Login exitoso para usuario ID: {UserId}, Username: {Username}", usuario.Id, usuario.Username);

                return (usuario.Id, null);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Error de conexión a la base de datos al intentar login para usuario: {Username}", username);
                return (null, "Error de conexión con la base de datos. Por favor, intente nuevamente.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al actualizar la base de datos para usuario: {Username}", username);
                return (null, "Error al procesar la solicitud. Por favor, intente nuevamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al validar credenciales para usuario: {Username}", username);
                return (null, "Error interno del servidor. Por favor, contacte al administrador.");
            }
        }

        private bool IsPasswordHashed(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            return BcryptPattern.IsMatch(password);
        }
    }
}