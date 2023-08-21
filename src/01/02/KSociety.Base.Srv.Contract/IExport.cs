namespace KSociety.Base.Srv.Contract
{
    using ProtoBuf.Grpc;
    using ProtoBuf.Grpc.Configuration;

    [Service]
    public interface IExport<in TExportReq, out TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        [Operation]
        TExportRes ExportData(TExportReq exportReq, CallContext context = default);
    }
}
