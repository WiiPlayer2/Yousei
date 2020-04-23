using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SharpYaml;
using SharpYaml.Serialization;
using SharpYaml.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yousei.Tools;

namespace Yousei
{
    class JobRegistry
    {
        public class SerializerBackend : DefaultObjectSerializerBackend
        {
            public static SerializerBackend Instance { get; } = new SerializerBackend();

            private SerializerBackend() { }

            public override object ReadMemberValue(ref ObjectContext objectContext, IMemberDescriptor memberDescriptor, object memberValue, Type memberType)
            {
                if (memberType == typeof(JToken))
                {
                    return JToken.FromObject(base.ReadMemberValue(ref objectContext, memberDescriptor, memberValue, typeof(object)));
                }
                return base.ReadMemberValue(ref objectContext, memberDescriptor, memberValue, memberType);
            }
        }

        public event EventHandler<Job> JobRemoved;

        public event EventHandler<Job> JobAdded;

        private readonly Dictionary<string, Job> jobs = new Dictionary<string, Job>();
        private readonly ILogger<JobRegistry> logger;
        private readonly Serializer yamlSerializer = new Serializer(new SerializerSettings { ObjectSerializerBackend = SerializerBackend.Instance });
        private readonly IFileSystemWatcher folderWatcher;
        private readonly DirectoryInfo folderInfo;

        public JobRegistry(IConfiguration configuration, ILogger<JobRegistry> logger)
        {
            this.logger = logger;

            var folderPath = configuration.GetValue<string>("Jobs");
            folderInfo = new DirectoryInfo(folderPath);
            if (folderInfo.Exists)
            {
                var baseFileSystemWatcher = new PeriodicFileSystemWatcher(folderInfo.FullName, "*.yaml", TimeSpan.FromSeconds(10));
                folderWatcher = new DelayFileSystemWatcher(baseFileSystemWatcher, TimeSpan.FromSeconds(2));
                folderWatcher.Changed += FolderWatcher_Changed;
            }
        }

        public void Initialize()
        {
            if (folderInfo.Exists)
            {
                foreach (var file in folderInfo.EnumerateFiles("*.yaml"))
                {
                    TryRegister(file.FullName);
                }

                folderWatcher.EnableRaisingEvents = true;
            }
        }

        private void FolderWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(e.FullPath))
                return;

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    TryRegister(e.FullPath);
                    break;

                case WatcherChangeTypes.Changed:
                    Remove(e.FullPath);
                    TryRegister(e.FullPath);
                    break;

                case WatcherChangeTypes.Deleted:
                    Remove(e.FullPath);
                    break;

                case WatcherChangeTypes.Renamed:
                    var renamedEventArgs = e as RenamedEventArgs;
                    Remove(renamedEventArgs.OldFullPath);
                    TryRegister(renamedEventArgs.FullPath);
                    break;
            }
        }

        private void Remove(string path)
        {
            if (jobs.Remove(Path.GetFullPath(path), out var job))
                JobRemoved?.Invoke(this, job);
        }

        private void TryRegister(string path)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var job = yamlSerializer.Deserialize<Job>(stream);
                    if (job == null)
                        return;

                    jobs.Add(Path.GetFullPath(path), job);
                    JobAdded?.Invoke(this, job);
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Could not deserialize {path}. {e}");
            }
        }
    }
}
