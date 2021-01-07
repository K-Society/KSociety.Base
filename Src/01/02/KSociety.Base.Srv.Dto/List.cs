using System;
using System.Collections.Generic;
using KSociety.Base.InfraSub.Shared.Interface;
using ProtoBuf;
using ProtoBuf.Meta;

namespace KSociety.Base.Srv.Dto
{
    [ProtoContract]
    public class ObjectList<T> : InfraSub.Shared.Interface.IList<T> where T : IObject
    {
        [ProtoMember(1)]
        public List<T> List
        {
            get; set;
        }

        public static void AddBaseTypeProtoMembers(MetaType metaType, Type derivedType)
        {
            //var myType = runtimeTypeModel[typeof(MachineTarget)];

            metaType.AddSubType(100, derivedType);
        }

        //[DataMember]
        //public List<PropertyInfo> GetProperties()
        //{
        //    List<string> output


        //    return typeof(T).GetProperties().ToList();
        //}
    }
}
