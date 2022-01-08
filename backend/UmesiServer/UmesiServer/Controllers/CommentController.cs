using Microsoft.AspNetCore.Mvc;
using UmesiServer.Data;
using UmesiServer.DTOs.Records;
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
        public async Task<ActionResult<List<Comment>>> GetCommentsForRecipe([FromRoute]int recipeId)
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
        public async Task<ActionResult> AddCommentToRecipe([FromRoute]int recipeId, [FromBody]Comment comment)
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

        [HttpPut("UpdateComment")]
        public async Task<ActionResult<Comment>> PutComment([FromBody]UpdateCommentDto dto)
        {
            try
            {
                return await _unitOfWork.CommentRepository.UpdateComment(dto.RecipeId, dto.Index, dto.Comment);
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }

        [HttpDelete("DeleteComment")]
        public async Task<ActionResult> DeleteComment([FromBody]DeleteCommentDto dto)
        {
            try
            {
                await _unitOfWork.CommentRepository.DeleteComment(dto.RecipeId, dto.Index);
                return Ok("Comment deleted");
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }
    }
}
