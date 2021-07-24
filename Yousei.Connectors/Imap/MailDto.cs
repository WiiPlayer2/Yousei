using Humanizer;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Imap
{

    internal record MailDto(UniqueId UniqueId, Envelope Envelope, IReadOnlyList<BodyPartDto> Parts)
    {
        public static async Task<MailDto> FromSummary(IMessageSummary summary, IMailFolder folder, CancellationToken cancellationToken)
        {
            var bodyParts = await summary.BodyParts
                .Select(async o => await BodyPartDto.From(o, await folder.GetBodyPartAsync(summary.UniqueId, o, cancellationToken), cancellationToken))
                .ToList();
            return new MailDto(summary.UniqueId, summary.Envelope, bodyParts);
        }
    }
}