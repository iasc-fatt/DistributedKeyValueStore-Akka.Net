namespace IASC.DistributedKeyValueStore.Common
{
    public class KillActor
    {
        public string Path { get; private set; }

        public KillActor(string path)
        {
            Path = path;
        }
    }
}