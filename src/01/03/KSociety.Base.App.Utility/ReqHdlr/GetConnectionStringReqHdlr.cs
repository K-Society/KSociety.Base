using KSociety.Base.App.Shared;
using KSociety.Base.App.Utility.Dto.Res.Control;
using KSociety.Base.Infra.Abstraction.Interface;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.App.Utility.ReqHdlr
{
    public class GetConnectionStringReqHdlr :
        IRequestHandlerWithResponse<ConnectionString>,
        IRequestHandlerWithResponseAsync<ConnectionString>
    {
        private readonly ILogger<GetConnectionStringReqHdlr> _logger;
        private readonly IDatabaseUnitOfWork _unitOfWork;

        public GetConnectionStringReqHdlr(ILogger<GetConnectionStringReqHdlr> logger, IDatabaseUnitOfWork unitOfWork)
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