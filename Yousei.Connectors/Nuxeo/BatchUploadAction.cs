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
    internal record BatchUploadArguments : BatchArguments
    {
        public IParameter<string> Filename { get; init; } = DefaultParameter<string>.Instance;

        public IParameter<byte[]> Raw { get; init; } = DefaultParameter<byte[]>.Instance;

        public IParameter<string> Text { get; init; } = DefaultParameter<string>.Instance;
    }

    internal class BatchUploadAction : BatchAction<BatchUploadArguments>
    {
        public override string Name { get; } = "batch_upload";

        protected override async Task Act(IFlowContext context, BatchUploadArguments arguments, Batch batch, int? fileIndex)
        {
            if (!fileIndex.HasValue)
                throw new ArgumentNullException(nameof(fileIndex));

            using var tempFile = Temp.File(out var fileInfo);
            var filename = await arguments.Filename.Resolve(context);
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException($"\"{filename}\" is not a valid filename.");

            var content = await arguments.Raw.Resolve(context);
            if (content is null)
            {
                var contentText = await arguments.Text.Resolve(context);
                if (contentText is null)
                    throw new ArgumentException($"No payload provided. Either Raw or Text must be set.");

                content = new UTF8Encoding(false).GetBytes(contentText);
            }

            using (var stream = fileInfo.OpenWrite())
            {
                await stream.WriteAsync(content);
                await stream.FlushAsync();
            }
            fileInfo.Refresh();

            var blob = new Blob(filename, fileInfo);
            var job = new UploadJob(blob).SetFileId(fileIndex.Value);
            batch = await batch.Upload(job);

            await context.SetData(batch);
        }
    }
}