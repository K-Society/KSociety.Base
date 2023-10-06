// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

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
        string GetConnectionString();

        /// <summary>
        /// GetConnectionStringAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken = default);
    }
}
