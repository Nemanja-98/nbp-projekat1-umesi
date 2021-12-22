namespace UmesiServer.Data.IdGeneratorRepository
{
    public interface IIdGeneratorRepository
    {
        public Task<int> GetRecipeId();
    }
}
