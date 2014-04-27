using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class CauseProvider<T> : ValueProvider<T, TriggeredGameEffect<T>>
    {
        public T GetValue(TriggeredGameEffect<T> effect, Game game, T outputType)
        {
            return effect.Cause;
        }
    }
}
