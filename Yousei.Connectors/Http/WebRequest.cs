using System;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Yousei.Connectors.Http
{
    internal record WebRequest
    {
        public Uri Url { get; init; }

        public string BodyText { get; init; }

        public IReadOnlyDictionary<string, string> Headers { get; init; }

        public IReadOnlyList<string> AcceptType { get; init; }

        public string HttpMethod { get; init; }

        public string LocalEndPoint { get; init; }

        public Version ProtocolVersion { get; init; }

        public IReadOnlyDictionary<string, string> QueryString { get; init; }

        public string RawUrl { get; init; }

        public string RemoteEndPoint { get; init; }

        public Guid RequestTraceIdentifier { get; init; }

        public Uri UrlReferrer { get; init; }

        public static async Task<WebRequest> FromRequest(HttpListenerRequest request)
        {
            using var memStream = new MemoryStream();
            await request.InputStream.CopyToAsync(memStream);
            var bodyData = memStream.ToArray();
            var bodyText = request.ContentEncoding.GetString(bodyData);

            return new WebRequest
            {
                Url = request.Url,
                BodyText = bodyText,
                Headers = ToDictionary(request.Headers),
                AcceptType = request.AcceptTypes,
                HttpMethod = request.HttpMethod,
                LocalEndPoint = request.LocalEndPoint.ToString(),
                ProtocolVersion = request.ProtocolVersion,
                QueryString = ToDictionary(request.QueryString),
                RawUrl = request.RawUrl,
                RemoteEndPoint = request.RemoteEndPoint.ToString(),
                RequestTraceIdentifier = request.RequestTraceIdentifier,
                UrlReferrer = request.UrlReferrer,
            };

            Dictionary<string, string> ToDictionary(NameValueCollection nameValueCollection)
                => nameValueCollection
                    .Cast<string>()
                    .ToDictionary(o => o, o => nameValueCollection[o]);
        }
    }
}