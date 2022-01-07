using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using UmesiServer.Constants;
using UmesiServer.DTOs.Records;
using UmesiServer.Exceptions;
using UmesiServer.Models;

namespace UmesiServer.Data.RecipeRepository
{
    public class RecipeRepository : IRecipeRepository
    {
        private ConnectionMultiplexer _redis;
        private UnitOfWork _unitOfWork;

        public RecipeRepository(ConnectionMultiplexer redis, UnitOfWork unit)
        {
            _redis = redis;
            _unitOfWork = unit;
        }

        public async Task<Recipe> GetRecipe(int id)
        {
            if (id < 0)
                throw new HttpResponseException(400, "Identifier out of bounds");
            IDatabase db = _redis.GetDatabase();
            Recipe recipe = (await db.ListRangeAsync(ListConsts.RecipeListKey)).ToList().Select(r => JsonSerializer.Deserialize<Recipe>(r)).Where(r => r.Id == id && r.IsDeleted == 0).FirstOrDefault();
            if (recipe == null)
                throw new HttpResponseException(404, "Recipe does not exist");
            List<RedisValue> redisComments = (await db.ListRangeAsync(recipe.CommentListKey)).ToList();
            recipe.Comments = redisComments.Select(c => JsonSerializer.Deserialize<Comment>(c.ToString())).ToList();
            return recipe;
        }

        public async Task<List<Recipe>> GetAllRecipes()
        {
            IDatabase db = _redis.GetDatabase();
            List<RedisValue> redisRecipes = (await db.ListRangeAsync(ListConsts.RecipeListKey)).ToList();
            List<Recipe> recipes = new List<Recipe>();
            if (redisRecipes.Count == 0)
                return recipes;
            recipes = redisRecipes.Select(rv => JsonSerializer.Deserialize<Recipe>(rv.ToString())).Where(r => r.IsDeleted == 0).ToList();
            return recipes;
        }

        public async Task AddRecipe(Recipe recipe)
        {
            if (recipe.Title == null)
                throw new HttpResponseException(400, "Recipe missing required fields");
            IDatabase db = _redis.GetDatabase();
            recipe.Id = await _unitOfWork.IdGenerator.GetRecipeId();
            string jsonRecipe = JsonSerializer.Serialize<Recipe>(recipe);
            string jsonUser = await db.StringGetAsync(recipe.UserRef);
            if (string.IsNullOrEmpty(jsonUser))
                throw new HttpResponseException(404, "User with given username does not exist.");
            await db.ListLeftPushAsync(ListConsts.RecipeListKey, jsonRecipe);
            
            User owner = JsonSerializer.Deserialize<User>(jsonUser);
            await db.ListRightPushAsync(owner.CreatedRecipesKey, recipe.Id.ToString());
        }

        public Task UpdateRecipe(string username, Recipe recipe)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRecipe(int id)
        {
            throw new NotImplementedException();
        }
    }
}