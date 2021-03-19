namespace YouseiReloaded
{
    interface IConnection
    {
        IFlowTrigger CreateTrigger(string name);

        IFlowAction CreateAction(string name);
    }
}