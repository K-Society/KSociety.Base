using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Contract
{
    [Service]
    public interface IExportAsync<in TExportReq, TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        [Operation]
        ValueTask<TExportRes> ExportDataAsync(TExportReq exportReq, CallContext context = default);
    }
}
