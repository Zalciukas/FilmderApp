namespace Filmder.DTOs
{
    public class ApiResponse<T> where T : class, new()
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ApiResponse(bool success, string message, T? data = null) 
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ApiResponse<TData> Create<TData>(bool success, string message)
            where TData : class, new()
        {
            return new ApiResponse<TData>(success, message, new TData());
        }
    }
}