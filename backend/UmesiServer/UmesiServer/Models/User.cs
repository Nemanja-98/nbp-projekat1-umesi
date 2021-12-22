using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UmesiServer.Models
{
    public class User
    {
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string Name { get; set; }
        
        public string Surname { get; set; }
        
        [JsonIgnore]
        public string CreatedRecipesKey
        { 
            get => (Username == string.Empty) ? string.Empty : $"{Username}:CreatedRecipes";
        }
        
        public List<Recipe> CreatedRecipes { get; set; } = new List<Recipe>();

        [JsonIgnore]
        public string FavoriteRecipesKey
        { 
            get => (Username == string.Empty) ? string.Empty : $"{Username}:FavoriteRecipes";
        }

        public List<Recipe> FavoriteRecipes { get; set; } = new List<Recipe>();

        [JsonIgnore]
        public string FollowedUsersKey
        {
            get => (Username == string.Empty) ? string.Empty : $"{Username}:FollowedUsers";
        }

        public List<User> FollowedUsers { get; set; } = new List<User>();
    }
}