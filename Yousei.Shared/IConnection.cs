namespace Yousei.Shared
{
    public interface IConnection
    {
        IFlowAction? CreateAction(string name);

        IFlowTrigger? CreateTrigger(string name);
    }
}