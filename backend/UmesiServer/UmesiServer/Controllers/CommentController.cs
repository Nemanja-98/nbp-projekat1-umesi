using Microsoft.AspNetCore.Mvc;
using UmesiServer.Data;
using UmesiServer.Exceptions;
using UmesiServer.Models;

namespace UmesiServer.Controllers
{
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private ILogger<CommentController> _logger;
        private UnitOfWork _unitOfWork;

        public CommentController(ILogger<CommentController> logger, UnitOfWork unit)
        {
            _logger = logger;
            _unitOfWork = unit;
        }

        [HttpGet("GetCommentsForRecipe/{recipeId}")]
        public async Task<ActionResult<List<Comment>>> GetCommentsForRecipe(int recipeId)
        {
            try
            {
                return await _unitOfWork.CommentRepository.GetCommentsForRecipe(recipeId);
            }
            catch(HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }

        [HttpPost("AddComment/{recipeId}")]
        public async Task<ActionResult> AddCommentToRecipe(int recipeId, Comment comment)
        {
            try
            {
                await _unitOfWork.CommentRepository.AddComment(recipeId, comment);
                return Ok("Comment added");
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }
    }
}
