using DebounceThrottle;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Yousei.Tools
{
    class DelayFileSystemWatcher : IFileSystemWatcher
    {
        public event FileSystemEventHandler Changed;

        private readonly IFileSystemWatcher fileSystemWatcher;

        private readonly TimeSpan delay;

        private readonly ConcurrentDictionary<string, DebounceDispatcher> debounceDispatchers = new ConcurrentDictionary<string, DebounceDispatcher>();

        public DelayFileSystemWatcher(IFileSystemWatcher fileSystemWatcher, TimeSpan delay)
        {
            this.delay = delay;
            this.fileSystemWatcher = fileSystemWatcher;
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
        }

        public bool EnableRaisingEvents
        {
            get => fileSystemWatcher.EnableRaisingEvents;
            set => fileSystemWatcher.EnableRaisingEvents = value;
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            var dispatcher = debounceDispatchers.GetOrAdd(e.FullPath, _ => new DebounceDispatcher((int)delay.TotalMilliseconds));
            dispatcher.Debounce(() =>
            {
                debounceDispatchers.TryRemove(e.FullPath, out var _);
                if(EnableRaisingEvents)
                    Changed?.Invoke(this, e);
            });
        }
    }
}
