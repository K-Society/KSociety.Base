using System;

namespace KSociety.Base.Infra.Shared.Test.Csv
{
    public class DtoTestClass4 : DtoTestClass3
    {
        public int Int { get; private set; }
        public string Ahh { get; private set; }
        public string SuperPippo { get; private set; } = "SuperPippo";

        public DtoTestClass4(Int32 intt, /*Guid id,*/ int classTypeId, string name, string ip, bool enable, string ahh)
            :base(/*id,*/ classTypeId, name, ip, enable)
        {
            Int = intt;
            Ahh = ahh;
        }

        public DtoTestClass4(int classTypeId, string name, string ip, bool enable)
            : base(classTypeId, name, ip, enable)
        {

        }

        private DtoTestClass4()
        {

        }
    }
}
