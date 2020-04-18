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
                if(memberType == typeof(JToken))
                {
                    return JToken.FromObject(base.ReadMemberValue(ref objectContext, memberDescriptor, memberValue, typeof(object)));
                }
                return base.ReadMemberValue(ref objectContext, memberDescriptor, memberValue, memberType);
            }
        }

        private readonly List<Job> jobs = new List<Job>();
        private readonly ILogger<JobRegistry> logger;
        private readonly Serializer yamlSerializer = new Serializer(new SerializerSettings { ObjectSerializerBackend = SerializerBackend.Instance });

        public JobRegistry(IConfiguration configuration, ILogger<JobRegistry> logger)
        {
            this.logger = logger;

            var folderPath = configuration.GetValue<string>("Jobs");
            var directoryInfo = new DirectoryInfo(folderPath);
            if(directoryInfo.Exists)
            {
                foreach(var file in directoryInfo.EnumerateFiles("*.yaml"))
                {
                    using (var stream = File.OpenRead(file.FullName))
                    {
                        try
                        {
                            var job = yamlSerializer.Deserialize<Job>(stream);
                        }
                        catch(Exception e)
                        {
                            logger.LogError($"Could not deserialize {file.FullName}. {e}");
                        }
                    }
                }
            }
        }

        public IReadOnlyCollection<Job> Jobs => jobs;

        public void Register(Job job) => jobs.Add(job);
    }
}
