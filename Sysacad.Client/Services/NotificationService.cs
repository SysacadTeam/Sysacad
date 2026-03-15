using Sysacad.Client.Attributes;
using Sysacad.Client.Components.Utils;
using Sysacad.Client.Models;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Sysacad.Client.Services
{
    [RegisterService(ServiceLifetime.Scoped)]
    public class NotificationService
    {
        public event Action? OnNotificationAdded;

        private readonly List<NotificacionModel> _notifications = new();
        public IReadOnlyList<NotificacionModel> Notifications => _notifications;

        public void AddNotification(NotificacionModel notification)
        {
            _notifications.Add(notification);
            OnNotificationAdded?.Invoke();

            if (notification.Duration > 0)
            {
                var timer = new System.Timers.Timer(notification.Duration * 1000);
                timer.Elapsed += (s, e) => RemoveNotification(notification);
                timer.AutoReset = false;
                timer.Start();
            }
        }

        public void RemoveNotification(NotificacionModel notification)
        {
            _notifications.Remove(notification);
            OnNotificationAdded?.Invoke();
        }

        public List<NotificacionModel> GetNotifications()
        {
            return _notifications;
        }
    }

    public enum NotificationType
    {
        Error,
        Warning,
        Info,
        Success
    }
}