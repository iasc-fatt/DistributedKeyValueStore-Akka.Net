using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IASC.DistributedKeyValueStore.Server
{
    class PathSelectorEnvelope
    {
        public object Message { get; }
        public string Path { get; }

        public PathSelectorEnvelope(object msg, string path)
        {
            Message = msg;
            Path = path;
        }
    }
}
