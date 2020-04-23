using LanguageExt;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Tools
{
    class PeriodicFileSystemWatcher : IFileSystemWatcher
    {
        private class FileNameComparer : IEqualityComparer<FileInfo>
        {
            public static FileNameComparer Instance { get; } = new FileNameComparer();

            private FileNameComparer() { }

            public bool Equals([AllowNull] FileInfo x, [AllowNull] FileInfo y) => string.Equals(x?.FullName, y?.FullName);

            public int GetHashCode([DisallowNull] FileInfo obj) => obj.FullName.GetHashCode();
        }

        private readonly string path;
        
        private readonly string filter;
        private readonly TimeSpan interval;
        private CancellationTokenSource cancellationTokenSource;
        private readonly DirectoryInfo directoryInfo;

        private readonly object enableLock = new object();

        public PeriodicFileSystemWatcher(string path, string filter, TimeSpan interval)
        {
            this.path = path;
            this.filter = filter;
            this.interval = interval;

            directoryInfo = new DirectoryInfo(path);
        }

        public bool EnableRaisingEvents
        {
            get => cancellationTokenSource != null;
            set
            {
                lock(enableLock)
                {
                    if(cancellationTokenSource != null && !value)
                    {
                        cancellationTokenSource.Cancel();
                    }
                    else if(cancellationTokenSource == null && value)
                    {
                        cancellationTokenSource = new CancellationTokenSource();
                        cancellationTokenSource.Token.Register(() => cancellationTokenSource = null);
                        RunPeriodicLoop(cancellationTokenSource.Token);
                    }
                }
            }
        }

        private async void RunPeriodicLoop(CancellationToken cancellationToken)
        {
            var lastSnapshot = GetSnapshot();
            while(!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(interval, cancellationToken);
                
                var currentSnapshot = GetSnapshot();
                var (removed, created, updated) = Compare(lastSnapshot, currentSnapshot);
                removed.ForEach(o => Changed?.Invoke(this, new FileSystemEventArgs(WatcherChangeTypes.Deleted, o.DirectoryName, o.Name)));
                created.ForEach(o => Changed?.Invoke(this, new FileSystemEventArgs(WatcherChangeTypes.Created, o.DirectoryName, o.Name)));
                updated.ForEach(o => Changed?.Invoke(this, new FileSystemEventArgs(WatcherChangeTypes.Changed, o.DirectoryName, o.Name)));
            }
        }

        private Arr<FileInfo> GetSnapshot() => directoryInfo.EnumerateFiles(filter).ToArr();

        private (Arr<FileInfo> Removed, Arr<FileInfo> Created, Arr<FileInfo> Updated) Compare(Arr<FileInfo> oldSnapshot, Arr<FileInfo> newSnapshot)
        {
            var newFiles = newSnapshot.Except(oldSnapshot, FileNameComparer.Instance);
            var removedFiles = oldSnapshot.Except(newSnapshot, FileNameComparer.Instance);
            var sameFilePairs =
                from newFile in newSnapshot
                join oldFile in oldSnapshot on newFile.FullName equals oldFile.FullName
                select (oldFile, newFile);
            var updatedFiles = sameFilePairs.Where(tuple => tuple.newFile.LastWriteTime > tuple.oldFile.LastWriteTime).Select(o => o.newFile);

            return (removedFiles.ToArr(), newFiles.ToArr(), updatedFiles.ToArr());
        }

        public event FileSystemEventHandler Changed;
    }
}
