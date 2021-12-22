using StackExchange.Redis;
using UmesiServer.Constants;

namespace UmesiServer.Data.IdGeneratorRepository
{
    public class IdGeneratorRepository : IIdGeneratorRepository
    {
        private ConnectionMultiplexer _redis;

        public IdGeneratorRepository(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<int> GetRecipeId()
        {
            IDatabase db = _redis.GetDatabase();
            int id = int.Parse(await db.StringGetAsync(IdGenConsts.RecipeIdGenKey));
            await db.StringSetAsync(IdGenConsts.RecipeIdGenKey, (id + 1).ToString());
            return id;
        }
    }
}
