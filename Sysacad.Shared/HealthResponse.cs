namespace Sysacad.Shared
{
    /// <summary>
    /// Modelo de respuesta del health check
    /// </summary>
    public class HealthResponse
    {
        /// <summary>
        /// Estado de salud del servicio
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Marca de tiempo de la verificación
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Nombre del servicio
        /// </summary>
        public string Service { get; set; } = string.Empty;

        /// <summary>
        /// Versión del servicio
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Entorno de ejecución
        /// </summary>
        public string Environment { get; set; } = string.Empty;
    }
}