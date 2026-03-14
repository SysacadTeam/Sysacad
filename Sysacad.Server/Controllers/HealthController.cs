using Microsoft.AspNetCore.Mvc;

using Sysacad.Shared;

namespace Sysacad.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Health check endpoint para verificar que la API est� funcionando
        /// </summary>
        /// <remarks>
        /// Este endpoint retorna el estado de salud de la aplicaci�n.
        /// 
        /// **Uso:**
        /// - Monitoreo en Render (health check autom�tico)
        /// - Verificaci�n manual de disponibilidad
        /// - Testing de conectividad
        /// 
        /// **No requiere autenticaci�n.**
        /// </remarks>
        /// <response code="200">La API est� saludable y funcionando correctamente</response>
        [HttpGet]
        [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(new HealthResponse
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Service = "Sysacad.Server",
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }
    }
}

