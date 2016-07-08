namespace IASC.DistributedKeyValueStore.Common
{
    public class InsertValue
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

        public InsertValue(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}