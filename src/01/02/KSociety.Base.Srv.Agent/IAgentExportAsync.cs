namespace KSociety.Base.Srv.Agent
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAgentExportAsync<in TExportReq, TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        ValueTask<TExportRes> ExportDataAsync(TExportReq exportReq, CancellationToken cancellationToken = default);
    }
}
