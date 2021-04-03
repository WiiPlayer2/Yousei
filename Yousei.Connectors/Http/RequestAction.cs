using Yousei.Core;
using System.Net;
using System.Threading.Tasks;
using Yousei.Shared;
using System.IO;
using System.Text;
using System;

namespace Yousei.Connectors.Http
{
    internal class RequestAction : FlowAction<RequestArguments>
    {
        private static readonly Encoding defaultEncoding = new UTF8Encoding(false);

        protected override async Task Act(IFlowContext context, RequestArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var url = await arguments.Url.Resolve<string>(context);
            var method = await arguments.Method.Resolve<string>(context);
            var body = await arguments.Body.Resolve<string>(context);

            if (url is null)
                throw new ArgumentNullException(nameof(arguments.Url));
            if (method is null)
                throw new ArgumentNullException(nameof(arguments.Method));

            var request = WebRequest.CreateHttp(url);
            request.Method = method;
            if (!string.IsNullOrEmpty(body))
            {
                using var requestStream = await request.GetRequestStreamAsync();
                using var requestWriter = new StreamWriter(requestStream, defaultEncoding);
                await requestWriter.WriteAsync(body);
                await requestWriter.FlushAsync();
            }

            var response = await request.GetResponseAsync();
            var httpResponse = await HttpResponse.FromResponse((HttpWebResponse)response);

            await context.SetData(httpResponse);
            response.Close();
        }
    }
}