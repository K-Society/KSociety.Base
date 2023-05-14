﻿using KSociety.Base.App.Shared;
using KSociety.Base.App.Utility.Dto.Res.Control;
using KSociety.Base.Infra.Abstraction.Interface;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.App.Utility.ReqHdlr
{
    public class EnsureDeletedReqHdlr :
        IRequestHandlerWithResponse<EnsureDeleted>,
        IRequestHandlerWithResponseAsync<EnsureDeleted>
    {
        private readonly ILogger<EnsureDeletedReqHdlr> _logger;
        private readonly IDatabaseUnitOfWork _unitOfWork;

        public EnsureDeletedReqHdlr(ILogger<EnsureDeletedReqHdlr> logger, IDatabaseUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public EnsureDeleted Execute()
        {
            var result = _unitOfWork.EnsureDeleted();
            if (result.HasValue)
            {
                return new EnsureDeleted(result.Value);
            }
            return new EnsureDeleted(false);
        }

        public async ValueTask<EnsureDeleted> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var result = await _unitOfWork.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
            if (result.HasValue)
            {
                return new EnsureDeleted(result.Value);
            }
            return new EnsureDeleted(false);
        }
    }
}