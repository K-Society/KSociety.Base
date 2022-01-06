using System;
using System.Collections.Generic;
using KSociety.Base.InfraSub.Shared.Interface;
using ProtoBuf;
using ProtoBuf.Meta;

namespace KSociety.Base.Srv.Dto;

[ProtoContract]
public class ObjectList<T> : InfraSub.Shared.Interface.IList<T> where T : IObject
{
    [ProtoMember(1)]
    public List<T> List
    {
        get; set;
    }

    public int Count
    {
        get
        {
            return List?.Count ?? 0;
        }
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

    //public void AddRange(InfraSub.Shared.Interface.IList<T> items)
    //{
    //    List.AddRange(items.List);
    //}

    //public void AddRange(IEnumerable<T> items)
    //{
    //    List.AddRange(items);
    //}
}