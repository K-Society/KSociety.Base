using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Pre.Model
{
    public interface ICommandModel<
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
        bool Remove(TRemove removeItem, CancellationToken cancellationToken = default);

        ValueTask<bool> RemoveAsync(TRemove removeItem, CancellationToken cancellationToken = default);

        TAddRes Add(TAddReq addItem, CancellationToken cancellationToken = default);

        ValueTask<TAddRes> AddAsync(TAddReq addItem, CancellationToken cancellationToken = default);

        TUpdateRes Update(TUpdateReq updateItem, CancellationToken cancellationToken = default);

        ValueTask<TUpdateRes> UpdateAsync(TUpdateReq updateItem, CancellationToken cancellationToken = default);

        TCopyRes Copy(TCopyReq copyItem, CancellationToken cancellationToken = default);

        ValueTask<TCopyRes> CopyAsync(TCopyReq copyItem, CancellationToken cancellationToken = default);

        bool ModifyField(TModifyField modifyFieldItem, CancellationToken cancellationToken = default);

        ValueTask<bool> ModifyFieldAsync(TModifyField modifyFieldItem, CancellationToken cancellationToken = default);
    }
}
