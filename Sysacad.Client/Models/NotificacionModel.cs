using Sysacad.Client.Services;

namespace Sysacad.Client.Models
{
    public class NotificacionModel
    {
        public string Message { get; set; } = string.Empty;
        public string? Description { get; set; }
        public NotificationType Type { get; set; }
        public int Duration { get; set; } = 0;
    }
}
