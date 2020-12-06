using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Interface
{
    public interface IDbUnitOfWork : IUnitOfWork
    {
        string GetConnectionString();

        ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken = default);
    }
}
