namespace IASC.DistributedKeyValueStore.Common
{
    public class KillActor
    {
        public int Hash { get; private set; }

        public KillActor(int hash)
        {
            Hash = hash;
        }
    }
}