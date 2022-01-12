using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System.Text.Json;
using UmesiServer.DTOs.Records;
using UmesiServer.Hubs.NotificationHub;
using UmesiServer.Models;

namespace UmesiServer.Data.NotificationService
{
    public class NotificationService : INotificationService
    {
        private ConnectionMultiplexer _redis;
        private IHubContext<NotificationHub> _notificationContext;
        private ILogger<UnitOfWork> _logger;
        private Dictionary<string, ConnIdAAndSubscriberRecord> _openSubscriptions;

        public NotificationService(ConnectionMultiplexer redis, IHubContext<NotificationHub> context, ILogger<UnitOfWork> logger)
        {
            _redis = redis;
            _notificationContext = context;
            _logger = logger;
            _openSubscriptions = new Dictionary<string, ConnIdAAndSubscriberRecord>();
        }

        public async Task RegisterUser(string username, string connectionId)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(connectionId))
                throw new Exception("RegisterUser failed because username or connectionId is null or empty");

            IDatabase db = _redis.GetDatabase();
            string redisUser = (await db.StringGetAsync(username)).ToString();

            if (string.IsNullOrEmpty(redisUser))
                throw new Exception("User does not exist");

            try
            {
                User user = JsonSerializer.Deserialize<User>(redisUser);
                ISubscriber subscriber = _redis.GetSubscriber();
                List<string> followedUsers = (await db.ListRangeAsync(user.FollowedUsersKey))
                    .Select(rv => rv.ToString())
                    .ToList();

                foreach (string followedUser in followedUsers)
                    (await subscriber.SubscribeAsync(followedUser))
                        .OnMessage(async message =>
                        {
                            NotificationDto notification = JsonSerializer.Deserialize<NotificationDto>(message.ToString());
                            await _notificationContext.Clients.Client(connectionId).SendAsync("notify", notification.Title, notification.Message);
                        });

                List<int> favoriteRecipes = (await db.ListRangeAsync(user.FavoriteRecipesKey))
                    .Select(rv => int.Parse(rv.ToString()))
                    .ToList();

                foreach (int favoriteRecipe in favoriteRecipes)
                    (await subscriber.SubscribeAsync(favoriteRecipe.ToString()))
                        .OnMessage(async message =>
                        {
                            NotificationDto notification = JsonSerializer.Deserialize<NotificationDto>(message.ToString());
                            await _notificationContext.Clients.Client(connectionId).SendAsync("notify", notification.Title, notification.Message);
                        });

                _openSubscriptions.Add(user.Username, new ConnIdAAndSubscriberRecord(connectionId, subscriber));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async void Notify(string topic, string title, string message)
        {
            try
            {
                ISubscriber publisher = _redis.GetSubscriber();
                await publisher.PublishAsync(topic, JsonSerializer.Serialize<NotificationDto>(new NotificationDto(title, message)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async void UnsubscribeUser(string connectionId)
        {
            try
            {
                if (!_openSubscriptions.ContainsKey(connectionId))
                    return;
                KeyValuePair<string, ConnIdAAndSubscriberRecord>? keyValuePair = _openSubscriptions.Select(kvp => kvp).Where(kvp => kvp.Value.ConnectionId == connectionId).SingleOrDefault();
                if (!keyValuePair.HasValue)
                    return;
                await keyValuePair.Value.Value.subscriber.UnsubscribeAllAsync();
                _openSubscriptions.Remove(keyValuePair.Value.Key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task AddSubscription(string username, string topic)
        {
            try
            {
                if (!_openSubscriptions.ContainsKey(username))
                    return;
                (await _openSubscriptions[username].subscriber.SubscribeAsync(topic))
                        .OnMessage(async message =>
                        {
                            NotificationDto notification = JsonSerializer.Deserialize<NotificationDto>(message.ToString());
                            await _notificationContext.Clients.Client(_openSubscriptions[username].ConnectionId).SendAsync("notify", notification.Title, notification.Message);
                        });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
