namespace Sysacad.Server.Services
{
    /// <summary>
    /// Servicio para hashear y verificar contraseñas usando BCrypt
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashea una contraseña en texto plano
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifica si una contraseña coincide con un hash
        /// </summary>
        bool VerifyPassword(string password, string passwordHash);
    }
}
