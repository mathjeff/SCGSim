using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class EndTurn_Effect : GameEffect
    {
        public EndTurn_Effect(ID<Readable_GamePlayer> controllerId)
        {
            this.ControllerID = controllerId;
        }
        public override GameEffect Clone(GameEffect outputType)
        {
            return new EndTurn_Effect(this.ControllerID);
        }
        public override void Process(Game game)
        {
            game.EndTurn();
        }
        public override string ToString(Game game)
        {
            return "End turn for " + game.Get_ReadableSnapshot(this.ControllerID);
        }
    }
}
