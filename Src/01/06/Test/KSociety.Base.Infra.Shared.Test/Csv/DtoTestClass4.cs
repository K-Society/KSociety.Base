using System;

namespace KSociety.Base.Infra.Shared.Test.Csv
{
    public class DtoTestClass4 : DtoTestClass3
    {
        public string Ahh { get; private set; }
        public DtoTestClass4(Guid id, int classTypeId, string name, string ip, bool enable, string ahh)
            :base(id, classTypeId, name, ip, enable)
        {
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
