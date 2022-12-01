using KSociety.Base.App.Shared;
using KSociety.Base.App.Utility.Dto.Res.Control;
using KSociety.Base.Infra.Abstraction.Interface;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.App.Utility.ReqHdlr
{
    public class EnsureCreatedReqHdlr :
        IRequestHandlerWithResponse<EnsureCreated>,
        IRequestHandlerWithResponseAsync<EnsureCreated>
    {
        private readonly ILogger<EnsureCreatedReqHdlr> _logger;
        private readonly IDatabaseUnitOfWork _unitOfWork;

        public EnsureCreatedReqHdlr(ILogger<EnsureCreatedReqHdlr> logger, IDatabaseUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public EnsureCreated Execute()
        {
            return new EnsureCreated(_unitOfWork.EnsureCreated());
        }

        public async ValueTask<EnsureCreated> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return new EnsureCreated(await _unitOfWork.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false));
        }
    }
}