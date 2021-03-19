namespace Yousei.Contracts
{
    public interface IConnection
    {
        IFlowAction CreateAction(string name);

        IFlowTrigger CreateTrigger(string name);
    }
}