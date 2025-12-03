using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleShop.Models;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SimpleShop.Services
{
    /// <summary>
    /// Реализация сервиса уведомлений на основе SMTP с безопаснымFallback в логирование.
    /// </summary>
    public class EmailOrderNotificationService : IOrderNotificationService
    {
        private readonly EmailSettings _options;
        private readonly ILogger<EmailOrderNotificationService> _logger;

        public EmailOrderNotificationService(IOptions<EmailSettings> options, ILogger<EmailOrderNotificationService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task NotifyAsync(Order order, string recipientEmail, CancellationToken cancellationToken = default)
        {
            var recipient = string.IsNullOrWhiteSpace(_options.RecipientOverride)
                ? recipientEmail
                : _options.RecipientOverride;

            var subject = $"SimpleShop · заказ №{order.Id}";
            var body = BuildBody(order);

            if (string.IsNullOrWhiteSpace(_options.SmtpHost))
            {
                _logger.LogInformation("SMTP не настроен. Отправка письма пропущена. Получатель: {Recipient}. Сообщение: {Body}",
                    recipient, body);
                return;
            }

            using var client = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
            {
                EnableSsl = _options.UseSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            if (!string.IsNullOrWhiteSpace(_options.Username) && !string.IsNullOrWhiteSpace(_options.Password))
            {
                client.Credentials = new NetworkCredential(_options.Username, _options.Password);
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_options.Sender, _options.SenderDisplayName),
                Subject = subject,
                Body = body,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            message.To.Add(recipient);

            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Email с подтверждением заказа {OrderId} отправлен на {Recipient}", order.Id, recipient);
        }

        private static string BuildBody(Order order)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Здравствуйте, {order.FullName}!");
            sb.AppendLine($"Вы успешно оформили заказ №{order.Id} от {order.CreatedAtUtc:dd.MM.yyyy HH:mm} UTC.");
            sb.AppendLine();
            sb.AppendLine("Состав заказа:");
            foreach (var item in order.Items)
            {
                sb.AppendLine($" • {item.ProductName} — {item.Quantity} шт × {item.UnitPrice:C} = {(item.UnitPrice * item.Quantity):C}");
            }
            sb.AppendLine();
            sb.AppendLine($"Итого: {order.TotalAmount:C}");
            sb.AppendLine($"Способ доставки: {order.DeliveryMethod}");
            sb.AppendLine($"Способ оплаты: {order.PaymentMethod}");
            sb.AppendLine();
            sb.AppendLine("Спасибо за покупку в SimpleShop!");
            return sb.ToString();
        }
    }
}


