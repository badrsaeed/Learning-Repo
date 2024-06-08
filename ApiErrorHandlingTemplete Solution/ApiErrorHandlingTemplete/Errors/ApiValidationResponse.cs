namespace ApiErrorHandlingTemplete.Errors
{
    public class ApiValidationResponse : ApiResponse
    {
        public List<string> Errors { get; set; }
        public ApiValidationResponse():base(400)
        {
            this.Errors = new List<string>();
        }
    }
}
