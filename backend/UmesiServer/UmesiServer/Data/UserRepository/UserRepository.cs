using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using UmesiServer.Constants;
using UmesiServer.Exceptions;
using UmesiServer.Models;

namespace UmesiServer.Data.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private ConnectionMultiplexer _redis;

        public UserRepository(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<User> GetUser(string username)
        {
            IDatabase db = _redis.GetDatabase();
            string jsonUser = await db.StringGetAsync(username);
            if(string.IsNullOrEmpty(jsonUser))
            {
                throw new HttpResponseException(404, "User does not exist");
            }
            User user = JsonSerializer.Deserialize<User>(jsonUser);
            user.CreatedRecipes = (await db.ListRangeAsync(user.CreatedRecipesKey)).Select(r => JsonSerializer.Deserialize<Recipe>(r.ToString())).ToList();
            user.FavoriteRecipes = (await db.ListRangeAsync(user.FavoriteRecipesKey)).Select(r => JsonSerializer.Deserialize<Recipe>(r.ToString())).ToList();
            return user;
        }

        public async Task<List<User>> GetAllUsers()
        {
            IDatabase db = _redis.GetDatabase();
            List<RedisValue> redisUsers = (await db.ListRangeAsync(ListConsts.UserListKey)).ToList();
            
            List<User> users = new List<User>();
            if (redisUsers.Count == 0)
                return users;
            foreach (RedisValue redisUser in redisUsers)
                users.Add(JsonSerializer.Deserialize<User>(await db.StringGetAsync(redisUser.ToString())));
            return users;
        }

        public async Task AddUser(User user)
        {
            if (user == null)
                throw new HttpResponseException(400, "User is null");
            IDatabase db = _redis.GetDatabase();
            string existingUser = await db.StringGetAsync(user.Username);
            if (!string.IsNullOrEmpty(existingUser))
            {
                throw new HttpResponseException(409, "User already exists.");
            }
            string jsonUser = JsonSerializer.Serialize<User>(user);
            await db.StringSetAsync(user.Username, jsonUser);
            await db.ListRightPushAsync(ListConsts.UserListKey, user.Username);
        }

        public Task UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUser(string username)
        {
            throw new NotImplementedException();
        }

    }
}