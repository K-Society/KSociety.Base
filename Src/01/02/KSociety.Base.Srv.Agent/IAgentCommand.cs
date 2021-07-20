using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentCommand<
        in TRemove,
        in TAddReq,
        TAddRes,
        in TUpdateReq,
        TUpdateRes,
        in TCopyReq,
        TCopyRes,
        in TModifyField>
        where TRemove : class
        where TAddReq : class
        where TAddRes : class
        where TUpdateReq : class
        where TUpdateRes : class
        where TCopyReq : class
        where TCopyRes : class
        where TModifyField : class
    {
        bool Remove(TRemove removeItem);
        bool Remove(TRemove removeItem, CancellationToken cancellationToken);

        ValueTask<bool> RemoveAsync(TRemove removeItem);
        ValueTask<bool> RemoveAsync(TRemove removeItem, CancellationToken cancellationToken);

        TAddRes Add(TAddReq addItem);
        TAddRes Add(TAddReq addItem, CancellationToken cancellationToken);

        ValueTask<TAddRes> AddAsync(TAddReq addItem);
        ValueTask<TAddRes> AddAsync(TAddReq addItem, CancellationToken cancellationToken);

        TUpdateRes Update(TUpdateReq updateItem);
        TUpdateRes Update(TUpdateReq updateItem, CancellationToken cancellationToken);

        ValueTask<TUpdateRes> UpdateAsync(TUpdateReq updateItem);
        ValueTask<TUpdateRes> UpdateAsync(TUpdateReq updateItem, CancellationToken cancellationToken);

        TCopyRes Copy(TCopyReq copyItem);
        TCopyRes Copy(TCopyReq copyItem, CancellationToken cancellationToken);

        ValueTask<TCopyRes> CopyAsync(TCopyReq copyItem);
        ValueTask<TCopyRes> CopyAsync(TCopyReq copyItem, CancellationToken cancellationToken);

        bool ModifyField(TModifyField modifyFieldItem);
        bool ModifyField(TModifyField modifyFieldItem, CancellationToken cancellationToken);

        ValueTask<bool> ModifyFieldAsync(TModifyField modifyFieldItem);
        ValueTask<bool> ModifyFieldAsync(TModifyField modifyFieldItem, CancellationToken cancellationToken);
    }
}
