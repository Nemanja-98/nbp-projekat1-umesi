using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using UmesiServer.Constants;
using UmesiServer.Exceptions;
using UmesiServer.Models;

namespace UmesiServer.Data.CommentRepository
{
    public class CommentRepository : ICommentRepository
    {
        private ConnectionMultiplexer _redis;
        public CommentRepository(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<List<Comment>> GetCommentsForRecipe(int recipeId)
        {
            IDatabase db = _redis.GetDatabase();
            if (recipeId < 0)
                throw new HttpResponseException(400, "Identifier our of bounds");
            Recipe recipe = (await db.ListRangeAsync(ListConsts.RecipeListKey)).ToList().Select(r => JsonSerializer.Deserialize<Recipe>(r.ToString())).Where(r => r.Id == recipeId).FirstOrDefault();
            if (recipe == null)
                throw new HttpResponseException(404, "Recipe not found");
            List<RedisValue> redisComments = (await db.ListRangeAsync(recipe.CommentListKey)).ToList();
            if (redisComments.Count > 0)
                return new List<Comment>();
            return redisComments.Select(c => JsonSerializer.Deserialize<Comment>(c.ToString())).ToList();
        }

        public async Task AddComment(int recipeId, Comment comment)
        {
            if (comment == null)
                throw new HttpResponseException(400, "Dto can't be null");
            IDatabase db = _redis.GetDatabase();
            Recipe recipe = (await db.ListRangeAsync(ListConsts.RecipeListKey)).ToList().Select(r => JsonSerializer.Deserialize<Recipe>(r.ToString())).Where(r => r.Id == recipeId).FirstOrDefault();
            if (recipe == null)
                throw new HttpResponseException(404, "Recipe with given id does not exist");
            await db.ListLeftPushAsync(recipe.CommentListKey, JsonSerializer.Serialize<Comment>(comment));
        }

        public Task<Comment> UpdateComment(int recipeId, int index, Comment comment)
        {
            throw new NotImplementedException();
        }

        public Task DeleteComment(int recipeId, int index)
        {
            throw new NotImplementedException();
        }
    }
}