using UmesiServer.Data;
using UmesiServer.Data.AuthManager;

namespace UmesiServer.Middleware
{
    public class AuthMiddleware
    {
        private IAuthManager _authManager;
        private RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next, UnitOfWork unit)
        {
            _authManager = unit.AuthManager;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Contains("api/Auth") || context.Request.Path.Value.Contains("api/User/AddUser"))
            {
                await _next?.Invoke(context);
                return;
            }
            if (!context.Request.Headers.ContainsKey("Authorization") || !(await _authManager.IsLogedIn(context.Request.Headers["Authorization"])))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("User is not authorized");
                return;
            }
            await _next?.Invoke(context);
        }
    }
}
