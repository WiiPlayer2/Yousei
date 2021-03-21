using System;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Yousei.Connectors.Http
{
    internal record HttpResponse
    {
        public string BodyText { get; init; }
        public string CharacterSet { get; init; }
        public string ContentEncoding { get; init; }
        public long ContentLength { get; init; }
        public string ContentType { get; init; }
        public CookieCollection Cookies { get; init; }
        public Dictionary<string, string> Headers { get; init; }
        public string Method { get; init; }
        public Version ProtocolVersion { get; init; }
        public Uri ResponseUri { get; init; }
        public string Server { get; init; }
        public HttpStatusCode StatusCode { get; init; }
        public string StatusDescription { get; init; }

        public static async Task<HttpResponse> FromResponse(HttpWebResponse response)
        {
            using var memStream = new MemoryStream();
            await response.GetResponseStream().CopyToAsync(memStream);
            var bodyData = memStream.ToArray();
            var bodyText = Encoding.UTF8.GetString(bodyData);

            return new HttpResponse
            {
                BodyText = bodyText,
                CharacterSet = response.CharacterSet,
                ContentEncoding = response.ContentEncoding,
                ContentLength = response.ContentLength,
                ContentType = response.ContentType,
                Cookies = response.Cookies,
                Headers = ToDictionary(response.Headers),
                Method = response.Method,
                ProtocolVersion = response.ProtocolVersion,
                ResponseUri = response.ResponseUri,
                Server = response.Server,
                StatusCode = response.StatusCode,
                StatusDescription = response.StatusDescription,
            };

            Dictionary<string, string> ToDictionary(NameValueCollection nameValueCollection)
                => nameValueCollection
                    .Cast<string>()
                    .ToDictionary(o => o, o => nameValueCollection[o]);
        }
    }
}