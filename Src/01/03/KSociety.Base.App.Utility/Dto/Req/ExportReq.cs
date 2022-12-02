using KSociety.Base.App.Shared;
using KSociety.Base.InfraSub.Shared.Interface;
using ProtoBuf;

namespace KSociety.Base.App.Utility.Dto.Req
{
    [ProtoContract]
    public class ExportReq : IRequest, IExport
    {
        [ProtoMember(1)] public string FileName { get; set; }

        public ExportReq() { }

        public ExportReq(string fileName)
        {
            FileName = fileName;
        }
    }
}