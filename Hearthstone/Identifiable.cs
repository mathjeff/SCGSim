using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// This Identifable class just stores an integer id for lookups
// The reason it's parameterized is to avoid accidentally using an ID of the wrong type (such as trying to look up a Monster having the same ID as its controller)
namespace Games
{
    public interface Identifiable<T>
    {
        // unfortunately we have to pass an input parameter to specify the type of output parameter we want (since C# won't let us just have T always match the class's type)
        ID<T> GetID(T outputType);
    }

}
