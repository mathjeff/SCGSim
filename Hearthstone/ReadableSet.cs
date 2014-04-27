using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public interface ReadableSet<TReadable, TWritable> where TReadable : Identifiable<TReadable> where TWritable : Identifiable<TReadable>
    {
        WriteControlled_Set<TReadable, TWritable> Clone();
        TReadable GetReadable(ID<TReadable> key);
        bool TryGetReadable(ID<TReadable> key, out TReadable value);
        
    }
}
