namespace KSociety.Base.Srv.Agent
{
    public interface IAgentCommandImportExportAsync<
        in TAddReq, TAddRes,
        in TUpdateReq, TUpdateRes,
        in TCopyReq, TCopyRes,
        in TModifyFieldReq, TModifyFieldRes,
        in TRemoveReq, TRemoveRes,
        in TImportReq, TImportRes,
        in TExportReq, TExportRes> : IAgentCommandAsync<TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes,
            TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes>,
        IAgentImportAsync<TImportReq, TImportRes>,
        IAgentExportAsync<TExportReq, TExportRes>
        where TAddReq : class
        where TAddRes : class
        where TUpdateReq : class
        where TUpdateRes : class
        where TCopyReq : class
        where TCopyRes : class
        where TModifyFieldReq : class
        where TModifyFieldRes : class
        where TRemoveReq : class
        where TRemoveRes : class
        where TImportReq : class
        where TImportRes : class
        where TExportReq : class
        where TExportRes : class
    {
    }
}
