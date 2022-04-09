using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace KSociety.Base.Srv.Contract;

[Service]
public interface IExport<in TExportReq, out TExportRes>
    where TExportReq : class
    where TExportRes : class
{
    [Operation]
    TExportRes ExportData(TExportReq exportReq, CallContext context = default);
}
