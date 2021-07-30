﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;
using Yousei.Web.Api;

namespace Yousei.Web.Model
{
    public abstract class ConfigModel
    {
        public ConfigModel(YouseiApi api)
        {
            Api = api;
        }

        protected YouseiApi Api { get; }

        public abstract Task Delete();

        public abstract Task<SourceConfig?> Load();

        public abstract Task Save(SourceConfig content);
    }
}