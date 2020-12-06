using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.App.Shared.Dto.Res.Control;
using KSociety.Base.Infra.Shared.Interface;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.App.Shared.ReqHdlr
{
    public class GetConnectionStringReqHdlr : IRequestHandlerWithResponse<ConnectionString>
    {
        private readonly ILogger<MigrationReqHdlr> _logger;
        private readonly IDbUnitOfWork _unitOfWork;

        public GetConnectionStringReqHdlr(ILogger<MigrationReqHdlr> logger, IDbUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public ConnectionString Execute()
        {
            return new ConnectionString(_unitOfWork.GetConnectionString());
        }

        public async ValueTask<ConnectionString> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var result = await _unitOfWork.GetConnectionStringAsync(cancellationToken).ConfigureAwait(false);
            return new ConnectionString(result);
        }
    }
}
