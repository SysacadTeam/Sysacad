namespace Sysacad.Server.Data.Entities
{
    public interface IAudithory
    {
        public Usuario CreatedBy { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public Usuario? UpdatedBy { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdatedAt { get; set; }
        bool BorradoLogico { get; set; }
        public Usuario? DeletedBy { get; set; }
        public int? DeletedById { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
