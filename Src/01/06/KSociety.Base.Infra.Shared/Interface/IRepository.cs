using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Interface
{
    public interface IRepository<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        bool ImportCsv(string fileName);

        bool ImportCsv(byte[] byteArray);

        ValueTask<bool> ImportCsvAsync(string fileName);
        ValueTask<bool> ImportCsvAsync(string fileName, CancellationToken cancellationToken);

        ValueTask<bool> ImportCsvAsync(byte[] byteArray);
        ValueTask<bool> ImportCsvAsync(byte[] byteArray, CancellationToken cancellationToken);

        bool ExportCsv(string fileName);

        ValueTask<bool> ExportCsvAsync(string fileName);
        ValueTask<bool> ExportCsvAsync(string fileName, CancellationToken cancellationToken);
    }
}
