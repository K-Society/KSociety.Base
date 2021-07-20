using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentExportModel<in TExportReq, TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        TExportRes Export(TExportReq exportReq);
        TExportRes Export(TExportReq exportReq, CancellationToken cancellationToken);

        ValueTask<TExportRes> ExportAsync(TExportReq exportReq);
        ValueTask<TExportRes> ExportAsync(TExportReq exportReq, CancellationToken cancellationToken);
    }
}
