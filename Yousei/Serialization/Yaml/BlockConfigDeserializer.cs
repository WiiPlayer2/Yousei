using System;
using System.Collections.Generic;
using Yousei.Shared;
using YamlDotNet.Serialization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Yousei.Serialization.Yaml
{
    internal class BlockConfigDeserializer : INodeDeserializer
    {
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value)
        {
            if (expectedType != typeof(BlockConfig))
            {
                value = default;
                return false;
            }

            if (reader.TryConsume<Scalar>(out var scalar))
            {
                value = new BlockConfig
                {
                    Type = scalar.Value
                };
                return true;
            }

            var map = nestedObjectDeserializer(reader, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            if (map is null)
            {
                value = default;
                return false;
            }

            var config = new BlockConfig();
            if (map.Remove("type", out var type))
                config.Type = (string)type;
            if (map.Remove("configuration", out var configuration))
                config.Configuration = (string)configuration;
            config.Arguments = map;
            value = config;
            return true;
        }
    }
}