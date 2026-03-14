using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Sysacad.Server.Data.Entities
{
    public abstract class GenericEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public bool BorradoLogico { get; set; }
    }
}
