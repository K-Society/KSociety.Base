namespace KSociety.Base.Infra.Abstraction.Interface
{
    using System.Threading;
    using System.Threading.Tasks;

    ///<inheritdoc/>
    public interface IDatabaseUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// GetConnectionString
        /// </summary>
        /// <returns></returns>
        string? GetConnectionString();

        /// <summary>
        /// GetConnectionStringAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<string?> GetConnectionStringAsync(CancellationToken cancellationToken = default);
    }
}