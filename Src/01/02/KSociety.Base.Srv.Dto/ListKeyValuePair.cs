using System.Collections.Generic;
using ProtoBuf;

namespace KSociety.Base.Srv.Dto;

[ProtoContract]
public class ListKeyValuePair<TKey, TValue>
{
    [ProtoMember(1)]
    public List<KeyValuePair<TKey, TValue>> List
    {
        get; set;
    }

    public ListKeyValuePair()
    {
    }

    public ListKeyValuePair(List<KeyValuePair<TKey, TValue>> keyValuePairs)
    {
        List = keyValuePairs;
    }
}