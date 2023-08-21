namespace KSociety.Base.InfraSub.Shared.Interface
{
    using System.Collections.Generic;

    public interface IList<T> where T : IObject
    {
        List<T> List { get; set; }
        int Count { get; }
    }
}