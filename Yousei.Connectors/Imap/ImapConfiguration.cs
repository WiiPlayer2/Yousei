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
    public record ImapConfiguration
    {
        public string? Host { get; init; }

        public int Port { get; init; }

        public string? Username { get; init; }

        public string? Password { get; init; }
    }
}