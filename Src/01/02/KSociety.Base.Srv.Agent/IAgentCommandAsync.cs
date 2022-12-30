using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentCommandAsync<
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
        ValueTask<TAddRes> AddAsync(TAddReq addItem, CancellationToken cancellationToken = default);
        ValueTask<TUpdateRes> UpdateAsync(TUpdateReq updateItem, CancellationToken cancellationToken = default);
        ValueTask<TCopyRes> CopyAsync(TCopyReq copyItem, CancellationToken cancellationToken = default);
        ValueTask<TModifyFieldRes> ModifyFieldAsync(TModifyFieldReq modifyFieldItem, CancellationToken cancellationToken = default);
        ValueTask<TRemoveRes> RemoveAsync(TRemoveReq removeItem, CancellationToken cancellationToken = default);
    }
}
