//using System.Threading;
//using System.Threading.Tasks;

//namespace KSociety.Base.Pre.Model
//{
//    public interface IImportModel<in TImportReq, TImportRes>
//        where TImportReq : class
//        where TImportRes : class
//    {
//        TImportRes Import(TImportReq importReq, CancellationToken cancellationToken = default);

//        ValueTask<TImportRes> ImportAsync(TImportReq importReq, CancellationToken cancellationToken = default);
//    }
//}
