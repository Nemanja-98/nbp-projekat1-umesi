using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UmesiServer.Models;

namespace UmesiServer.Data.UserRepository
{
    public interface IUserRepository
    {
        Task<User> GetUser(string username);
        Task<List<User>> GetAllUsers();
        Task AddUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(string username);
    }
}