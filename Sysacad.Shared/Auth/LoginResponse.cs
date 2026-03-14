namespace Sysacad.Shared.Auth
{
    /// <summary>
    /// Modelo de respuesta de login
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Indica si el login fue exitoso
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo del resultado
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Token JWT generado (solo si el login fue exitoso)
        /// </summary>
        public string? Token { get; set; }
    }
}
