using System.Threading.Tasks;
using YouseiReloaded;

internal interface IParameter
{
    Task<object> Resolve(FlowContext context);
}