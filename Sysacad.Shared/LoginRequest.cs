namespace Sysacad.Shared
{
    /// <summary>
    /// Modelo de solicitud de login
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
