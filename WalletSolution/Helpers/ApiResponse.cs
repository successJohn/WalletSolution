namespace WalletSolution.Helpers
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }

        public ApiResponse()
        {

        }

        public ApiResponse(bool succeeded, string message, T data)
        {
            Succeeded = succeeded;
            Message = message;
            Data = data;
        }

        public ApiResponse(bool succeeded, string message)
        {
            Succeeded = succeeded;
            Message = message;
        }

        public static ApiResponse<T> Fail(string errorMessage)
        {
            return new ApiResponse<T> { Succeeded = false, Message = errorMessage };
        }
        public static ApiResponse<T> Success(T data)
        {
            return new ApiResponse<T> { Succeeded = true, Data = data };
        }
    }


    public class ApiResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }

        public ApiResponse()
        {
        }

        public ApiResponse(bool status, string message)
        {
            Status = status;
            Message = message;
        }
        public ApiResponse(bool status)
        {
            Status = status;
        }
    }
}