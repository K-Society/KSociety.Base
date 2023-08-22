// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.App.Utility.ReqHdlr
{
    using Shared;
    using Infra.Abstraction.Interface;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;

    public class MigrationReqHdlr : IRequestHandler, IRequestHandlerAsync
    {
        private readonly ILogger<MigrationReqHdlr> _logger;
        private readonly IDatabaseUnitOfWork _unitOfWork;

        public MigrationReqHdlr(ILogger<MigrationReqHdlr> logger, IDatabaseUnitOfWork unitOfWork)
        {
            this._logger = logger;
            this._unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            this._unitOfWork.Migrate();
        }

        public async ValueTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await this._unitOfWork.MigrateAsync(null, cancellationToken).ConfigureAwait(false);
        }
    }
}
