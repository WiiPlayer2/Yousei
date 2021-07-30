using BlazorMonaco;
using GraphQL;
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
        private Dictionary<string, List<string>> configurations = new();

        private ConfigModel? currentConfig = null;

        private MonacoEditor? editor;

        private List<string> flows = new();

        private bool isReadOnly = true;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Inject]
        public GraphQlRequestHandler Api { get; set; }

        [Inject]
        public YouseiApi Api2 { get; set; }

        [Inject]
        public IJSRuntime Js { get; set; }

        [Inject]
        public ILogger<Index> Logger { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override Task OnInitializedAsync() => LoadData();

        private async Task AddConfiguration(string connector)
        {
            if (isReadOnly)
                return;

            var name = await Js.InvokeAsync<string>("prompt", "Enter configuration name:", string.Empty);
            if (string.IsNullOrWhiteSpace(name))
                return;

            configurations[connector].Add(name);
            await SetConfig(new ConnectionConfigModel(connector, name, Api));
            this.StateHasChanged();
        }

        private async Task AddConnector()
        {
            if (isReadOnly)
                return;

            var name = await Js.InvokeAsync<string>("prompt", "Enter connector name:", string.Empty);
            if (string.IsNullOrWhiteSpace(name))
                return;

            configurations.Add(name, new List<string>());
            this.StateHasChanged();
        }

        private async Task AddFlow()
        {
            if (isReadOnly)
                return;

            var name = await Js.InvokeAsync<string>("prompt", "Enter flow name:", string.Empty);
            if (string.IsNullOrWhiteSpace(name))
                return;

            flows.Add(name);
            await SetConfig(new FlowConfigModel(name, Api));
            this.StateHasChanged();
        }

        private StandaloneEditorConstructionOptions Construct(MonacoEditor editor)
            => new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Language = "yaml",
                Theme = "vs-dark",
                ReadOnly = true,
            };

        private async Task Delete()
        {
            if (currentConfig is null || isReadOnly)
                return;

            await currentConfig.Delete();
            StateHasChanged();
        }

        private async Task LoadData()
        {
            var result = await Api2.LoadData.ExecuteAsync();
            if (result.Data is null)
                return;

            isReadOnly = result.Data.Database.IsReadOnly;
            flows = result.Data.Database.Flows.Select(o => o.Name).ToList();
            configurations = result.Data.Database.Connections.ToDictionary(
                o => o.Id,
                o => o.Configurations.Select(o => o.Name).ToList());
        }

        private async Task Reload()
        {
            await Api2.Reload.ExecuteAsync();
            await LoadData();
        }

        private async Task Save()
        {
            if (currentConfig is null || editor is null || isReadOnly)
                return;

            try
            {
                var content = await editor.GetValue();
                var model = await editor.GetModel();
                //var language = await js.InvokeAsync<string>("blazorMonaco.editor.getModelLanguage", editor.Id, model.Id);
                var language = "yaml";
                await currentConfig.Save(new SourceConfig(language, content));
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

            await editor.UpdateOptions(new()
            {
                ReadOnly = isReadOnly,
            });
        }
    }
}