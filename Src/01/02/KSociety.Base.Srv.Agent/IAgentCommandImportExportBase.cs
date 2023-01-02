namespace KSociety.Base.Srv.Agent
{
    public interface IAgentCommandImportExportBase<
        in TAddReq, out TAddRes,
        in TUpdateReq, out TUpdateRes,
        in TCopyReq, out TCopyRes,
        in TModifyFieldReq, out TModifyFieldRes,
        in TRemoveReq, out TRemoveRes,
        in TImportReq, out TImportRes,
        in TExportReq, out TExportRes> : IAgentCommandBase<TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes,
            TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes>,
        IAgentImportBase<TImportReq, TImportRes>,
        IAgentExportBase<TExportReq, TExportRes>
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
