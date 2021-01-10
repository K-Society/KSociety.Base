using CsvHelper.Configuration.Attributes;
using System;

namespace KSociety.Base.Infra.Shared.Test.Csv
{
    public class DtoTestClass
    {
        #region [Propery]
        public Guid Id { get; private set; }

        public int ClassTypeId { get; private set; }

        public string Name { get; private set; }

        public string Ip { get; private set; }

        public bool Enable { get; private set; }

        [Ignore]
        public bool ExtraEnable { get; private set; }

        #endregion

        public DtoTestClass(Guid id, int classTypeId, string name, string ip, bool enable)
        {
            Id = id;
            ClassTypeId = classTypeId;
            Name = name + "Uffa";
            Ip = "127.0.0.1";
            Enable = enable;
        }

        public DtoTestClass(Guid id, int classTypeId, string name, string ip, bool enable, bool extraEnable)
        {
            Id = id;
            ClassTypeId = classTypeId;
            Name = name;
            Ip = ip;
            Enable = enable;
            ExtraEnable = extraEnable;
        }

        public DtoTestClass()
        {

        }
    }
}
