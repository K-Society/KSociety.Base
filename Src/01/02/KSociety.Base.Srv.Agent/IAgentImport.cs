using System.Threading;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentImport<in TImportReq, TImportRes> : IAgentImportAsync<TImportReq, TImportRes>
        where TImportReq : class
        where TImportRes : class
    {
        TImportRes ImportData(TImportReq importReq, CancellationToken cancellationToken = default);
    }
}