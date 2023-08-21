namespace KSociety.Base.App.Utility.ReqHdlr
{
    using Shared;
    using Dto.Res.Control;
    using Infra.Abstraction.Interface;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;

    public class EnsureCreatedReqHdlr :
        IRequestHandlerWithResponse<EnsureCreated>,
        IRequestHandlerWithResponseAsync<EnsureCreated>
    {
        private readonly ILogger<EnsureCreatedReqHdlr> _logger;
        private readonly IDatabaseUnitOfWork _unitOfWork;

        public EnsureCreatedReqHdlr(ILogger<EnsureCreatedReqHdlr> logger, IDatabaseUnitOfWork unitOfWork)
        {
            this._logger = logger;
            this._unitOfWork = unitOfWork;
        }

        public EnsureCreated Execute()
        {
            var result = this._unitOfWork.EnsureCreated();
            if (result.HasValue)
            {
                return new EnsureCreated(result.Value);
            }
            return new EnsureCreated(false);
        }

        public async ValueTask<EnsureCreated> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var result = await this._unitOfWork.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
            if (result.HasValue)
            {
                return new EnsureCreated(result.Value);
            }
            return new EnsureCreated(false);
        }
    }
}
