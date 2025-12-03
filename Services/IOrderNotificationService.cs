using SimpleShop.Models;

namespace SimpleShop.Services
{
    /// <summary>
    /// Отправляет пользователям сводку по заказу по настроенному каналу уведомлений.
    /// </summary>
    public interface IOrderNotificationService
    {
        Task NotifyAsync(Order order, string recipientEmail, CancellationToken cancellationToken = default);
    }
}


