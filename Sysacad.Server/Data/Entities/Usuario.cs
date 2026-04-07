
using Sysacad.Server.Data.Attributes;
using Sysacad.Server.Data.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sysacad.Server.Data.Entities
{
    [Repository(typeof(UsuarioRepository))]
    [Table("Usuario")]
    public class Usuario : GenericEntity, IAudithory
    {
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;
        public virtual Usuario CreatedBy { get; set; } = null!;
        public required int CreatedById { get; set; }
        public required DateTime CreatedAt { get; set; }
        public virtual Usuario? UpdatedBy { get; set; } = null;
        public int? UpdatedById { get; set; } = null;
        public DateTime? UpdatedAt { get; set; } = null;
        public virtual Usuario? DeletedBy { get; set; } = null;
        public int? DeletedById { get; set; } = null;
        public DateTime? DeletedAt { get; set; } = null;
        public virtual List<Perfil>? Perfiles { get; set; } = null;
        public virtual Persona? Persona { get; set; } = null;
        public int? PersonaId { get; set; }
    }
}
