// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Interface
{
    using System.Threading;
    using System.Threading.Tasks;

    ///<inheritdoc cref="IRepositoryBase{TEntity}"/>
    public interface IRepository<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        bool ImportCsv(string fileName);

        bool ImportCsv(byte[] byteArray);

        //ValueTask<bool> ImportCsvAsync(string fileName, CancellationToken cancellationToken = default);

        //ValueTask<bool> ImportCsvAsync(byte[] byteArray, CancellationToken cancellationToken = default);

        bool ExportCsv(string fileName);

        //ValueTask<bool> ExportCsvAsync(string fileName, CancellationToken cancellationToken = default);
    }
}
