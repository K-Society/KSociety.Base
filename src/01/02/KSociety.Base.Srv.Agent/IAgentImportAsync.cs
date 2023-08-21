namespace KSociety.Base.Srv.Agent
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAgentImportAsync<in TImportReq, TImportRes>
        where TImportReq : class
        where TImportRes : class
    {
        ValueTask<TImportRes> ImportDataAsync(TImportReq importReq, CancellationToken cancellationToken = default);
    }
}
