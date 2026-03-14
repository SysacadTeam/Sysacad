using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Sysacad.Server.Data;
using Sysacad.Server.Data.Entities;
using Sysacad.Server.Data.Repositories;
using Sysacad.Server.Services;

namespace Sysacad.Server.Controllers
{
    /// <summary>
    /// Controlador para gestión de usuarios
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : GenericController
    {
        /// <summary>
        /// Constructor del controlador de usuarios
        /// </summary>
        /// <param name="entityManager">Gestor de entidades</param>
        /// <param name="currentUserService">Servicio del usuario actual</param>
        public UsuarioController(EntityManager entityManager, ICurrentUserService currentUserService)
            : base(entityManager, currentUserService)
        {
        }
    }
}

