namespace IASC.DistributedKeyValueStore.Common
{
    public class InsertMessage
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

        public InsertMessage(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}