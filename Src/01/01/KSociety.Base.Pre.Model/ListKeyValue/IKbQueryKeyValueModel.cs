using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.Srv.Dto;

namespace KSociety.Base.Pre.Model.ListKeyValue
{
    public interface IKbQueryKeyValueModel<TList, TKey, TValue>
    where TList : KbListKeyValuePair<TKey, TValue>
    {
        TList LoadData(CancellationToken cancellationToken = default);

        ValueTask<TList> LoadDataAsync(CancellationToken cancellationToken = default);
    }
}
