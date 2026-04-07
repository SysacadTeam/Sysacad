namespace Sysacad.Server.Data.Entities
{
    public class Permiso : GenericEntity, IAudithory
    {
        public required string Nombre { get; set; } = string.Empty;
        public required string Codigo { get; set; } = string.Empty;
        public required string Descripcion { get; set; } = string.Empty;
        public virtual Usuario CreatedBy { get; set; } = null!;
        public required int CreatedById { get; set; }
        public required DateTime CreatedAt { get; set; }
        public virtual Usuario? UpdatedBy { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual Usuario? DeletedBy { get; set; }
        public int? DeletedById { get; set; }
        public DateTime? DeletedAt { get; set; }
        public virtual List<Perfil>? Perfiles { get; set; } = null;
    }
}
