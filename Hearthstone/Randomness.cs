using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class Randomness
    {
        public static Random Random
        {
            get
            {
                return singletonRandom;
            }
        }
        static Random singletonRandom = new Random();
    }
}
