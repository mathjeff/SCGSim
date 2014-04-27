using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    interface ReadableDictionary<TKey, TValue>
    {
        bool TryGetValue(TKey key, out TValue output);
    }
}
