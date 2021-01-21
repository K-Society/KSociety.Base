//using System.Threading;
//using System.Threading.Tasks;
//using KSociety.Base.InfraSub.Shared.Interface;

//namespace KSociety.Base.Pre.Model
//{
//    public interface IQueryModel<T, in TObject>
//        where T : IObject
//        where TObject : IIdObject
//    {
//        T Find(TObject idObject, CancellationToken cancellationToken = default);

//        ValueTask<T> FindAsync(TObject idObject, CancellationToken cancellationToken = default);
//    }
//}
