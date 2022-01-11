using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using UmesiServer.Data.UserRepository;
using UmesiServer.Data.RecipeRepository;
using UmesiServer.Data.CommentRepository;
using UmesiServer.Constants;
using UmesiServer.Data.IdGeneratorRepository;
using UmesiServer.Data.AuthManager;
using Microsoft.AspNetCore.SignalR;
using UmesiServer.Hubs.NotificationHub;
using UmesiServer.Data.NotificationService;

namespace UmesiServer.Data
{
    public class UnitOfWork
    {
        private ConnectionMultiplexer _redis;
        private ILogger<UnitOfWork> _logger;
        private IHubContext<NotificationHub> _notificatioContext;

        public UnitOfWork(ILogger<UnitOfWork> logger, IHubContext<NotificationHub> context)
        {
            _redis = ConnectionMultiplexer.Connect("localhost:6379");
            _logger = logger;
            _notificatioContext = context;
            _redis.ErrorMessage += _redis_ErrorMessage;
            if (string.IsNullOrEmpty(_redis.GetDatabase().StringGet(IdGenConsts.RecipeIdGenKey)))
                _redis.GetDatabase().StringSet(IdGenConsts.RecipeIdGenKey, "1");
        }

        private void _redis_ErrorMessage(object? sender, RedisErrorEventArgs e)
        {
            _logger.LogError(e.Message);
        }

        private IUserRepository _userRepo;
        private IRecipeRepository _recipeRepo;
        private ICommentRepository _commentRepo;
        private IIdGeneratorRepository _idGenRepo;
        private IAuthManager _authManager;
        private INotificationService _notificationService;

        public IUserRepository UserRepository 
        { 
            get => _userRepo ??= new UserRepository.UserRepository(_redis, this);
        }

        public IRecipeRepository RecipeRepository
        {
            get => _recipeRepo ??= new RecipeRepository.RecipeRepository(_redis, this);
        }
        
        public ICommentRepository CommentRepository
        {
            get => _commentRepo ??= new CommentRepository.CommentRepository(_redis, this);
        }

        public IIdGeneratorRepository IdGenerator
        {
            get => _idGenRepo ??= new IdGeneratorRepository.IdGeneratorRepository(_redis);
        }

        public IAuthManager AuthManager
        {
            get => _authManager ??= new AuthManager.AuthManager(_redis);
        }

        public INotificationService NotificationService
        {
            get => _notificationService ??= new NotificationService.NotificationService(_redis, _notificatioContext, _logger);
        }
    }
}