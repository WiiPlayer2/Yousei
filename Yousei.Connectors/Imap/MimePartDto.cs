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

    internal record MimePartDto(BodyPartBasic Part, byte[] Raw) : BodyPartDto(Part)
    {
        public override bool IsText { get; } = false;

        public static async Task<MimePartDto> From(BodyPartBasic bodyPart, MimePart mimePart, CancellationToken cancellationToken)
        {
            using var memStream = new MemoryStream();
            await mimePart.Content.WriteToAsync(memStream, cancellationToken);
            return new(bodyPart, memStream.ToArray());
        }
    }
}