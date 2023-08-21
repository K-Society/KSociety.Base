namespace KSociety.Base.Srv.Dto
{
    using ProtoBuf;

    [ProtoContract]
    public class KeyValuePair<TKey, TValue>
    {
        [ProtoMember(1), CompatibilityLevel(CompatibilityLevel.Level200)]
        public TKey Key { get; set; }

        [ProtoMember(2), CompatibilityLevel(CompatibilityLevel.Level200)]
        public TValue Value { get; set; }

        private KeyValuePair() { }

        public KeyValuePair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
