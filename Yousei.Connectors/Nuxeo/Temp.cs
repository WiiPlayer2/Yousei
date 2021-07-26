using Newtonsoft.Json.Linq;
using NuxeoClient;
using NuxeoClient.Adapters;
using NuxeoClient.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;
using Task = System.Threading.Tasks.Task;

namespace Yousei.Connectors.Nuxeo
{
    internal static class Temp
    {
        public static IDisposable File(out FileInfo fileInfo)
        {
            var tempFile = Path.GetTempFileName();
            fileInfo = new FileInfo(tempFile);
            return new ActionDisposable(fileInfo.Delete);
        }
    }
}