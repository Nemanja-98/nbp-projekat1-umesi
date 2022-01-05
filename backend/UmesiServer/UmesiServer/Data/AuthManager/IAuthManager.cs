using UmesiServer.DTOs.Records;

namespace UmesiServer.Data.AuthManager
{
    public interface IAuthManager
    {
        Task<string> Login(LoginDto creds);

        Task<bool> IsLogedIn(string token);
    }
}
