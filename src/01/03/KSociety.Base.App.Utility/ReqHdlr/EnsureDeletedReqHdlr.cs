// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.App.Utility.ReqHdlr
{
    using Shared;
    using Dto.Res.Control;
    using Infra.Abstraction.Interface;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;

    public class EnsureDeletedReqHdlr :
        IRequestHandlerWithResponse<EnsureDeleted>,
        IRequestHandlerWithResponseAsync<EnsureDeleted>
    {
        private readonly ILogger<EnsureDeletedReqHdlr> _logger;
        private readonly IDatabaseUnitOfWork _unitOfWork;

        public EnsureDeletedReqHdlr(ILogger<EnsureDeletedReqHdlr> logger, IDatabaseUnitOfWork unitOfWork)
        {
            this._logger = logger;
            this._unitOfWork = unitOfWork;
        }

        public EnsureDeleted Execute()
        {
            var result = this._unitOfWork.EnsureDeleted();
            if (result.HasValue)
            {
                return new EnsureDeleted(result.Value);
            }
            return new EnsureDeleted(false);
        }

        public async ValueTask<EnsureDeleted> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var result = await this._unitOfWork.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
            if (result.HasValue)
            {
                return new EnsureDeleted(result.Value);
            }
            return new EnsureDeleted(false);
        }
    }
}
