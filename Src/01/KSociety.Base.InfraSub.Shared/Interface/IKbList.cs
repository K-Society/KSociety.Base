using System.Collections.Generic;

namespace KSociety.Base.InfraSub.Shared.Interface
{
    public interface IKbList<T> where T : IObject
    {
        List<T> List { get; set; }
    }
}
