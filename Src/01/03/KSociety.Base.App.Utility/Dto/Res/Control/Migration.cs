using KSociety.Base.App.Shared;
using ProtoBuf;

namespace KSociety.Base.App.Utility.Dto.Res.Control
{
    [ProtoContract]
    public class Migration : IResponse
    {
        [ProtoMember(1)] public bool Result { get; set; }

        public Migration() { }

        public Migration(bool result)
        {
            Result = result;
        }
    }
}