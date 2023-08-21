namespace KSociety.Base.Srv.Agent
{
    using InfraSub.Shared.Interface;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAgentQueryModelAsync<in TObject, T>
        where TObject : IIdObject
        where T : IObject
    {
        ValueTask<T> FindAsync(TObject idObject, CancellationToken cancellationToken = default);
    }
}
