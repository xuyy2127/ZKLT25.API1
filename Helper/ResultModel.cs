namespace ZKLT25.API.Helper
{
    public class ResultModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
        public string? Warning { get; set; }

        // 静态工厂方法
        public static ResultModel<T> Ok(T data) => new() { Success = true, Data = data };
        public static ResultModel<T> Error(string msg) => new() { Success = false, Message = msg };
    }
}
