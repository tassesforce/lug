using System;
using System.Collections.Generic;
using System.Linq;
using lug.Json.Serializer;
using RestSharp;

namespace lug.Helper.Http
{
    public class HttpHelper
    {
        private static List<Method> authorizedMethods = new List<Method>{Method.GET, Method.POST, Method.PUT, Method.DELETE};
        public static RestRequest CreateBaseRequest(HttpCriterias criterias)
        {
            ValidateCriterias(criterias);
                    
            var request = new RestRequest(criterias.Uri, criterias.Method) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new RestSharpJsonNetSerializer();
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Authorization", "Bearer " + criterias.AccessToken);
            request.AddHeader("Correlation-Id-Header", (string.IsNullOrEmpty(criterias.CorrelationId)) ? Guid.NewGuid().ToString() : criterias.CorrelationId);

            return request;
        }

        private static void ValidateCriterias(HttpCriterias criterias)
        {
            if(string.IsNullOrEmpty(criterias.Uri))
            {
                throw new ArgumentException("L'attribut {attribute} est invalide", "HttpCriterias.Uri");
            }
            if (!authorizedMethods.Any(m => m.Equals(criterias.Method)))
            {
                throw new ArgumentException("L'attribut {attribute} est invalide", "HttpCriterias.Method");
            }
        }

    }
}