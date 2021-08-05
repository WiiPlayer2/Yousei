using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Web.Shared.Editors
{
    public class ValueEditor : EditorBase
    {
        private static readonly Dictionary<string, Type> editorCreators = new();

        private EditorBase? editor;

        static ValueEditor()
        {
            Add<BlockConfig, BlockConfigEditor>();
        }

        public override bool IsValid => editor?.IsValid ?? false;

        public override JToken BuildToken()
            => (editor ?? throw new InvalidOperationException()).BuildToken();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var editorType = GetEditorType(Type);
            if (editorType is null)
            {
                builder.AddContent(0, $"No editor for {{{Type}}} available.");
            }
            else
            {
                builder.OpenComponent(0, editorType);
                builder.AddAttribute(1, nameof(TypeKind), TypeKind);
                builder.AddAttribute(2, nameof(Type), Type);
                builder.AddComponentReferenceCapture(3, obj => editor = (EditorBase)obj);
                builder.CloseComponent();
            }

            base.BuildRenderTree(builder);
        }

        private static void Add<TEditor>(string type)
            where TEditor : EditorBase
            => editorCreators.Add(type, typeof(TEditor));

        private static void Add<TType, TEditor>()
            where TEditor : EditorBase
            => Add<TEditor>(typeof(TType).FullName!);

        private Type? GetEditorType(string type)
            => !editorCreators.TryGetValue(type, out var editorType)
                ? TypeKind == Api.TypeKind.Object
                    ? typeof(ObjectTypeEditor)
                    : null
                : editorType;
    }
}