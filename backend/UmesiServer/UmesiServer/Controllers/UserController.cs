using Microsoft.AspNetCore.Mvc;
using UmesiServer.Data;
using UmesiServer.Exceptions;
using UmesiServer.Models;

namespace UmesiServer.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private ILogger<UserController> _logger;
        private UnitOfWork _unitOfWork;

        public UserController(ILogger<UserController> log, UnitOfWork unitOfWork)
        {
            _logger = log;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetUser/{username}")]
        public async Task<ActionResult<User>> GetUser([FromRoute] string username)
        {
            try
            {
                return await _unitOfWork.UserRepository.GetUser(username);
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }

        [HttpPost("AddUser")]
        public async Task<ActionResult> PostUser([FromBody]User user)
        {
            try
            {
                await _unitOfWork.UserRepository.AddUser(user);
                return Ok("User added");
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }

        [HttpGet("AddRecipeToFavorites/{username}/{recipeId}")]
        public async Task<ActionResult> GetAddToFavorites(string username, int recipeId)
        {
            try
            {
                await _unitOfWork.UserRepository.AddRecipeToFavorites(username, recipeId);
                return Ok("Added to favorites");
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }

        [HttpGet("RemoveRecipeToFavorites/{username}/{recipeId}")]
        public async Task<ActionResult> GetRemoveToFavorites(string username, int recipeId)
        {
            try
            {
                await _unitOfWork.UserRepository.RemoveRecipeFromFavorites(username, recipeId);
                return Ok("Removed from favorites");
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }

        [HttpGet("FollowUser/{currentUser}/{userToFollow}")]
        public async Task<ActionResult> GetFollowUser(string currentUser, string userToFollow)
        {
            try
            {
                await _unitOfWork.UserRepository.FollowUser(currentUser, userToFollow);
                return Ok("User followed");
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }

        [HttpGet("UnfollowUser/{currentUser}/{userToFollow}")]
        public async Task<ActionResult> GetUnfollowUser(string currentUser, string userToFollow)
        {
            try
            {
                await _unitOfWork.UserRepository.UnfollowUser(currentUser, userToFollow);
                return Ok("User unfollowed");
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<User>> PutUser([FromBody]User user)
        {
            try
            {
                return await _unitOfWork.UserRepository.UpdateUser(user);
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }

        [HttpDelete("DeleteUser/{username}")]
        public async Task<ActionResult> DeleteUser([FromRoute]string username)
        {
            try
            {
                await _unitOfWork.UserRepository.DeleteUser(username);
                return Ok("User deleted");
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }
    }
}
