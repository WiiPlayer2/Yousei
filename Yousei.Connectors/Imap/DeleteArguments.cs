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

    internal record DeleteArguments
    {
        public IParameter Folder { get; init; } = "INBOX".ToConstantParameter();

        public IParameter ID { get; init; } = 0u.ToConstantParameter();
    }
}