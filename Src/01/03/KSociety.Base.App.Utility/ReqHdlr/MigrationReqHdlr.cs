using KSociety.Base.App.Shared;
using KSociety.Base.Infra.Abstraction.Interface;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.App.Utility.ReqHdlr
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

        public async ValueTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await _unitOfWork.MigrateAsync(null, cancellationToken).ConfigureAwait(false);
        }
    }
}