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
    public class ImapConnector : SimpleConnector<ImapConfiguration>
    {
        public ImapConnector() : base("imap")
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        protected override IConnection? CreateConnection(ImapConfiguration configuration)
            => new ImapConnection(configuration);
    }
}