
namespace ApiErrorHandlingTemplete.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public ApiResponse(int statusCode, string? msg = null)
        {
            StatusCode = statusCode;
            Message = msg ?? GetDefaultErrorMSG(statusCode);
        }

        private string? GetDefaultErrorMSG(int statusCode)
        {
             switch (statusCode)
            {
                case 400:
                    return "you made a bad request";
                case 401:
                    return "You are unothorized";
                case 404:
                    return "the resource not found";
                case 500:
                    return "Internal Server Error";
                default:
                    return "";
            }
        }
    }
}
