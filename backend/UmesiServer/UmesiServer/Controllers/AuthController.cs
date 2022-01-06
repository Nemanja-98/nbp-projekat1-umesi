using Microsoft.AspNetCore.Mvc;
using UmesiServer.Data;
using UmesiServer.DTOs.Records;
using UmesiServer.Exceptions;

namespace UmesiServer.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private ILogger<AuthController> _logger;
        private UnitOfWork _unitOfWork;

        public AuthController(ILogger<AuthController> logger, UnitOfWork unit)
        {
            _logger = logger;
            _unitOfWork = unit;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody]LoginDto creds)
        {
            try
            {
                return new JsonResult(new TokenDto(await _unitOfWork.AuthManager.Login(creds)));
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(ex.Status, ex.Message);
            }
        }
    }
}
