using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Web.Api;

namespace Yousei.Web.Shared.Editors
{
    public abstract class EditorBase : ComponentBase
    {
        public abstract bool IsValid { get; }

        [Parameter]
        public string Type { get; set; } = default!;

        [Parameter]
        public TypeKind TypeKind { get; set; } = TypeKind.Any;

        public abstract JToken BuildToken();
    }
}