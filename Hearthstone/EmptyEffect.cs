using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class EmptyEffect : GameEffect
    {
        public EmptyEffect()
        {
        }
        public override void Process(Game game)
        {
        }
        public override GameEffect Clone(GameEffect outputType)
        {
            return new EmptyEffect();
        }
    }
}
