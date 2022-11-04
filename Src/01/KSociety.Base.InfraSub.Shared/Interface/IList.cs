using System.Collections.Generic;

namespace KSociety.Base.InfraSub.Shared.Interface;

public interface IList<T> where T : IObject
{
    List<T> List { get; set; }
    int Count { get; }
}