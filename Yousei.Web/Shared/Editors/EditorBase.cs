using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yousei.Web.Shared.Editors
{
    public abstract class EditorBase : ComponentBase
    {
        public virtual bool IsAllValid => IsValid;

        public abstract bool IsValid { get; }

        public abstract JToken BuildToken();
    }
}