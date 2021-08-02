using BlazorMonaco;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Yousei.Shared;
using Yousei.Web.Api;
using Yousei.Web.Model;

namespace Yousei.Web.Pages
{
    public partial class Index
    {
        private ConfigModel? currentConfig = null;

        private MonacoEditor? editor;

        private bool isDirty = false;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Inject]
        public YouseiApi Api { get; set; }

        [Inject]
        public IJSRuntime Js { get; set; }

        [Inject]
        public ILogger<Index> Logger { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private StandaloneEditorConstructionOptions Construct(MonacoEditor editor)
            => new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Language = "yaml",
                Theme = "vs-dark",
                ReadOnly = true,
                TabIndex = 2,
            };

        private void EditorContentChanged()
        {
            if (isDirty)
                return;

            isDirty = true;
            StateHasChanged();
        }

        private async Task Save()
        {
            if (currentConfig is null || editor is null || currentConfig.IsReadOnly)
                return;

            try
            {
                var content = await editor.GetValue();
                var model = await editor.GetModel();
                //var language = await js.InvokeAsync<string>("blazorMonaco.editor.getModelLanguage", editor.Id, model.Id);
                var language = "yaml";
                await currentConfig.Save(new SourceConfig(language, content));
                isDirty = false;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Exception while saving content.");
            }
        }

        private async Task SetConfig(ConfigModel configModel)
        {
            if (editor is null)
                return;

            if (isDirty)
            {
                var result = await Js.InvokeAsync<bool>("confirm", "This config has not been saved yet. Are you sure you want to load another one?");
                if (!result)
                    return;
            }

            string? content = default;
            try
            {
                var sourceConfig = await configModel.Load();
                content = sourceConfig?.Content ?? string.Empty;
                var model = await editor.GetModel();
                await Js.InvokeVoidAsync("blazorMonaco.editor.setModelLanguage", editor.Id, model.Id, sourceConfig?.Language ?? string.Empty);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Exception while loading content.");
            }

            content ??= string.Empty;
            await editor.SetValue(content);
            currentConfig = configModel;
            isDirty = false;

            await editor.UpdateOptions(new()
            {
                ReadOnly = currentConfig.IsReadOnly,
            });
        }
    }
}