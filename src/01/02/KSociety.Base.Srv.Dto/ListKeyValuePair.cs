// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Dto
{
    using System.Collections.Generic;
    using ProtoBuf;

    [ProtoContract]
    public class ListKeyValuePair<TKey, TValue>
    {
        [ProtoMember(1)]
        public List<KeyValuePair<TKey, TValue>>? List
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
