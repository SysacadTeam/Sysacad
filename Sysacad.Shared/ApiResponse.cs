namespace Sysacad.Shared
{
    /// <summary>
    /// Modelo de respuesta genérica de la API
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Indica si la operación fue exitosa
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo del resultado
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}