using System.Collections.Generic;
using ProtoBuf;

namespace KSociety.Base.App.Shared;

/// <inheritdoc/>
[ProtoContract]
public class AppList<T> : IAppList<T> where T : IRequest
{
    /// <inheritdoc/>
    [ProtoMember(1)]
    public List<T> List
    {
        get; set;
    }
}