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
        private UnitOfWork _unitOfWork;

        public UserRepository(ConnectionMultiplexer redis, UnitOfWork unit)
        {
            _redis = redis;
            _unitOfWork = unit;
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
            user.CreatedRecipes = (await db.ListRangeAsync(user.CreatedRecipesKey))
                .Select(rv => int.Parse(rv.ToString()))
                .Select(rid => db.ListRange(ListConsts.RecipeListKey)
                                        .Select(rv => JsonSerializer.Deserialize<Recipe>(rv.ToString()))
                                        .Where(r => r.Id == rid)
                                        .Single()).ToList();
            user.FavoriteRecipes = (await db.ListRangeAsync(user.FavoriteRecipesKey))
                .Select(rv => int.Parse(rv.ToString()))
                .Select(rid => db.ListRange(ListConsts.RecipeListKey)
                                        .Select(rv => JsonSerializer.Deserialize<Recipe>(rv.ToString()))
                                        .Where(r => r.Id == rid)
                                        .Single()).ToList();
            user.FollowedUsers = (await db.ListRangeAsync(user.FollowedUsersKey)).Select(rv => rv.ToString()).ToList();
            return user;
        }

        public async Task AddUser(User user)
        {
            if (string.IsNullOrEmpty(user.Username))
                throw new HttpResponseException(400, "Bad dto");
            IDatabase db = _redis.GetDatabase();
            string existingUser = await db.StringGetAsync(user.Username);
            if (!string.IsNullOrEmpty(existingUser))
            {
                throw new HttpResponseException(409, "User already exists.");
            }
            string jsonUser = JsonSerializer.Serialize<User>(user);
            await db.StringSetAsync(user.Username, jsonUser);
        }

        public async Task<User> UpdateUser(User user)
        {
            if (string.IsNullOrEmpty(user.Username))
                throw new HttpResponseException(400, "User dto is not ok");

            IDatabase db = _redis.GetDatabase();
            string redisUser = await db.StringGetAsync(user.Username);
            
            if (string.IsNullOrEmpty(redisUser))
                throw new HttpResponseException(404, "User does not exist");
            
            User oldUser = JsonSerializer.Deserialize<User>(redisUser);

            oldUser.Password = string.IsNullOrEmpty(user.Password) ? oldUser.Password : user.Password;
            oldUser.Name = string.IsNullOrEmpty(user.Name) ? oldUser.Name : user.Name;
            oldUser.Surname= string.IsNullOrEmpty(user.Surname) ? oldUser.Surname: user.Surname;

            await db.StringSetAsync(oldUser.Username, JsonSerializer.Serialize<User>(oldUser));
            return oldUser;
        }

        public async Task DeleteUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new HttpResponseException(400, "username is null");

            IDatabase db = _redis.GetDatabase();
            string redisUser = (await db.StringGetAsync(username)).ToString();

            if (string.IsNullOrEmpty(redisUser))
                throw new HttpResponseException(404, "User does not exist");
            
            User user = JsonSerializer.Deserialize<User>(redisUser);
            await _unitOfWork.RecipeRepository.DeleteAllCreatedRecipesOfUser(user.CreatedRecipesKey);
            await db.KeyDeleteAsync(username);
        }

        public async Task AddRecipeToFavorites(string username, int recipeId)
        {
            if (string.IsNullOrEmpty(username) || recipeId <= 0)
                throw new HttpResponseException(400, "Bad request");
            IDatabase db = _redis.GetDatabase();
            string redisUser = await db.StringGetAsync(username);
            if (string.IsNullOrEmpty(redisUser))
                throw new HttpResponseException(404, "User does not exist");
            User user = JsonSerializer.Deserialize<User>(redisUser);
            Recipe recipe = (await db.ListRangeAsync(ListConsts.RecipeListKey))
                .Select(rv => JsonSerializer.Deserialize<Recipe>(rv.ToString()))
                .Where(r => r.Id == recipeId && r.IsDeleted == 0).SingleOrDefault();
            if (recipe == null)
                throw new HttpResponseException(404, "Recipe not found");
            await db.ListLeftPushAsync(user.FavoriteRecipesKey, recipeId.ToString());
            await _unitOfWork.NotificationService.AddSubscription(username, recipeId.ToString());
        }

        public async Task FollowUser(string currentUser, string userToFollow)
        {
            if (string.IsNullOrEmpty(currentUser) || string.IsNullOrEmpty(userToFollow))
                throw new HttpResponseException(400, "Data is not valid");
            IDatabase db = _redis.GetDatabase();

            string redisCurUser = await db.StringGetAsync(currentUser);
            if (string.IsNullOrEmpty(redisCurUser))
                throw new HttpResponseException(404, "Current user does not exist");

            string redisUserToFollow = await db.StringGetAsync(userToFollow);
            if (string.IsNullOrEmpty(redisUserToFollow))
                throw new HttpResponseException(404, "User you want to follow does not exist");

            User loggedInUser = JsonSerializer.Deserialize<User>(redisCurUser);
            await db.ListLeftPushAsync(loggedInUser.FollowedUsersKey, userToFollow);
            await _unitOfWork.NotificationService.AddSubscription(currentUser, userToFollow);
        }
    }
}