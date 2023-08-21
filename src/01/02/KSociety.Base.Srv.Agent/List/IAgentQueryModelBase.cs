namespace KSociety.Base.Srv.Agent.List
{
    using InfraSub.Shared.Interface;
    using System.Threading;

    public interface IAgentQueryModelBase<T, out TList>
        where T : IObject
        where TList : IList<T>
    {
        TList LoadAllRecords(CancellationToken cancellationToken = default);
    }
}
