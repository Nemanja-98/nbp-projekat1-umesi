namespace UmesiServer.Data.NotificationService
{
    public interface INotificationService
    {
        void Notify(string topic, string title , string message);
        Task RegisterUser(string username, string connectionId);
        void UnsubscribeUser(string connectionId);
        Task AddSubscription(string username, string topic);
    }
}
