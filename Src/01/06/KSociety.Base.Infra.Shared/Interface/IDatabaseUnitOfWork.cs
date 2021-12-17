using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Interface;

/// <summary>
/// The DatabaseUnitOfWork class.
/// </summary>
public interface IDatabaseUnitOfWork : IUnitOfWork
{
    /// <summary>
    /// GetConnectionString
    /// </summary>
    /// <returns></returns>
    string GetConnectionString();

    /// <summary>
    /// GetConnectionStringAsync
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken = default);
}