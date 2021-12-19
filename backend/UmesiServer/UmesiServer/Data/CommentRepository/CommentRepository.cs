using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace UmesiServer.Data.CommentRepository
{
    public class CommentRepository : ICommentRepository
    {
        private ConnectionMultiplexer _redis;
        public CommentRepository(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }
    }
}