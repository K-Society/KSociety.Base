using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Contract;

[Service]
public interface IImportAsync<in TImportReq, TImportRes>
    where TImportReq : class
    where TImportRes : class
{
    [Operation]
    ValueTask<TImportRes> ImportDataAsync(TImportReq importReq, CallContext context = default);
}
