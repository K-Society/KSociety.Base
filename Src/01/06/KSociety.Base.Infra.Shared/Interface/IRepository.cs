using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Interface
{
    public interface IRepository<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        bool ImportCsv(string fileName);

        ValueTask<bool> ImportCsvAsync(string fileName, CancellationToken cancellationToken = default);

        bool ExportCsv(string fileName);

        ValueTask<bool> ExportCsvAsync(string fileName, CancellationToken cancellationToken = default);
    }
}
