namespace KSociety.Base.Srv.Agent
{
    public interface IAgentImport<in TImportReq, TImportRes> : IAgentImportBase<TImportReq, TImportRes>, IAgentImportAsync<TImportReq, TImportRes>
        where TImportReq : class
        where TImportRes : class
    {

    }
}