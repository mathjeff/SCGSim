using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public interface ReadableQueue<TValue> where TValue : Identifiable<TValue>
    {
        int CurrentNumItems { get; } // current number of cards
        int NextDrawIndex { get; } // index into the array at which to draw next
        IReadOnlyList<ID<TValue>> Keys { get; } // list of cards from which we generate this deck
        IReadOnlyDictionary<ID<TValue>, KeyValuePair<int, TValue>> BackingItems { get; } // list of cards from which we generate this deck
        WritableQueue<TValue> Clone();
    }
}
