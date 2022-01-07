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

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            try
            {
                return await _unitOfWork.UserRepository.GetAllUsers();
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
    }
}
