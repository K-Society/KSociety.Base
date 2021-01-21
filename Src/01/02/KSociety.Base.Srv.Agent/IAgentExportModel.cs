using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentExportModel<in TExportReq, TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        TExportRes Export(TExportReq exportReq, CancellationToken cancellationToken = default);

        ValueTask<TExportRes> ExportAsync(TExportReq exportReq, CancellationToken cancellationToken = default);
    }
}
