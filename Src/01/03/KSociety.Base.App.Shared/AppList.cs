using System.Collections.Generic;
using ProtoBuf;

namespace KSociety.Base.App.Shared
{
    [ProtoContract]
    public class AppList<T> : IAppList<T> where T : IRequest
    {
        [ProtoMember(1)]
        public List<T> List
        {
            get; set;
        }
    }
}
