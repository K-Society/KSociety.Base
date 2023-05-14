using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace KSociety.Base.Srv.Contract
{
    [Service]
    public interface ICommand<
        in TAddReq, out TAddRes,
        in TUpdateReq, out TUpdateRes,
        in TCopyReq, out TCopyRes,
        in TModifyFieldReq, out TModifyFieldRes,
        in TRemoveReq, out TRemoveRes>
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
    {
        [Operation]
        TAddRes Add(TAddReq request, CallContext context = default);

        [Operation]
        TUpdateRes Update(TUpdateReq request, CallContext context = default);

        [Operation]
        TCopyRes Copy(TCopyReq request, CallContext context = default);

        [Operation]
        TModifyFieldRes ModifyField(TModifyFieldReq request, CallContext context = default);

        [Operation]
        TRemoveRes Remove(TRemoveReq request, CallContext context = default);
    }
}
