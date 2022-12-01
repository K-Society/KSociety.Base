using KSociety.Base.App.Shared;
using KSociety.Base.InfraSub.Shared.Interface;
using ProtoBuf;

namespace KSociety.Base.App.Utility.Dto.Req
{
    [ProtoContract]
    public class ImportReq : IRequest, IImport
    {
        [ProtoMember(1)] public string FileName { get; set; }

        [ProtoMember(2)] public byte[] ByteArray { get; set; }

        public ImportReq() { }

        public ImportReq(string fileName, byte[] byteArray)
        {
            FileName = fileName;
            ByteArray = byteArray;
        }
    }
}