namespace SSC.Data
{

    public class DbResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static DbResult CreateSuccess(string message)
        {
            return new DbResult { Success = true, Message = message};
        }

        public static DbResult CreateFail(string message)
        {
            return new DbResult { Message = message };
        }
    }

    public class DbResult<T> : DbResult
    {
        public T Data { get; set; }

        public static DbResult<T> CreateSuccess(string message, T Data)
        {
            return new DbResult<T> { Success = true, Message = message, Data = Data };
        }

        public static DbResult<T> CreateFail(string message)
        {
            return new DbResult<T> { Message = message };
        }
    }
}
