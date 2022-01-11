namespace UmesiServer.Hubs.NotificationHub
{
    public interface IClientNotificationHub
    {
        Task NotifyUser(string connectionId, string title, string message);
    }
}
