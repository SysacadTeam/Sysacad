using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sysacad.Server.Data;
using Sysacad.Shared;
using Sysacad.Server.Services;
using System.IdentityModel.Tokens.Jwt;
using Sysacad.Shared.Auth;

namespace Sysacad.Server.Controllers
{
    /// <summary>
    /// Controlador de autenticación para login, logout y validación de tokens
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : GenericController
    {
        private readonly ISessionService _sessionService;
        private readonly IJwtService _jwtService;

        /// <summary>
        /// Constructor del controlador de autenticación
        /// </summary>
        /// <param name="entityManager">Gestor de entidades</param>
        /// <param name="currentUserService">Servicio del usuario actual</param>
        /// <param name="sessionService">Servicio de sesión</param>
        /// <param name="jwtService">Servicio de tokens JWT</param>
        public AuthController(EntityManager entityManager, ICurrentUserService currentUserService, ISessionService sessionService, IJwtService jwtService) : base(entityManager, currentUserService)
        {
            _sessionService = sessionService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Autentica un usuario y retorna un token JWT
        /// </summary>
        /// <param name="request">Credenciales del usuario</param>
        /// <returns>Token JWT válido</returns>
        /// <response code="200">Login exitoso con token JWT</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="401">Credenciales incorrectas</response>
        /// <response code="503">Error de conexión a base de datos</response>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = "Usuario y contraseña son requeridos"
                });
            }

            var (userId, errorMessage) = await _sessionService.ValidateCredentialsAsync(request.Username, request.Password);

            if (userId == null)
            {
                if (errorMessage?.Contains("base de datos") == true || errorMessage?.Contains("conexión") == true)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new LoginResponse
                    {
                        Success = false,
                        Message = errorMessage
                    });
                }

                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = errorMessage ?? "Usuario o contraseña incorrectos"
                });
            }

            var token = _jwtService.GenerateToken(userId.Value);

            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Login exitoso",
                Token = token
            });
        }

        /// <summary>
        /// Cierra la sesión del usuario actual
        /// </summary>
        /// <remarks>
        /// Con JWT stateless, el cliente debe eliminar el token del almacenamiento local.
        /// El token seguirá siendo válido hasta su expiración natural.
        /// </remarks>
        /// <response code="200">Logout exitoso</response>
        /// <response code="401">No autorizado</response>
        [HttpPost("LogOut")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public IActionResult Logout()
        {
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Logout exitoso"
            });
        }

        /// <summary>
        /// Valida un token JWT y retorna información del usuario
        /// </summary>
        /// <remarks>
        /// Verifica que el token sea válido y no haya expirado.
        /// El refresh de tokens se maneja automáticamente via middleware.
        /// </remarks>
        /// <response code="200">Token válido</response>
        /// <response code="401">Token inválido o expirado</response>
        [HttpGet("Validate")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ValidateToken()
        {
            var userIdClaim = User.FindFirst("id")?.Value
                           ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { Success = false, Message = "Token sin userId válido" });
            }

            return Ok(new { Success = true, UserId = userId });
        }

        
    }
}


