namespace KSociety.Base.Srv.Contract
{
    using ProtoBuf.Grpc;
    using ProtoBuf.Grpc.Configuration;
    using System.Threading.Tasks;

    [Service]
    public interface ICommandAsync<
        in TAddReq, TAddRes,
        in TUpdateReq, TUpdateRes,
        in TCopyReq, TCopyRes,
        in TModifyFieldReq, TModifyFieldRes,
        in TRemoveReq, TRemoveRes>
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
        ValueTask<TAddRes> AddAsync(TAddReq request, CallContext context = default);

        [Operation]
        ValueTask<TUpdateRes> UpdateAsync(TUpdateReq request, CallContext context = default);

        [Operation]
        ValueTask<TCopyRes> CopyAsync(TCopyReq request, CallContext context = default);

        [Operation]
        ValueTask<TModifyFieldRes> ModifyFieldAsync(TModifyFieldReq request, CallContext context = default);

        [Operation]
        ValueTask<TRemoveRes> RemoveAsync(TRemoveReq request, CallContext context = default);
    }
}
