using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class IDFactory
    {
        static int nextId = 0;
        public static int NewID()
        {
            return nextId++;
        }
    }
}
