using System.Collections.Generic;
using ProtoBuf;

namespace KSociety.Base.Srv.Dto
{
    [ProtoContract]
    public class KbListKeyValuePair<TKey, TValue>
    {
        [ProtoMember(1)]
        public List<KbKeyValuePair<TKey, TValue>> List
        {
            get; set;
        }

        public KbListKeyValuePair()
        {
        }

        public KbListKeyValuePair(List<KbKeyValuePair<TKey, TValue>> keyValuePairs)
        {
            List = keyValuePairs;
        }
    }
}
