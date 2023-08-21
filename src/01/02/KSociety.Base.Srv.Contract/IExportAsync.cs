namespace KSociety.Base.Srv.Contract
{
    using ProtoBuf.Grpc;
    using ProtoBuf.Grpc.Configuration;
    using System.Threading.Tasks;

    [Service]
    public interface IExportAsync<in TExportReq, TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        [Operation]
        ValueTask<TExportRes> ExportDataAsync(TExportReq exportReq, CallContext context = default);
    }
}
