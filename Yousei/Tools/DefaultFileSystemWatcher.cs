using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Yousei.Tools
{
    class DefaultFileSystemWatcher : IFileSystemWatcher
    {
        private readonly FileSystemWatcher fileSystemWatcher;

        public DefaultFileSystemWatcher(string path, string filter)
        {
            fileSystemWatcher = new FileSystemWatcher(path, filter);
        }

        public bool EnableRaisingEvents
        {
            get => fileSystemWatcher.EnableRaisingEvents;
            set => fileSystemWatcher.EnableRaisingEvents = value;
        }

        public event FileSystemEventHandler Changed
        {
            add => fileSystemWatcher.Changed += value;
            remove => fileSystemWatcher.Changed -= value;
        }
    }
}
