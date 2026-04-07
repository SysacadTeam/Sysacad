using Sysacad.Server.Data.Attributes;
using Sysacad.Server.Data.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sysacad.Server.Data.Entities
{
    [Repository(typeof(PersonaRepository))]
    public class Persona : GenericEntity, IAudithory
    {
        public required string Nombres { get; set; }
        public required string Apellidos { get; set; }
        public required string Documento { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public string? Cobertura { get; set; }
        public string? NroAfiliado { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Domicilio { get; set; }
        public virtual Usuario CreatedBy { get; set; } = null!;
        public required int CreatedById { get; set; }
        public required DateTime CreatedAt { get; set; }
        public virtual Usuario? UpdatedBy { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual Usuario? DeletedBy { get; set; }
        public int? DeletedById { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
