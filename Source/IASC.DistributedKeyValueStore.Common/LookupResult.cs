namespace IASC.DistributedKeyValueStore.Common
{
    public class LookupResult
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

        public LookupResult(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}