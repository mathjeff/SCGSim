using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class EndTurn_Effect : GameEffect
    {
        public EndTurn_Effect()
        {

        }
        public override GameEffect Clone(GameEffect outputType)
        {
            return new EndTurn_Effect();
        }
        public override void Process(Game game)
        {
            game.EndTurn();
        }
    }
}
