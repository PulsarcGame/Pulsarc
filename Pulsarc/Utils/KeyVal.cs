namespace Pulsarc.Utils
{
    public class KeyVal<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public KeyVal() { }

        public KeyVal(TKey key, TValue val)
        {
            Key = key;
            Value = val;
        }
    }
}
