using System.Threading;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentExport<in TExportReq, TExportRes> : IAgentExportAsync<TExportReq, TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        TExportRes ExportData(TExportReq exportReq, CancellationToken cancellationToken = default);
    }
}