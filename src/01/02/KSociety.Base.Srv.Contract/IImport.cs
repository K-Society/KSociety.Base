namespace KSociety.Base.Srv.Contract
{
    using ProtoBuf.Grpc;
    using ProtoBuf.Grpc.Configuration;

    [Service]
    public interface IImport<in TImportReq, out TImportRes>
        where TImportReq : class
        where TImportRes : class
    {
        [Operation]
        TImportRes ImportData(TImportReq importReq, CallContext context = default);
    }
}
