using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Yousei.Tools
{
    interface IFileSystemWatcher
    {
        event FileSystemEventHandler Changed;

        bool EnableRaisingEvents { get; set; }
    }
}
