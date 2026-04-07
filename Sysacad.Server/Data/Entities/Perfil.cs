namespace Sysacad.Server.Data.Entities
{
    public class Perfil : GenericEntity, IAudithory
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public virtual Usuario CreatedBy { get; set; } = null!;
        public int CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Usuario? UpdatedBy { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual Usuario? DeletedBy { get; set; }
        public int? DeletedById { get; set; }
        public DateTime? DeletedAt { get; set; }
        public virtual List<Permiso>? Permisos { get; set; } = null;
        public virtual List<Usuario>? Usuarios { get; set; } = null;
    }
}
