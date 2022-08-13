namespace SSC.Data.Repositories
{
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
