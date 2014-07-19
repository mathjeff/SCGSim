using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public interface GameEvaluator
    {
        double EstimateWinProbability(Game game, ID<Readable_GamePlayer> gamePlayer);
        Dictionary<ID<Readable_GamePlayer>, double> EstimateWinProbabilities(Game game);
    }
}
