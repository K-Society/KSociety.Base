using System;

namespace KSociety.Base.Infra.Shared.Test.Csv.Dto
{
    public class TestClassPrivateSetter
    {

        #region [Propery]
        public Guid Id { get; private set; }

        public int ClassTypeId { get; private set; }

        public string Name { get; private set; }

        public string Ip { get; private set; }

        public bool Enable { get; private set; }

        public string Ahh { get; private set; }

        #endregion

        public TestClassPrivateSetter(Guid id, int classTypeId, string name, string ip, bool enable, string ahh)
        {
            Id = id;
            ClassTypeId = classTypeId;
            Name = name;
            Ip = ip;
            Enable = enable;
            Ahh = ahh;
        }

        public TestClassPrivateSetter(Guid id, int classTypeId, string name, string ip, bool enable, string ahh, string test)
        {
            Id = id;
            ClassTypeId = classTypeId;
            Name = name;
            Ip = ip;
            Enable = enable;
            Ahh = ahh;
            Ahh = test;
        }

        public TestClassPrivateSetter(Guid id, int classTypeId, string ip, bool enable)
        {
            Id = id;
            ClassTypeId = classTypeId;
            Name = "MaDai";
            Ip = ip;
            Enable = enable;
            Ahh = "MaDai";
        }

        //Do not use this constructor.
        //public TestClassPrivateSetter()
        //{

        //}
    }
}
