namespace PrayerTimes.API.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; private set; }  // Changed property name from 'Success' to 'IsSuccess'
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }

        private ApiResponse(bool isSuccess, T? data, string? errorMessage)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
        }

        public static ApiResponse<T> Success(T data) => new ApiResponse<T>(true, data, null);
        public static ApiResponse<T> Fail(string errorMessage) => new ApiResponse<T>(false, default, errorMessage);
    }
}
