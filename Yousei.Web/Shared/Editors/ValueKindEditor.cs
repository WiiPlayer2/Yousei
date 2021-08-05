using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Yousei.Web.Api;

namespace Yousei.Web.Shared.Editors
{
    public class ValueKindEditor : EditorBase
    {
        private EditorBase? editor;

        public override bool IsValid => TypeKind == TypeKind.Unit || (editor?.IsValid ?? false);

        [Parameter]
        public string Type { get; set; } = default!;

        [Parameter]
        public TypeKind TypeKind { get; set; } = TypeKind.Any;

        public override JToken BuildToken()
        {
            if (TypeKind == TypeKind.Unit)
                return JToken.FromObject(Unit.Default);

            if (editor is null)
                throw new InvalidOperationException();

            return editor.BuildToken();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (Type is null)
                return;

            switch (TypeKind)
            {
                case TypeKind.Scalar:
                case TypeKind.Object:
                    builder.OpenComponent<ValueEditor>(0);
                    builder.AddAttribute(1, nameof(ValueEditor.Type), Type);
                    builder.AddComponentReferenceCapture(2, obj => editor = (EditorBase)obj);
                    builder.CloseComponent();
                    break;

                case TypeKind.Any:
                    builder.OpenComponent<RawEditor>(0);
                    builder.AddComponentReferenceCapture(1, obj => editor = (EditorBase)obj);
                    builder.CloseComponent();
                    break;

                case TypeKind.Unit:
                    builder.AddContent(0, "<not required>");
                    break;

                default:
                    //throw new NotImplementedException();
                    builder.OpenElement(0, "div");
                    builder.AddAttribute(1, "class", "flex flex-col");
                    builder.AddMarkupContent(2, @$"<span class=""text-red-600 font-bold"">{TypeKind}</span><span class=""text-red-600 font-bold"">{Type}</span>");
                    builder.OpenComponent<RawEditor>(3);
                    builder.AddComponentReferenceCapture(4, obj => editor = (EditorBase)obj);
                    builder.CloseComponent();
                    builder.CloseElement();
                    break;
            }

            base.BuildRenderTree(builder);
        }
    }
}