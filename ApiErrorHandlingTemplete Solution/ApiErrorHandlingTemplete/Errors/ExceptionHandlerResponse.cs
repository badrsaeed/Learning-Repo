namespace ApiErrorHandlingTemplete.Errors
{
    public class ExceptionHandlerResponse : ApiResponse
    {
        public string? MoreInfo { get; set; }
        public ExceptionHandlerResponse(int statusCode, string? message = null, string ?moreInfo = null):base(statusCode, message)
        {
            this.MoreInfo = moreInfo;
        }
    }
}
