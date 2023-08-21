namespace KSociety.Base.Srv.Agent
{
    using InfraSub.Shared.Interface;
    using System.Threading;

    public interface IAgentQueryModelBase<in TObject, out T>
        where TObject : IIdObject
        where T : IObject
    {
        T Find(TObject idObject, CancellationToken cancellationToken = default);
    }
}
