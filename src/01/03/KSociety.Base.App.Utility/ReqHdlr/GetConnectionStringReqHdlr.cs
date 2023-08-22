// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.App.Utility.ReqHdlr
{
    using Shared;
    using Dto.Res.Control;
    using Infra.Abstraction.Interface;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetConnectionStringReqHdlr :
        IRequestHandlerWithResponse<ConnectionString>,
        IRequestHandlerWithResponseAsync<ConnectionString>
    {
        private readonly ILogger<GetConnectionStringReqHdlr> _logger;
        private readonly IDatabaseUnitOfWork _unitOfWork;

        public GetConnectionStringReqHdlr(ILogger<GetConnectionStringReqHdlr> logger, IDatabaseUnitOfWork unitOfWork)
        {
            this._logger = logger;
            this._unitOfWork = unitOfWork;
        }

        public ConnectionString Execute()
        {
            return new ConnectionString(this._unitOfWork.GetConnectionString());
        }

        public async ValueTask<ConnectionString> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var result = await this._unitOfWork.GetConnectionStringAsync(cancellationToken).ConfigureAwait(false);
            return new ConnectionString(result);
        }
    }
}
