namespace KSociety.Base.Srv.Dto
{
    using System.Collections.Generic;
    using ProtoBuf;

    [ProtoContract]
    public class ListKeyValuePair<TKey, TValue>
    {
        [ProtoMember(1)]
        public List<KeyValuePair<TKey, TValue>> List
        {
            get;
            set;
        }

        public ListKeyValuePair()
        {
        }

        public ListKeyValuePair(List<KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            this.List = keyValuePairs;
        }
    }
}
