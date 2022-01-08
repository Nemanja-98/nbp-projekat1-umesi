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
        Task AddUser(User user);
        Task<User> UpdateUser(User user);
        Task DeleteUser(string username);
        Task AddRecipeToFavorites(string username, int recipeId);
        Task FollowUser(string currentUser, string userToFollow);
    }
}