using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentImport<in TImportReq, TImportRes>
        where TImportReq : class
        where TImportRes : class
    {
        TImportRes ImportData(TImportReq importReq, CancellationToken cancellationToken = default);

        ValueTask<TImportRes> ImportDataAsync(TImportReq importReq, CancellationToken cancellationToken = default);
    }
}