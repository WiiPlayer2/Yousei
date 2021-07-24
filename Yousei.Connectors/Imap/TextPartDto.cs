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

    internal record TextPartDto(BodyPartBasic Part, string Text, byte[] Raw) : MimePartDto(Part, Raw)
    {
        public override bool IsText { get; } = true;

        public static async Task<TextPartDto> From(BodyPartBasic bodyPart, TextPart textPart, CancellationToken cancellationToken)
        {
            using var memStream = new MemoryStream();
            await textPart.Content.WriteToAsync(memStream, cancellationToken);
            return new(bodyPart, textPart.Text, memStream.ToArray());
        }
    }
}