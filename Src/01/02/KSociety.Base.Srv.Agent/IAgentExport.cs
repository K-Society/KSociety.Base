using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent;

public interface IAgentExport<in TExportReq, TExportRes>
    where TExportReq : class
    where TExportRes : class
{
    TExportRes ExportData(TExportReq exportReq, CancellationToken cancellationToken = default);

    ValueTask<TExportRes> ExportDataAsync(TExportReq exportReq, CancellationToken cancellationToken = default);
}