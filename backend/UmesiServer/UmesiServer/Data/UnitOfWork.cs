using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using UmesiServer.Data.UserRepository;
using UmesiServer.Data.RecipeRepository;
using UmesiServer.Data.CommentRepository;

namespace UmesiServer.Data
{
    public class UnitOfWork
    {
        private ConnectionMultiplexer _redis;
        private ILogger<UnitOfWork> _logger;

        public UnitOfWork(ILogger<UnitOfWork> logger)
        {
            _redis = ConnectionMultiplexer.Connect("localhost:6379");
            _logger = logger;
            _redis.ErrorMessage += _redis_ErrorMessage;
        }

        private void _redis_ErrorMessage(object? sender, RedisErrorEventArgs e)
        {
            _logger.LogError(e.Message);
        }

        private IUserRepository _userRepo;
        private IRecipeRepository _recipeRepo;
        private ICommentRepository _commentRepo;

        public IUserRepository UserRepository 
        { 
            get => (_userRepo != null) ? _userRepo : (_userRepo = new UserRepository.UserRepository(_redis)); 
        }

        public IRecipeRepository RecipeRepository
        {
            get => (_recipeRepo != null) ? _recipeRepo : (_recipeRepo = new RecipeRepository.RecipeRepository(_redis));
        }
        
        public ICommentRepository CommentRepository
        {
            get => (_commentRepo != null) ? _commentRepo : (_commentRepo = new CommentRepository.CommentRepository(_redis));
        }
    }
}