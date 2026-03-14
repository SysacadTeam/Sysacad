using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sysacad.Server.Data;
using Sysacad.Server.Data.Entities;
using Sysacad.Server.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Sysacad.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class GenericController : ControllerBase
    {
        protected EntityManager _entityManager;
        protected ICurrentUserService _currentUserService;

        protected GenericController(EntityManager entityManager, ICurrentUserService currentUserService)
        {
            _entityManager = entityManager;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Obtiene el ID del usuario autenticado actual
        /// </summary>
        protected int? GetCurrentUserId() => _currentUserService.GetCurrentUserId();

        /// <summary>
        /// Obtiene el usuario autenticado actual completo
        /// </summary>
        protected Task<Usuario?> GetCurrentUserAsync() => _currentUserService.GetCurrentUserAsync();
    }
}




