using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Interface
{
    public interface IRepository<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        void ExportCsv(string fileName);

        ValueTask ExportCsvAsync(string fileName, CancellationToken cancellationToken = default);
    }
}
