using Microsoft.AspNetCore.SignalR;
using UmesiServer.Data;
using UmesiServer.Data.NotificationService;

namespace UmesiServer.Hubs.NotificationHub
{
    public class NotificationHub : Hub<IClientNotificationHub>
    {
        private ILogger<NotificationHub> _logger;
        private INotificationService _notificationService;

        public NotificationHub(ILogger<NotificationHub> logger, UnitOfWork unit)
        {
            _logger = logger;
            _notificationService = unit.NotificationService;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _notificationService.UnsubscribeUser(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterUserForNotifications(string username)
        {
            try
            {
                await _notificationService.RegisterUser(username, Context.ConnectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
