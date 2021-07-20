using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.App.Shared.Dto.Res.Control;
using KSociety.Base.Infra.Shared.Interface;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.App.Shared.ReqHdlr
{
    public class GetConnectionStringReqHdlr : 
        IRequestHandlerWithResponse<ConnectionString>,
        IRequestHandlerWithResponseAsync<ConnectionString>
    {
        private readonly ILogger<MigrationReqHdlr> _logger;
        private readonly IDatabaseUnitOfWork _unitOfWork;

        public GetConnectionStringReqHdlr(ILogger<MigrationReqHdlr> logger, IDatabaseUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public ConnectionString Execute()
        {
            return new ConnectionString(_unitOfWork.GetConnectionString());
        }

        public async ValueTask<ConnectionString> ExecuteAsync()
        {
            var result = await _unitOfWork.GetConnectionStringAsync().ConfigureAwait(false);
            return new ConnectionString(result);
        }

        public async ValueTask<ConnectionString> ExecuteAsync(CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.GetConnectionStringAsync(cancellationToken).ConfigureAwait(false);
            return new ConnectionString(result);
        }
    }
}
