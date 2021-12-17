using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent;

public interface IAgentImportModel<in TImportReq, TImportRes>
    where TImportReq : class
    where TImportRes : class
{
    TImportRes Import(TImportReq importReq, CancellationToken cancellationToken = default);

    ValueTask<TImportRes> ImportAsync(TImportReq importReq, CancellationToken cancellationToken = default);
}