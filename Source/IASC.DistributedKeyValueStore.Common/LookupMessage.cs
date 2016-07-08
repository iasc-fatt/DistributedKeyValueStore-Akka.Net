namespace IASC.DistributedKeyValueStore.Common
{
    public class LookupMessage
    {
        public string Key { get; private set; }

        public LookupMessage(string key)
        {
            Key = key;
        }
    }
}