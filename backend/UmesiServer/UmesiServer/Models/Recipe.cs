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

        public List<string> Ingredients { get; set; } = new List<string>();
        
        public int IsDeleted { get; set; }

        [JsonIgnore]
        public string CommentListKey 
        { 
            get => (Id == 0 || UserRef == string.Empty) ? string.Empty : $"{UserRef}:Recipe:{Id}";
        }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}