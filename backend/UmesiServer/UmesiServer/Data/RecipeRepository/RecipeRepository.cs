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
            recipe.Comments = redisComments
                .Select(rv => JsonSerializer.Deserialize<Comment>(rv.ToString()))
                .ToList();
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
            string jsonUser = await db.StringGetAsync(recipe.UserRef);
            
            if (string.IsNullOrEmpty(jsonUser))
                throw new HttpResponseException(404, "User with given username does not exist.");

            recipe.Id = await _unitOfWork.IdGenerator.GetRecipeId();
            string jsonRecipe = JsonSerializer.Serialize<Recipe>(recipe);
            await db.ListLeftPushAsync(ListConsts.RecipeListKey, jsonRecipe);
            
            User owner = JsonSerializer.Deserialize<User>(jsonUser);
            await db.ListRightPushAsync(owner.CreatedRecipesKey, recipe.Id.ToString());
            _unitOfWork.NotificationService.Notify(recipe.UserRef, "Recipe added", $"{recipe.UserRef} added a new recipe");
        }

        public async Task<Recipe> UpdateRecipe(Recipe recipe)
        {
            if (recipe.Id <= 0)
                throw new HttpResponseException(400, "Id out of bounds");

            IDatabase db = _redis.GetDatabase();
            List<Recipe> recipes = (await db.ListRangeAsync(ListConsts.RecipeListKey))
                .Select(rv => JsonSerializer.Deserialize<Recipe>(rv.ToString()))
                .ToList();

            Recipe oldRecipe = null;
            int index = 0;

            if (!IndexOfRecipeById(recipe.Id, out oldRecipe, out index))
                throw new HttpResponseException(404, "Recipe does not exist");

            oldRecipe.Title = (string.IsNullOrEmpty(recipe.Title)) ? oldRecipe.Title : recipe.Title;
            oldRecipe.Description = (string.IsNullOrEmpty(recipe.Description)) ? oldRecipe.Description : recipe.Description;
            oldRecipe.Ingredients = (recipe.Ingredients.Count > 0) ? oldRecipe.Ingredients : recipe.Ingredients;

            await db.ListSetByIndexAsync(ListConsts.RecipeListKey, index, JsonSerializer.Serialize<Recipe>(oldRecipe));

            return oldRecipe;
        }

        public async Task DeleteRecipe(int id)
        {
            if (id <= 0)
                throw new HttpResponseException(400, "Id out of bounds");
            IDatabase db = _redis.GetDatabase();

            Recipe recipe = null;
            int index = 0;

            if (!IndexOfRecipeById(id, out recipe, out index))
                throw new HttpResponseException(404, "Recipe does not exist");

            await _unitOfWork.CommentRepository.DeleteAllCommentsForKey(recipe.CommentListKey);
            recipe.IsDeleted = 1;
            await db.ListSetByIndexAsync(ListConsts.RecipeListKey, index, JsonSerializer.Serialize<Recipe>(recipe));
        }

        public async Task DeleteAllCreatedRecipesOfUser(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new HttpResponseException(400, "Key is not valid");
            IDatabase db = _redis.GetDatabase();
            
            List<int> recipeIds = (await db.ListRangeAsync(key)).Select(rv => int.Parse(rv.ToString())).ToList();
            List<Recipe> recipes = (await db.ListRangeAsync(ListConsts.RecipeListKey))
                .Select(rv => JsonSerializer.Deserialize<Recipe>(rv.ToString()))
                .ToList();
            List<RecipeWithIndex> recipesWithIndices = new List<RecipeWithIndex>();

            for(int i=0; i<recipes.Count; i++)
            {
                if(recipeIds.Contains(recipes[i].Id))
                    recipesWithIndices.Add(new RecipeWithIndex(i, recipes[i]));
            }

            if (recipesWithIndices.Count == 0)
                return;

            foreach(RecipeWithIndex el in recipesWithIndices)
            {
                await _unitOfWork.CommentRepository.DeleteAllCommentsForKey(el.Recipe.CommentListKey);
                el.Recipe.IsDeleted = 1;

                await db.ListSetByIndexAsync(ListConsts.RecipeListKey, el.Index, JsonSerializer.Serialize<Recipe>(el.Recipe));
            }
        }

        private bool IndexOfRecipeById(int id, out Recipe recipe, out int index)
        {
            IDatabase db = _redis.GetDatabase();
            List<Recipe> recipes = db.ListRange(ListConsts.RecipeListKey)
                .Select(rv => JsonSerializer.Deserialize<Recipe>(rv.ToString()))
                .ToList();

            recipe = null;
            index = 0;
            for (int i = 0; i < recipes.Count; i++)
            {
                if (recipes[i].Id == id)
                {
                    index = i;
                    recipe = recipes[i];
                    break;
                }
            }
            if (recipe == null)
                return false;
            return true;
        }
    }
}