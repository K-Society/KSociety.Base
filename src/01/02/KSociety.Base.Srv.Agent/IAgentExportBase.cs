namespace KSociety.Base.Srv.Agent
{
    using System.Threading;

    public interface IAgentExportBase<in TExportReq, out TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        TExportRes ExportData(TExportReq exportReq, CancellationToken cancellationToken = default);
    }
}