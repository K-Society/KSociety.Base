using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.Infra.Shared.Interface;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.App.Shared.ReqHdlr
{
    public class MigrationReqHdlr : IRequestHandler, IRequestHandlerAsync
    {
        private readonly ILogger<MigrationReqHdlr> _logger;
        private readonly IDatabaseUnitOfWork _unitOfWork;

        public MigrationReqHdlr(ILogger<MigrationReqHdlr> logger, IDatabaseUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            _unitOfWork.Migrate();
        }

        public async ValueTask ExecuteAsync()
        {
            await _unitOfWork.MigrateAsync().ConfigureAwait(false);
        }

        public async ValueTask ExecuteAsync(CancellationToken cancellationToken)
        {
            await _unitOfWork.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
