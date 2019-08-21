using Newtonsoft.Json;

namespace Lug.GlobalErrorHandling.API
{
    public class ApiErrorResponse
    {
        [JsonProperty("status")]
        public int StatusCode { get; set; }
        [JsonProperty("code")]
        public string ErrorCode {get;set;}
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("trace_id")]
        public string TraceId {get;set;}
 
        #region Constructeurs
        public ApiErrorResponse()
        {

        }

        public ApiErrorResponse(int statusCode, string traceId, string errorCode, string message) {
            this.StatusCode = statusCode;
            this.TraceId = traceId;
            this.ErrorCode = errorCode;
            this.Message = message;
        }
        #endregion Constructeurs

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}