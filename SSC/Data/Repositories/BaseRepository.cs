namespace SSC.Data.Repositories
{
    public class BaseRepository
    {
        public DbResult Validate(Dictionary<Func<bool>, string> conditions)
        {
            foreach (var condition in conditions)
            {
                if (condition.Key())
                {
                    return DbResult.CreateFail(condition.Value);
                }
            }
            return null;
        }
    }

    public class BaseRepository<T> where T : class
    {
        public DbResult<T> Validate(Dictionary<Func<bool>, string> conditions)
        {
            foreach (var condition in conditions)
            {
                if (condition.Key())
                {
                    return DbResult<T>.CreateFail(condition.Value);
                }
            }
            return null;
        }
    }
}
