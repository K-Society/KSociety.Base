// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAgentDatabaseControl
    {
        string GetConnectionString(CancellationToken cancellationToken = default);

        ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken = default);

        bool EnsureCreated(CancellationToken cancellationToken = default);

        ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default);

        bool EnsureDeleted(CancellationToken cancellationToken = default);

        ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default);

        void Migration(CancellationToken cancellationToken = default);

        ValueTask MigrationAsync(CancellationToken cancellationToken = default);
    }
}