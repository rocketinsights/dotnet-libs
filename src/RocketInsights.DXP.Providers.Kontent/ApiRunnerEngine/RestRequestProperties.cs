using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;

namespace RocketInsights.DXP.Providers.Kontent.ApiRunnerEngine
{
    public class RestRequestProperties
    {
        public Method Method { get; set; }
        public Uri? BaseUrl { get; set; }
        public IAuthenticator? Authenticator { get; set; }
        public string? Resource { get; set; }
        public object? Body { get; set; }
        public Dictionary<string, string> QueryParameters { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public List<RequestParameter> Parameters { get; set; } = new List<RequestParameter>();
        public List<RequestFile> Files { get; set; } = new List<RequestFile>();
        public List<UrlSegment> UrlSegments { get; set; } = new List<UrlSegment>();
    }

    public class RequestParameter
    {
        internal string Name { get; }
        internal string Value { get; }
        internal ParameterType? ParameterType { get; }

        public RequestParameter(string name, string value, ParameterType? parameterType = null)
        {
            Name = name;
            Value = value;
            ParameterType = parameterType;
        }
    }

    public class RequestFile
    {
        internal string Name { get; }
        internal byte[]? Bytes { get; }
        internal string? FileName { get; }
        internal string? Path { get; }

        public RequestFile(string name, byte[]? bytes, string? fileName, string? path)
        {
            Name = name;
            Bytes = bytes;
            FileName = fileName;
            Path = path;
        }
    }

    public class UrlSegment
    {
        internal string Key { get; set; }
        internal string Value { get; set; }
        internal bool SkipEncoding { get; set; }

        public UrlSegment(string key, string value, bool skipEncoding = false)
        {
            Key = key;
            Value = value;
            SkipEncoding = skipEncoding;
        }
    }
}
