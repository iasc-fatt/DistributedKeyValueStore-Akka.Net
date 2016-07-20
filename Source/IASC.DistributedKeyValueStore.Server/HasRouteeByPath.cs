using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IASC.DistributedKeyValueStore.Server
{

    class HasRouteeByPath
    {
        public string Path { get; private set; }

        public HasRouteeByPath(string path)
        {
            Path = path;
        }
    }
}
