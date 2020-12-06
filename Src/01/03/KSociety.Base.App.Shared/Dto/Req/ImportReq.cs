using KSociety.Base.InfraSub.Shared.Interface;
using ProtoBuf;

namespace KSociety.Base.App.Shared.Dto.Req
{
    [ProtoContract]
    public class ImportReq : IRequest, IKbImport
    {
        [ProtoMember(1)]
        public string FileName { get; set; }

        public ImportReq() { }

        public ImportReq(string fileName)
        {
            FileName = fileName;
        }
    }
}
