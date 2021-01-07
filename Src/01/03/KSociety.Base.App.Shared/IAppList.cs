using System.Collections.Generic;

namespace KSociety.Base.App.Shared
{
    public interface IAppList<T> where T : IRequest
    {
        List<T> List { get; set; }
    }
}
