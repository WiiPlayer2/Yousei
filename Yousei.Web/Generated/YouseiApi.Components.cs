﻿// <auto-generated/>
#nullable enable

namespace Yousei.Web.Api.Components
{
    [global::System.CodeDom.Compiler.GeneratedCode("StrawberryShake", "12.0.0.0")]
    public partial class LoadDataRenderer : global::StrawberryShake.Razor.QueryBase<global::Yousei.Web.Api.ILoadDataResult>
    {
        [global::Microsoft.AspNetCore.Components.InjectAttribute]
        internal global::Yousei.Web.Api.LoadDataQuery Operation { get; set; } = default !;
        protected override void OnInitialized()
        {
            Subscribe(Operation.Watch(strategy: Strategy));
        }

        protected override void OnParametersSet()
        {
            Subscribe(Operation.Watch(strategy: Strategy));
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCode("StrawberryShake", "12.0.0.0")]
    public partial class GetConfigurationRenderer : global::StrawberryShake.Razor.QueryBase<global::Yousei.Web.Api.IGetConfigurationResult>
    {
        [global::Microsoft.AspNetCore.Components.InjectAttribute]
        internal global::Yousei.Web.Api.GetConfigurationQuery Operation { get; set; } = default !;
        [global::Microsoft.AspNetCore.Components.ParameterAttribute]
        public global::System.String Connector { get; set; } = default !;
        [global::Microsoft.AspNetCore.Components.ParameterAttribute]
        public global::System.String Name { get; set; } = default !;
        protected override void OnInitialized()
        {
            Subscribe(Operation.Watch(Connector, Name, strategy: Strategy));
        }

        protected override void OnParametersSet()
        {
            Subscribe(Operation.Watch(Connector, Name, strategy: Strategy));
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCode("StrawberryShake", "12.0.0.0")]
    public partial class GetFlowRenderer : global::StrawberryShake.Razor.QueryBase<global::Yousei.Web.Api.IGetFlowResult>
    {
        [global::Microsoft.AspNetCore.Components.InjectAttribute]
        internal global::Yousei.Web.Api.GetFlowQuery Operation { get; set; } = default !;
        [global::Microsoft.AspNetCore.Components.ParameterAttribute]
        public global::System.String FlowName { get; set; } = default !;
        protected override void OnInitialized()
        {
            Subscribe(Operation.Watch(FlowName, strategy: Strategy));
        }

        protected override void OnParametersSet()
        {
            Subscribe(Operation.Watch(FlowName, strategy: Strategy));
        }
    }
}
