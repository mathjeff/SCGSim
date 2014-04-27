using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// This ID class just stores an integer id for lookups
// The reason it's parameterized is to avoid accidentally using an ID of the wrong type (such as trying to look up a Monster having the same ID as its controller) without an explicit cast
namespace Games
{
    // for creating temporary ID's 
    public class ID<T>
    {
        public ID(int id)
        {
            this.id = id;
        }
        public ID(Identifiable<T> item)
        {
            this.id = item.GetID(default(T)).id;
        }
        public ID<T2> AsType<T2>(T2 outputType)
        {
            return new ID<T2>(this.id);
        }
        public int ToInt()
        {
            return this.id;
        }

        public override int GetHashCode()
        {
            return this.id;
        }
        public override bool Equals(object obj)
        {
            ID<T> converted = obj as ID<T>;
            if (converted != null)
            {
                if (this.id == converted.id)
                    return true;
            }
            return false;
        }

        private int id;

    }



}
