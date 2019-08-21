namespace lug.Helper.Http
{
    public class HttpCriterias
    {
        public RestSharp.Method Method {get; set;}
        public string Uri {get; set;}
        public string AccessToken {get; set;}
        public string CorrelationId {get; set;}
    }
}