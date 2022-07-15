using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent.ApiRunnerEngine
{
    public class RestRunner : IRestRunner
    {
        public async Task<T?> Execute<T>(RestRequestProperties requestProperties) where T : class
        {
            var responseJson = await Execute(requestProperties).ConfigureAwait(false);
            try
            {
                return !string.IsNullOrWhiteSpace(responseJson) ?
                    JsonSerializer.Deserialize<T>(responseJson) :
                    null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string?> Execute(RestRequestProperties requestProperties)
        {
            var client = new RestClient(requestProperties.BaseUrl!)
            {
                Authenticator = requestProperties.Authenticator
            };

            var request = new RestRequest(requestProperties.Resource, requestProperties.Method);

            if (requestProperties.Body != null)
                request.AddJsonBody(requestProperties.Body);

            request = GetHeaders(request, requestProperties.Headers);
            request = GetQueryParameters(request, requestProperties.QueryParameters);
            request = GetParameters(request, requestProperties.Parameters);
            request = GetFiles(request, requestProperties.Files);
            request = GetUrlSegments(request, requestProperties.UrlSegments);

            try
            {
                var response = await client.ExecuteAsync(request, new CancellationTokenSource().Token).ConfigureAwait(false);
                return response.Content;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static RestRequest GetHeaders(RestRequest request, Dictionary<string, string>? headers = null)
        {
            if (headers == null || !headers.Any())
                return request;

            foreach (var header in headers)
            {
                request.AddHeader(header.Key, header.Value);
            }

            return request;
        }

        private static RestRequest GetQueryParameters(RestRequest request, Dictionary<string, string>? queryParameters = null)
        {
            if (queryParameters == null || !queryParameters.Any())
                return request;

            foreach (var queryParameter in queryParameters)
            {
                request.AddQueryParameter(queryParameter.Key, queryParameter.Value);
            }

            return request;
        }

        private static RestRequest GetParameters(RestRequest request, List<RequestParameter>? parameters)
        {
            if (parameters == null || !parameters.Any())
                return request;

            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType == null)
                    request.AddParameter(parameter.Name, parameter.Value);
                else
                    request.AddParameter(parameter.Name, parameter.Value, (ParameterType)parameter.ParameterType);
            }

            return request;
        }

        private static RestRequest GetFiles(RestRequest request, List<RequestFile>? files)
        {
            if (files == null || !files.Any())
                return request;

            foreach (var file in files)
            {
                if (string.IsNullOrWhiteSpace(file.FileName) && !string.IsNullOrWhiteSpace(file.Path))
                    request.AddFile(file.Name, file.Path);
                else if (file.Bytes != null)
                    request.AddFile(file.Name, file.Bytes, file.FileName ?? "");
            }

            return request;
        }

        private static RestRequest GetUrlSegments(RestRequest request, List<UrlSegment>? urlSegments)
        {
            if (urlSegments == null || !urlSegments.Any())
                return request;

            foreach (var urlSegment in urlSegments)
            {
                request.AddUrlSegment(urlSegment.Key, urlSegment.Value, !urlSegment.SkipEncoding);
            }

            return request;
        }
    }
}
