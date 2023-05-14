namespace KSociety.Base.Srv.Agent
{
    public interface IAgentExport<in TExportReq, TExportRes> : IAgentExportBase<TExportReq, TExportRes>, IAgentExportAsync<TExportReq, TExportRes>
        where TExportReq : class
        where TExportRes : class
    {

    }
}