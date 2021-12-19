using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UmesiServer.Models
{
    public class Recipe
    {

        public string UserRef { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public string ImagePath { get; set; }
        
        public string CommentListKey 
        { 
            get => (Title == string.Empty || UserRef == string.Empty) ? string.Empty : $"{UserRef}:{Title}";
        }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}