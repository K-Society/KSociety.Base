using KSociety.Base.App.Shared;
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
            return new EnsureDeleted(_unitOfWork.EnsureDeleted());
        }

        public async ValueTask<EnsureDeleted> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return new EnsureDeleted(await _unitOfWork.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false));
        }
    }
}