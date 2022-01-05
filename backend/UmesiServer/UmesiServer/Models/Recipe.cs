using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UmesiServer.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        public string UserRef { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        [JsonIgnore]
        public string ImagePath { get; set; }
        
        [JsonIgnore]
        public string CommentListKey 
        { 
            get => (Id == 0 || Title == string.Empty || UserRef == string.Empty) ? string.Empty : $"{UserRef}:({Id}):{Title}";
        }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}