using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public interface Strategy
    {
        GameEffect ChooseBestAction(GameChoice choice, Game game);
    }
}
