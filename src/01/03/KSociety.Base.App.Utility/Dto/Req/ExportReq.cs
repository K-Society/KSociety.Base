namespace KSociety.Base.App.Utility.Dto.Req
{
    using Shared;
    using KSociety.Base.InfraSub.Shared.Interface;
    using ProtoBuf;

    [ProtoContract]
    public class ExportReq : IRequest, IExport
    {
        [ProtoMember(1)] public string FileName { get; set; }

        public ExportReq() { }

        public ExportReq(string fileName)
        {
            this.FileName = fileName;
        }
    }
}
