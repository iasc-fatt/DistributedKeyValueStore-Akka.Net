using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
