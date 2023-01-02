using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentExportAsync<in TExportReq, TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        ValueTask<TExportRes> ExportDataAsync(TExportReq exportReq, CancellationToken cancellationToken = default);
    }
}
