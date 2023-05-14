using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Interface
{
    ///<inheritdoc cref="IRepositoryBase{TEntity}"/>
    public interface IRepository<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        bool ImportCsv(string fileName);

        bool ImportCsv(byte[] byteArray);

        ValueTask<bool> ImportCsvAsync(string fileName, CancellationToken cancellationToken = default);

        ValueTask<bool> ImportCsvAsync(byte[] byteArray, CancellationToken cancellationToken = default);

        bool ExportCsv(string fileName);

        ValueTask<bool> ExportCsvAsync(string fileName, CancellationToken cancellationToken = default);
    }
}