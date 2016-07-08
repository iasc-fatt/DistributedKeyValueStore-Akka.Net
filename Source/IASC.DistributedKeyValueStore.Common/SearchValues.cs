namespace IASC.DistributedKeyValueStore.Common
{
    public class SearchValues
    {
        public string ValueToCompare { get; private set; }

        /// <summary>
        /// For example "ge"
        /// g: greater
        /// e: equal
        /// l: lower
        /// </summary>
        public string Comparison { get; private set; }

        public SearchValues(string valueToCompare, string comparison)
        {
            ValueToCompare = valueToCompare;
            Comparison = comparison;
        }
    }
}