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

    internal abstract record BodyPartDto(BodyPartBasic Part)
    {
        public abstract bool IsText { get; }

        public static async Task<BodyPartDto> From(BodyPartBasic bodyPart, MimeEntity mimeEntity, CancellationToken cancellationToken)
            => mimeEntity switch
            {
                TextPart textPart => await TextPartDto.From(bodyPart, textPart, cancellationToken),
                MimePart mimePart => await MimePartDto.From(bodyPart, mimePart, cancellationToken),
                _ => throw new NotImplementedException($"{mimeEntity.GetType().FullName} is not implemented yet."),
            };
    }
}