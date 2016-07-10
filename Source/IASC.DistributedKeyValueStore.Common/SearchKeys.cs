namespace IASC.DistributedKeyValueStore.Common
{
    public class SearchKeys
    {
        public string Regex { get; private set; }

        public SearchKeys(string regex)
        {
            Regex = regex;
        }
    }
}