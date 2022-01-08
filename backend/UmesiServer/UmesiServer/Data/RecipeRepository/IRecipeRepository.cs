using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UmesiServer.Models;

namespace UmesiServer.Data.RecipeRepository
{
    public interface IRecipeRepository
    {
        Task<Recipe> GetRecipe(int id);
        Task<List<Recipe>> GetAllRecipes();
        Task AddRecipe(Recipe recipe);
        Task<Recipe> UpdateRecipe(Recipe recipe);
        Task DeleteRecipe(int id);
        Task DeleteAllCreatedRecipesOfUser(string key);
    }
}