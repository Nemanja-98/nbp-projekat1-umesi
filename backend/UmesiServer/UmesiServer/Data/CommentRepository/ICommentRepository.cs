using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UmesiServer.Models;

namespace UmesiServer.Data.CommentRepository
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetCommentsForRecipe(int recipeId);
        Task AddComment(int recipeId, Comment comment);
        Task<Comment> UpdateComment(int recipeId, int index, Comment comment);
        Task DeleteComment(int recipeId, int index);
    }
}