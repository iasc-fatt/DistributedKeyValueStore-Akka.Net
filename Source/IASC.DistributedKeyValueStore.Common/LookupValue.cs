namespace IASC.DistributedKeyValueStore.Common
{
    public class LookupValue
    {
        public string Key { get; private set; }

        public LookupValue(string key)
        {
            Key = key;
        }
    }
}