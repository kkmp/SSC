namespace SSC.Data
{
    public class DbResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
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
