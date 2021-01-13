using System;
using KSociety.Base.Domain.Shared.Class;

namespace KSociety.Base.Infra.Shared.Test.Csv
{
    public class DtoTestClass3 : BaseEntity
    {
        #region [Propery]
        //public Guid Id { get; private set; }

        public int ClassTypeId { get; private set; }
        public virtual ClassType ClassType { get; protected set; }

        public string Name { get; private set; }

        public string Ip { get; private set; }

        public bool Enable { get; private set; }

        public bool Extra { get; private set; }

        #endregion

        public DtoTestClass3(/*Guid id,*/ int classTypeId, string name, string ip, bool enable)
        {
            //Id = id;
            ClassTypeId = classTypeId;
            Name = name;
            Ip = ip;
            Enable = enable;
            Name = "Urka";
        }

        public DtoTestClass3(string name, string ip, bool enable)
        {
            ClassTypeId = 0;
            Name = name;
            Ip = ip;
            Enable = enable;
            Name = "Urka2";
        }

        protected DtoTestClass3()
        {

        }
    }
}
