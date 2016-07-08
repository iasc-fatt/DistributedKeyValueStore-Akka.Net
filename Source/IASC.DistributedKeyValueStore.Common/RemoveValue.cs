namespace IASC.DistributedKeyValueStore.Common
{
    public class RemoveValue
    {
        public string Key { get; private set; }

        public RemoveValue(string key)
        {
            Key = key;
        }
    }
}