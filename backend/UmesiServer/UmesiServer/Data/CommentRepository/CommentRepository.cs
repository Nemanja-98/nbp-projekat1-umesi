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
        private UnitOfWork _unitOfWork;

        public CommentRepository(ConnectionMultiplexer redis, UnitOfWork unit)
        {
            _redis = redis;
            _unitOfWork = unit;
        }

        public async Task<List<Comment>> GetCommentsForRecipe(int recipeId)
        {
            IDatabase db = _redis.GetDatabase();
            if (recipeId < 0)
                throw new HttpResponseException(400, "Identifier out of bounds");
            Recipe recipe = (await db.ListRangeAsync(ListConsts.RecipeListKey)).ToList().Select(r => JsonSerializer.Deserialize<Recipe>(r.ToString())).Where(r => r.Id == recipeId).FirstOrDefault();
            if (recipe == null)
                throw new HttpResponseException(404, "Recipe not found");
            List<RedisValue> redisComments = (await db.ListRangeAsync(recipe.CommentListKey)).ToList();
            if (redisComments.Count == 0)
                return new List<Comment>();
            return redisComments.Select(c => JsonSerializer.Deserialize<Comment>(c.ToString())).ToList();
        }

        public async Task AddComment(int recipeId, Comment comment)
        {
            if (string.IsNullOrEmpty(comment.UserRef))
                throw new HttpResponseException(400, "Dto missing required fields");
            IDatabase db = _redis.GetDatabase();
            Recipe recipe = (await db.ListRangeAsync(ListConsts.RecipeListKey)).ToList().Select(r => JsonSerializer.Deserialize<Recipe>(r.ToString())).Where(r => r.Id == recipeId).FirstOrDefault();
            if (recipe == null)
                throw new HttpResponseException(404, "Recipe with given id does not exist");
            await db.ListLeftPushAsync(recipe.CommentListKey, JsonSerializer.Serialize<Comment>(comment));
            _unitOfWork.NotificationService.Notify(recipeId.ToString(), "Comment added", $"{comment.UserRef} has commented on {recipe.Title}.");
        }

        public async Task<Comment> UpdateComment(int recipeId, int index, Comment comment)
        {
            if(recipeId <= 0 || index < 0)
                throw new HttpResponseException(400, "Recipe id or index out of bounds");
            if (comment.UserRef == null)
                throw new HttpResponseException(400, "Dto is not valid");
            IDatabase db = _redis.GetDatabase();
            Recipe recipe = (await db.ListRangeAsync(ListConsts.RecipeListKey))
                .Select(rv => JsonSerializer.Deserialize<Recipe>(rv.ToString()))
                .Where(r => r.Id == recipeId && r.IsDeleted == 0)
                .SingleOrDefault();
            if (recipe == null)
                throw new HttpResponseException(404, "Recipe not found");
            RedisValue redisComment = await db.ListGetByIndexAsync(recipe.CommentListKey, index);
            if (!redisComment.HasValue)
                throw new HttpResponseException(404, "Comment not found");
            Comment oldComment = JsonSerializer.Deserialize<Comment>(redisComment.ToString());
            if (oldComment.IsDeleted != 0)
                throw new HttpResponseException(403, "Deleted comments can't be updated");
            oldComment.Description = string.IsNullOrEmpty(comment.Description) ? oldComment.Description : comment.Description;
            await db.ListSetByIndexAsync(recipe.CommentListKey, index, JsonSerializer.Serialize<Comment>(oldComment));
            return oldComment;
        }

        public async Task DeleteComment(int recipeId, int index)
        {
            if (recipeId <= 0 || index < 0)
                throw new HttpResponseException(400, "Recipe id or index out of bounds");
            IDatabase db = _redis.GetDatabase();
            Recipe recipe = (await db.ListRangeAsync(ListConsts.RecipeListKey))
                .Select(rv => JsonSerializer.Deserialize<Recipe>(rv.ToString()))
                .Where(r => r.Id == recipeId && r.IsDeleted == 0)
                .SingleOrDefault();
            if (recipe == null)
                throw new HttpResponseException(404, "Recipe not found");
            RedisValue redisComment = await db.ListGetByIndexAsync(recipe.CommentListKey, index);
            if (!redisComment.HasValue)
                throw new HttpResponseException(404, "Comment not found");
            Comment comment = JsonSerializer.Deserialize<Comment>(redisComment.ToString());
            if (comment.IsDeleted != 0)
                throw new HttpResponseException(403, "Comment already deleted");
            comment.IsDeleted = 1;
            await db.ListSetByIndexAsync(recipe.CommentListKey, index, JsonSerializer.Serialize<Comment>(comment));
        }

        public async Task DeleteAllCommentsForKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new HttpResponseException(400, "Comments key can not be null");
            IDatabase db = _redis.GetDatabase();
            List<Comment> comments = (await db.ListRangeAsync(key)).Select(rv => JsonSerializer.Deserialize<Comment>(rv.ToString())).ToList();
            if (comments.Count == 0)
                return;
            for(int i=0; i<comments.Count; i++)
            {
                Comment comment = comments[i];
                comment.IsDeleted = 1;
                await db.ListSetByIndexAsync(key, i, JsonSerializer.Serialize<Comment>(comment));
            }
        }
    }
}