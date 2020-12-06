using ProtoBuf;

namespace KSociety.Base.Srv.Dto
{
    [ProtoContract]
    public class KbKeyValuePair<TKey, TValue>
    {
        [ProtoMember(1), CompatibilityLevel(CompatibilityLevel.Level200)]
        public TKey Key { get; set; }

        [ProtoMember(2), CompatibilityLevel(CompatibilityLevel.Level200)]
        public TValue Value { get; set; }

        private KbKeyValuePair() { }

        public KbKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}
