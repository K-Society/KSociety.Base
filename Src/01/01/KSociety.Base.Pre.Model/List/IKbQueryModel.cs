using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.Pre.Model.List
{
    public interface IKbQueryModel<T, TList>
        where T : IObject
        where TList : IKbList<T>
    {
        TList LoadAllRecords(CancellationToken cancellationToken = default);

        ValueTask<TList> LoadAllRecordsAsync(CancellationToken cancellationToken = default);
    }
}
