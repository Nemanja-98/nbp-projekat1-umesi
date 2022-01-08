using Microsoft.AspNetCore.Mvc;
using UmesiServer.Data;
using UmesiServer.Exceptions;
using UmesiServer.Models;

namespace UmesiServer.Controllers
{
    [Route("api/[controller]")]
    public class RecipeController : Controller
    {
        private ILogger<RecipeController> _logger;
        private UnitOfWork _unitOfWork;

        public RecipeController(ILogger<RecipeController> logger, UnitOfWork unit)
        {
            _logger = logger;
            _unitOfWork = unit;
        }

        [HttpGet("GetRecipe/{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe([FromRoute]int id)
        {
            try
            {
                return await _unitOfWork.RecipeRepository.GetRecipe(id);
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }

        [HttpGet("GetAllRecipes")]
        public async Task<ActionResult<List<Recipe>>> GetRecipes()
        {
            try
            {
                return await _unitOfWork.RecipeRepository.GetAllRecipes();
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }

        [HttpPost("AddRecipe")]
        public async Task<ActionResult> PostRecipe([FromBody]Recipe recipe)
        {
            try
            {
                await _unitOfWork.RecipeRepository.AddRecipe(recipe);
                return Ok("Recipe added");
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }

        [HttpPut("UpdateRecipe")]
        public async Task<ActionResult<Recipe>> PutRecipe([FromBody]Recipe recipe)
        {
            try
            {
                return await _unitOfWork.RecipeRepository.UpdateRecipe(recipe);
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }

        [HttpDelete("DeleteRecipe/{id}")]
        public async Task<ActionResult> DeleteRecipe([FromRoute]int id)
        {
            try
            {
                await _unitOfWork.RecipeRepository.DeleteRecipe(id);
                return Ok("Recipe deleted");
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }
    }
}
