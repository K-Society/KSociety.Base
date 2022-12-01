//using System.Threading;
//using System.Threading.Tasks;
//using KSociety.Base.App.Shared.Dto.Res.Control;
//using KSociety.Base.Infra.Shared.Interface;
//using Microsoft.Extensions.Logging;

//namespace KSociety.Base.App.Shared.ReqHdlr
//{
//    public class EnsureCreatedReqHdlr :
//        IRequestHandlerWithResponse<EnsureCreated>,
//        IRequestHandlerWithResponseAsync<EnsureCreated>
//    {
//        private readonly ILogger<EnsureCreatedReqHdlr> _logger;
//        private readonly IDatabaseUnitOfWork _unitOfWork;

//        public EnsureCreatedReqHdlr(ILogger<EnsureCreatedReqHdlr> logger, IDatabaseUnitOfWork unitOfWork)
//        {
//            _logger = logger;
//            _unitOfWork = unitOfWork;
//        }

//        public EnsureCreated Execute()
//        {
//            return new EnsureCreated(_unitOfWork.EnsureCreated());
//        }

//        public async ValueTask<EnsureCreated> ExecuteAsync(CancellationToken cancellationToken = default)
//        {
//            return new EnsureCreated(await _unitOfWork.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false));
//        }
//    }
//}