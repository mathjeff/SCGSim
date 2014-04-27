using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public interface Strategy
    {
        GameEffect ChooseBestAction(IEnumerable<GameEffect> effects, Readable_GamePlayer chooser, Game game);
        //Readable_LifeTarget Choose_LifeTarget(Game game, GameEffect sourceEffect);

    }
}
